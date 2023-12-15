using Dapper;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using System.Data;

namespace project_backend.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly IDbConnection _connection;
        public ItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public IEnumerable<Item> GetItems()
        {
            return _connection.Query<Item>("SELECT * FROM items");
        }

        public int GetItemAmountInStore(string item_name)
        {
            string sql = $"SELECT quantity FROM items WHERE item_name = @Item_Name";
            var queryArguments = new
            {
                Item_Name = item_name
            };
            int quantityInStore = _connection.QuerySingleOrDefault<int>(sql, queryArguments, null);
            return quantityInStore;
        }

        public int GetTotalItemPrice(string item_name, int quantityToBuy)
        {
            string sql = $"SELECT price FROM items WHERE item_name = @Item_Name";
            var queryArguments = new
            {
                Item_Name = item_name,
            };
            int totalPrice = quantityToBuy*(_connection.QuerySingleOrDefault<int>(sql, queryArguments, null));
            return totalPrice;
        }

       

        public void DeleteItem(string item_name)
        {
            string sql = $"UPDATE items SET quantity = 0, is_deleted = 'true' WHERE item_name = @Item_Name";
            var queryArguments = new
            {
                Item_Name = item_name
            };
            _connection.Execute(sql, queryArguments);
        }

        public void ReduceItemQuantity(string item_name, int reducedQuantity)
        {
            string sql = $"UPDATE items SET quantity = @ReducedQuantity WHERE item_name = @Item_Name";
            var queryArguments = new
            {
                ReducedQuantity = reducedQuantity,
                Item_Name = item_name
            };
            _connection.Execute(sql, queryArguments);
        }
    }
}
