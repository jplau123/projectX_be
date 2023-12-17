using project_backend.DTOs.RequestDTO;
using project_backend.DTOs.ResponseDTO;
using project_backend.Exceptions;
using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public int AddUserBalance(int user_id, int balance)
        {
            return _userRepository.AddUserBalance(user_id, balance);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var result = await _userRepository.GetUsersAsync();
            return result.ToList();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id) ?? throw new NotFoundException("User not found.");
        }

        public async Task<List<GetPurchaseResponse>> GetAllPurchaseHistoryAsync()
        {
            var purchaseHistory = await _userRepository.GetAllPurchaseHistoryAsync();
            return purchaseHistory.ToList();
        }

        public async Task DeleteUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id) ?? throw new NotFoundException("Could not find user with provided id.");

            if (user.Is_Deleted)
            {
                throw new AlreadySoftDeletedException("User is already deleted.");
            }

            await _userRepository.DeleteUserByIdAsync(id);
        }

        public async Task AddUserAsync(AddUserRequest request)
        {
            await _userRepository.AddUserAsync(request);
        }

        public async Task<User> UpdateUserByIdAsync(UpdateUserRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(request.User_Id) ?? throw new NotFoundException("Could not find user with provided id.");

            var updatedUserCount = await _userRepository.UpdateUserByIdAsync(request);

            if (updatedUserCount == 0 || user.Is_Deleted)
            {
                throw new NotFoundException("Could not find user with provided id.");
            }

            return user;
        }
    }
}
