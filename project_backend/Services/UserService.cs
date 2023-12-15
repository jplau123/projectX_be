using project_backend.Exceptions;
using project_backend.Interfaces;

namespace project_backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IItemRepository _itemRepository;
        
        public UserService(IUserRepository userRepository, IItemService itemService, IItemRepository itemRepository)
        {
            _userRepository = userRepository;
            _itemRepository = itemRepository;
        }
        public int AddUserBalance(int user_id, int balance)
        {
            int currentBalance = _userRepository.GetUserBalance(user_id);
            int balanceAfterTopUp = currentBalance + balance;
            return _userRepository.AddUserBalance(user_id, balanceAfterTopUp);
        }

        public void PurchaseItem(int userId, string itemName, int quantityToBuy)
        {
            int userBalance = _userRepository.GetUserBalance(userId);
            int quantityInStore = _itemRepository.GetItemAmountInStore(itemName);
            int totalPrice = _itemRepository.GetTotalItemPrice(itemName, quantityToBuy);
            
            int unitPrice = totalPrice / quantityToBuy;
            int reducedBalance = userBalance - totalPrice;
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
    }
}
