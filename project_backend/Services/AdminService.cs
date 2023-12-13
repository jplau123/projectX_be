using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Services
{
    public class AdminService(IAdminRepository adminRepository) : IAdminService
    {
        private readonly IAdminRepository _adminRepository = adminRepository;

        public List<User>? GetUsers()
        {
            var departmentList = _adminRepository.GetUsers();

            if (departmentList.Count == 0)
            {
                return null;
            }

            return departmentList;
        }
    }
}
