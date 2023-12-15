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

        public async Task<List<User>>? GetUsersAsync()
        {
            var result = await _userRepository.GetUsersAsync();
            return result.ToList();
        }

        public async Task DeleteUserByUserIdAsync(int id)
        {
            var user = await _userRepository.GetUserByUserIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("Could not find user with provided id.");
            }

            if (user.Is_Deleted)
            {
                throw new AlreadySoftDeletedException("User is already deleted.");
            }

            await _userRepository.DeleteUserByUserIdAsync(id);
        }
    }
}
