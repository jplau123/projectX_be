using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Services
{
    public class AdminService(IAdminRepository adminRepository) : IAdminService
    {
        private readonly IAdminRepository _adminRepository = adminRepository;

        public async Task<List<User>>? GetUsersAsync()
        {
            var departmentList = await _adminRepository.GetUsersAsync();
            return departmentList;
        }
    }
}
