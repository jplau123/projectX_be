using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IAdminService
    {
        Task<List<User>>? GetUsersAsync();

        Task<int> DeleteUserByUserIdAsync(int id);
    }
}
