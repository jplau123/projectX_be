using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IAdminService
    {
        List<User>? GetUsers();
    }
}
