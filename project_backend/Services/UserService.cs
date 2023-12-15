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
            return _userRepository.AddUserBalance(user_id, balance);
        }

        public void PurchaseItem(int user_id, string item_name, int quantityToBuy)
        {
            int userBalance = _userRepository.GetUserBalance(user_id);
            int quantityInStore = _itemRepository.GetItemAmountInStore(item_name);
            int totalPrice = _itemRepository.GetTotalItemPrice(item_name, quantityToBuy);
            int unitPrice = totalPrice / quantityToBuy;

            if (userBalance < totalPrice)
            {
                throw new ExceededPriceException($"The price of requested items (${totalPrice}) exceeds your balance. You only got ${userBalance}.");
            }

            if (quantityInStore < quantityToBuy)
            {
                throw new ExceededAmountException($"The requested amount of {item_name}s exceeds the amount in the store. There are only {quantityInStore} {item_name}s left.");
            }
            else if(quantityInStore == quantityToBuy)
            {
                _itemRepository.DeleteItem(item_name);
            }

            int reducedBalance = userBalance - totalPrice;

            _userRepository.ReduceUserBalance(user_id, reducedBalance);

            int reducedQuantity = quantityInStore - quantityToBuy;

            _itemRepository.ReduceItemQuantity(item_name, reducedQuantity);

            _userRepository.AppendPurchaseHistory(user_id, item_name, quantityToBuy, unitPrice);

            
        }
    }
}
