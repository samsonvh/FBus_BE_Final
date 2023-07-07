using AutoMapper;
using FBus_BE.DTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Enums;
using FBus_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FBus_BE.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Account, object>>> _orderDict;

        public AccountService(FbusMainContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Account, object>>>
            {
                {"id", account => account.Id},
            };
        }

        public Task<bool> ChangeStatus(int id, string status)
        {
            throw new NotImplementedException();
        }

        public Task<AccountDto> Create(int createdById, AccountDto inputDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountDto> GetDetails(int id)
        {
            Account? account = await _context.Accounts.FirstOrDefaultAsync(account => account.Id == id && account.Status != (byte)AccountStatusEnum.Deleted);
            return _mapper.Map<AccountDto>(account);
        }

        public async Task<DefaultPageResponse<AccountDto>> GetList(AccountPageRequest pageRequest)
        {
            DefaultPageResponse<AccountDto> pageResponse = new DefaultPageResponse<AccountDto>();
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
            int skippedCount = (int)((pageRequest.PageIndex - 1) * pageRequest.PageSize);
            List<AccountDto> accounts = new List<AccountDto>();
            int totalCount = await _context.Accounts
                .Where(account => !account.Status.Equals((int)AccountStatusEnum.Deleted))
                .Where(account => pageRequest.Code != null && pageRequest.Email != null
                                  ? account.Code.Contains(pageRequest.Code) || account.Email.Contains(pageRequest.Email)
                                  : pageRequest.Code != null && pageRequest.Email == null
                                    ? account.Code.Contains(pageRequest.Code)
                                    : pageRequest.Code == null && pageRequest.Email != null
                                      ? account.Email.Contains(pageRequest.Email)
                                      : true)
                .CountAsync();
            if (totalCount > 0 && skippedCount < totalCount)
            {
                accounts = pageRequest.Direction == "desc"
                    ? await _context.Accounts.OrderByDescending(_orderDict[pageRequest.OrderBy.ToLower()])
                                             .Skip(skippedCount)
                                             .Where(account => account.Status.Equals((int)AccountStatusEnum.Deleted))
                                             .Where(account => pageRequest.Code != null && pageRequest.Email != null
                                                               ? account.Code.Contains(pageRequest.Code) || account.Email.Contains(pageRequest.Email)
                                                               : pageRequest.Code != null && pageRequest.Email == null
                                                                 ? account.Code.Contains(pageRequest.Code)
                                                                 : pageRequest.Code == null && pageRequest.Email != null
                                                                   ? account.Email.Contains(pageRequest.Email)
                                                                   : true)
                                             .Select(account => _mapper.Map<AccountDto>(account))
                                             .ToListAsync()
                    : await _context.Accounts.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                                             .Skip(skippedCount)
                                             .Where(account => account.Status.Equals((int)AccountStatusEnum.Deleted))
                                             .Where(account => pageRequest.Code != null && pageRequest.Email != null
                                                               ? account.Code.Contains(pageRequest.Code) || account.Email.Contains(pageRequest.Email)
                                                               : pageRequest.Code != null && pageRequest.Email == null
                                                                 ? account.Code.Contains(pageRequest.Code)
                                                                 : pageRequest.Code == null && pageRequest.Email != null
                                                                   ? account.Email.Contains(pageRequest.Email)
                                                                   : true)
                                             .Select(account => _mapper.Map<AccountDto>(account))
                                             .ToListAsync();
            }
            pageResponse.Data = accounts;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }

        public Task<AccountDto> Update(int createdById, AccountDto inputDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
