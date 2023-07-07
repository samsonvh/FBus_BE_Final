using FBus_BE.DTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Models;

namespace FBus_BE.Services
{
    public interface IAccountService : IDefaultService<AccountDto, AccountDto, AccountDto, AccountPageRequest>
    {
    }
}
