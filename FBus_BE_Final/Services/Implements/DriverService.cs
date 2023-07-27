using AutoMapper;
using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Enums;
using FBus_BE.Exceptions;
using FBus_BE.Models;
using FBus_BE.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace FBus_BE.Services.Implements
{
    public class DriverService : IDriverService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Driver, object>>> _orderDict;
        private readonly IFirebaseStorageService _storageService;
        private const string cloudStoragePrefix = @"https://firebasestorage.googleapis.com/v0/b/fbus-388009.appspot.com/o/";

        public DriverService(FbusMainContext context, IMapper mapper, IFirebaseStorageService storageService)
        {
            this.errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Driver, object>>> {
                { "id", driver => driver.Id }
            };
            _storageService = storageService;
        }

        public async Task<bool> ChangeStatus(int id, string status)
        {
            Driver? driver = await _context.Drivers.Include(driver => driver.Account).FirstOrDefaultAsync(driver => driver.Id == id);
            if (driver != null)
            {
                if (driver.Status != (byte)DriverStatusEnum.Deleted)
                {
                    status = TextUtil.Capitalize(status);
                    DriverStatusEnum statusEnum;
                    switch (status)
                    {
                        case nameof(DriverStatusEnum.Active):
                            statusEnum = DriverStatusEnum.Active;
                            break;
                        case nameof(DriverStatusEnum.Inactive):
                            statusEnum = DriverStatusEnum.Inactive;
                            break;
                        default:
                            return false;
                    }
                    driver.Account.Status = (byte)statusEnum;
                    driver.Status = (byte)statusEnum;
                    _context.Accounts.Update(driver.Account);
                    _context.Drivers.Update(driver);
                    await _context.SaveChangesAsync();
                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                throw new EntityNotFoundException("Driver", id);
            }
        }

        public async Task<DriverDto> Create(int createdById, DriverInputDto inputDto)
        {
            await CheckCreateDuplicate(inputDto);
            if (errors.IsNullOrEmpty())
            {
                Account account = new Account
                {
                    Email = inputDto.Email,
                    Code = inputDto.Code,
                    Role = nameof(RoleEnum.Driver),
                    Status = (byte)AccountStatusEnum.Unsigned,
                };
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                Driver driver = _mapper.Map<Driver>(inputDto);
                driver.AccountId = account.Id;
                driver.CreatedById = (short?)createdById;
                driver.Status = (byte)DriverStatusEnum.Active;

                Uri avatarUri;
                if (inputDto.AvatarFile != null)
                {
                    avatarUri = await _storageService.UploadFile(inputDto.Code, inputDto.AvatarFile, "avatars");
                    driver.Avatar = cloudStoragePrefix + avatarUri.AbsolutePath.Substring(avatarUri.AbsolutePath.LastIndexOf('/') + 1) + "?alt=media";
                }

                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();

                return _mapper.Map<DriverDto>(driver);
            }
            else
            {
                throw new DuplicateException(errors);
            }
        }

        public async Task<bool> Delete(int id)
        {
            Driver? driver = await _context.Drivers.Include(driver => driver.Account).FirstOrDefaultAsync(driver => driver.Id == id);
            if (driver != null)
            {
                if (driver.Status != (byte)DriverStatusEnum.Deleted)
                {
                    driver.Account.Status = (byte)DriverStatusEnum.Deleted;
                    driver.Status = (byte)DriverStatusEnum.Deleted;
                    _context.Accounts.Update(driver.Account);
                    _context.Drivers.Update(driver);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new EntityNotFoundException("Driver", id);
            }
        }

        public async Task<DriverDto> GetDetails(int id)
        {
            Driver? driver = await _context.Drivers
                .Include(driver => driver.Account)
                .Include(driver => driver.CreatedBy)
                .FirstOrDefaultAsync(driver => driver.Id == id && driver.Status != (byte)DriverStatusEnum.Deleted);
            return _mapper.Map<DriverDto>(driver);
        }

        public async Task<DefaultPageResponse<DriverListingDto>> GetList(DriverPageRequest pageRequest)
        {
            DefaultPageResponse<DriverListingDto> pageResponse = new DefaultPageResponse<DriverListingDto>();
            if (pageRequest.PageIndex == null)
            {
                pageRequest.PageIndex = 1;
            }
            if (pageRequest.PageSize == null)
            {
                pageRequest.PageSize = 10;
            }
            if (pageRequest.OrderBy == null)
            {
                pageRequest.OrderBy = "id";
            }
            DriverStatusEnum statusEnum = DriverStatusEnum.Active;
            bool validStatus = false;
            if(pageRequest.Status != null)
            {
                switch(TextUtil.Capitalize(pageRequest.Status))
                {
                    case nameof(DriverStatusEnum.Active):
                        statusEnum = DriverStatusEnum.Active;
                        validStatus = true;
                        break;
                    case nameof(DriverStatusEnum.Inactive):
                        statusEnum = DriverStatusEnum.Inactive;
                        validStatus = true;
                        break;
                }
            }
            int skippedCount = (int)((pageRequest.PageIndex - 1) * pageRequest.PageSize);
            List<DriverListingDto> drivers = new List<DriverListingDto>();
            int totalCount = await _context.Drivers
                .Where(driver => (validStatus) ? driver.Status == (byte)statusEnum : !driver.Status.Equals((int)DriverStatusEnum.Deleted))
                .Where(driver => pageRequest.Code != null && pageRequest.Email != null
                                  ? driver.Account.Code.Contains(pageRequest.Code) || driver.Account.Email.Contains(pageRequest.Email)
                                  : pageRequest.Code != null && pageRequest.Email == null
                                    ? driver.Account.Code.Contains(pageRequest.Code)
                                    : pageRequest.Code == null && pageRequest.Email != null
                                      ? driver.Account.Email.Contains(pageRequest.Email)
                                      : true)
                .CountAsync();
            if (totalCount > 0 && totalCount > skippedCount)
            {
                drivers = pageRequest.Direction == "desc"
                    ? await _context.Drivers.OrderByDescending(_orderDict[pageRequest.OrderBy.ToLower()])
                                            .Skip(skippedCount)
                                            .Include(driver => driver.Account)
                                            .Where(driver => (validStatus) ? driver.Status == (byte)statusEnum : !driver.Status.Equals((int)DriverStatusEnum.Deleted))
                                            .Where(driver => pageRequest.Code != null && pageRequest.Email != null
                                                              ? driver.Account.Code.Contains(pageRequest.Code) || driver.Account.Email.Contains(pageRequest.Email)
                                                              : pageRequest.Code != null && pageRequest.Email == null
                                                                ? driver.Account.Code.Contains(pageRequest.Code)
                                                                : pageRequest.Code == null && pageRequest.Email != null
                                                                  ? driver.Account.Email.Contains(pageRequest.Email)
                                                                  : true)
                                            .Select(driver => _mapper.Map<DriverListingDto>(driver))
                                            .ToListAsync()
                    : await _context.Drivers.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                                            .Skip(skippedCount)
                                            .Include(driver => driver.Account)
                                            .Where(driver => (validStatus) ? driver.Status == (byte)statusEnum : !driver.Status.Equals((int)DriverStatusEnum.Deleted))
                                            .Where(driver => pageRequest.Code != null && pageRequest.Email != null
                                                              ? driver.Account.Code.Contains(pageRequest.Code) || driver.Account.Email.Contains(pageRequest.Email)
                                                              : pageRequest.Code != null && pageRequest.Email == null
                                                                ? driver.Account.Code.Contains(pageRequest.Code)
                                                                : pageRequest.Code == null && pageRequest.Email != null
                                                                  ? driver.Account.Email.Contains(pageRequest.Email)
                                                                  : true)
                                            .Select(driver => _mapper.Map<DriverListingDto>(driver))
                                            .ToListAsync();
            }
            pageResponse.Data = drivers;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }

        public async Task<DriverDto> Update(int createdById, DriverInputDto inputDto, int id)
        {
            Driver? driver = await _context.Drivers
                .Include(driver => driver.Account)
                .Include(driver => driver.CreatedBy)
                .Where(driver => driver.Id == id)
                .FirstOrDefaultAsync();
            if (driver == null)
            {
                throw new EntityNotFoundException("Driver", id);
            }
            await CheckUpdateDuplicate(driver, inputDto);
            if (errors.IsNullOrEmpty())
            {
                Account account = driver.Account;
                account.Code = inputDto.Code;

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                driver = _mapper.Map(inputDto, driver);

                Uri avatarUri;
                if (inputDto.AvatarFile != null)
                {
                    if (driver.Avatar != null)
                    {
                        string fileName = driver.Avatar.Substring(driver.Avatar.LastIndexOf('/') + 1).Replace("?alt=media", "").Replace("%2F","/");
                        await _storageService.DeleteFile(fileName);
                    }

                    avatarUri = await _storageService.UploadFile(inputDto.Code, inputDto.AvatarFile, "avatars");
                    driver.Avatar = cloudStoragePrefix + avatarUri.AbsolutePath.Substring(avatarUri.AbsolutePath.LastIndexOf('/') + 1) + "?alt=media";
                }

                _context.Drivers.Update(driver);
                await _context.SaveChangesAsync();

                return _mapper.Map<DriverDto>(driver);
            }
            else
            {
                throw new DuplicateException(errors);
            }
        }

        private async Task CheckCreateDuplicate(DriverInputDto inputDto)
        {
            List<Account> accounts = await _context.Accounts
                .Where(account => account.Code == inputDto.Code || account.Email == inputDto.Email)
                .Select(account => new Account { Email = account.Email, Code = account.Code })
                .ToListAsync();
            foreach (Account account in accounts)
            {
                if (account.Code == inputDto.Code)
                {
                    if (!errors.ContainsKey("Code"))
                    {
                        errors.Add("Code", "Code is unavailable");
                    }
                }
                if (account.Email == inputDto.Email)
                {
                    if (!errors.ContainsKey("Email"))
                    {
                        errors.Add("Email", "Email is unavailable");
                    }
                }
                if (errors.Count == 2)
                {
                    break;
                }
            }

            List<Driver> drivers = await _context.Drivers
                .Where(driver => (inputDto.PersonalEmail != null ? driver.PersonalEmail == inputDto.PersonalEmail : true) ||
                                 (driver.PhoneNumber == inputDto.PhoneNumber) || (driver.IdCardNumber == inputDto.IdCardNumber))
                .Select(driver => new Driver { PersonalEmail = driver.PersonalEmail, PhoneNumber = driver.PhoneNumber, IdCardNumber = driver.IdCardNumber })
                .ToListAsync();
            foreach (Driver driver in drivers)
            {
                if (driver.PersonalEmail == inputDto.PersonalEmail && driver.PersonalEmail != null)
                {
                    if (!errors.ContainsKey("PersonalEmail"))
                    {
                        errors.Add("PersonalEmail", "PersonalEmail is unavailable");
                    }
                }
                if (driver.PhoneNumber == inputDto.PhoneNumber)
                {
                    if (!errors.ContainsKey("PhoneNumber"))
                    {
                        errors.Add("PhoneNumber", "PhoneNumber is unavailable");
                    }
                }
                if (driver.IdCardNumber == inputDto.IdCardNumber)
                {
                    if (!errors.ContainsKey("idCardNumber"))
                    {
                        errors.Add("IdCardNumber", "IdCardNumber is unavailable");
                    }
                }
                if (errors.Count == 3)
                {
                    break;
                }
            }
        }

        private async Task CheckUpdateDuplicate(Driver currentDriver, DriverInputDto inputDto)
        {
            List<Account> accounts = await _context.Accounts
                .Where(account => account.Id != currentDriver.AccountId)
                .Where(account => account.Code == inputDto.Code)
                .Select(account => new Account { Code = account.Code })
                .ToListAsync();
            foreach (Account account in accounts)
            {
                if (account.Code == inputDto.Code)
                {
                    if (!errors.ContainsKey("code"))
                    {
                        errors.Add("code", "Code is unavailable");
                    }
                }
                if (errors.Count == 2)
                {
                    break;
                }
            }

            List<Driver> drivers = await _context.Drivers
                .Where(driver => driver.Id != currentDriver.Id)
                .Where(driver => (inputDto.PersonalEmail != null ? driver.PersonalEmail == inputDto.PersonalEmail : true) ||
                                 (driver.PhoneNumber == inputDto.PhoneNumber) || (driver.IdCardNumber == inputDto.IdCardNumber))
                .Select(driver => new Driver { PersonalEmail = driver.PersonalEmail, PhoneNumber = driver.PhoneNumber, IdCardNumber = driver.IdCardNumber })
                .ToListAsync();
            foreach (Driver driver in drivers)
            {
                if (driver.PersonalEmail == inputDto.PersonalEmail && driver.PersonalEmail != null)
                {
                    if (!errors.ContainsKey("personalEmail"))
                    {
                        errors.Add("personalEmail", "PersonalEmail is unavailable");
                    }
                }
                if (driver.PhoneNumber == inputDto.PhoneNumber)
                {
                    if (!errors.ContainsKey("phoneNumber"))
                    {
                        errors.Add("phoneNumber", "PhoneNumber is unavailable");
                    }
                }
                if (driver.IdCardNumber == inputDto.IdCardNumber)
                {
                    if (!errors.ContainsKey("idCardNumber"))
                    {
                        errors.Add("idCardNumber", "IdCardNumber is unavailable");
                    }
                }
                if (errors.Count == 3)
                {
                    break;
                }
            }
        }
    }
}
