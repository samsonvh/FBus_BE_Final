using FBus_BE.DTOs.PageDTOs;

namespace FBus_BE.Services
{
    public interface IDefaultService<D,L,I,P>
    {
        Task<DefaultPageResponse<L>> GetList(P pageRequest);
        Task<D> GetDetails(int id);
        Task<D> Create(int createdById, I inputDto);
        Task<D> Update(int createdById, I inputDto, int id);
        Task<bool> ChangeStatus(int id, string status);
        Task<bool> Delete(int id);
    }
}
