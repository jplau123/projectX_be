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
        private readonly IItemRepository _itemRepository;

        public UserService(IUserRepository userRepository, IItemRepository itemRepository)
        {
            _userRepository = userRepository;
            _itemRepository = itemRepository;
        }
        public decimal AddUserBalance(int userId, decimal balance)
        {
            decimal currentBalance = _userRepository.GetUserBalance(userId);
            decimal balanceAfterTopUp = currentBalance + balance;
            return _userRepository.AddUserBalance(userId, balanceAfterTopUp);
        }

        public void PurchaseItem(int userId, string itemName, int quantityToBuy)
        {
            decimal userBalance = _userRepository.GetUserBalance(userId);
            int quantityInStore = _itemRepository.GetItemQuantityInStore(itemName);
            decimal totalPrice = _itemRepository.GetTotalItemPrice(itemName, quantityToBuy);

            decimal unitPrice = totalPrice / quantityToBuy;
            decimal reducedBalance = userBalance - totalPrice;
            int reducedQuantity = quantityInStore - quantityToBuy;

            if (quantityInStore < quantityToBuy)
            {
                throw new ExceededAmountException($"The requested amount of {itemName}s exceeds the amount in the store. There are only {quantityInStore} {itemName}s left.");
            }

            if (userBalance < totalPrice)
            {
                throw new ExceededPriceException($"The price of requested items (${totalPrice}) exceeds your balance. You only got ${userBalance}.");
            }

            _userRepository.UpdateUserBalance(userId, reducedBalance);

            _userRepository.AppendPurchaseHistory(userId, itemName, quantityToBuy, unitPrice);

            if (quantityInStore == quantityToBuy)
            {
                _itemRepository.DeleteItem(itemName);

                return;
            }

            _itemRepository.UpdateItemQuantity(itemName, reducedQuantity);
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
