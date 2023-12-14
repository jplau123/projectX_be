using project_backend.Interfaces;

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
    }
}
