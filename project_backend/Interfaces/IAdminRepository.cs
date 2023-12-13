using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IAdminRepository
    {
        List<User>? GetUsers();
    }
}
