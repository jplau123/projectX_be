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

        public async Task<User> GetUserById(int user_id)
        {
            return await _userRepository.GetUserById(user_id) ?? throw new NotFoundException($"User with id '{user_id}' could not be found.");
        }
    }
}
