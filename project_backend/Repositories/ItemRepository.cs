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

        public int GetItemQuantityInStore(string itemName)
        {
            string sql = $"SELECT quantity FROM items WHERE item_name = @Item_Name";
            var queryArguments = new
            {
                Item_Name = itemName
            };
            int quantityInStore = _connection.QuerySingleOrDefault<int>(sql, queryArguments, null);
            return quantityInStore;
        }

        public decimal GetTotalItemPrice(string itemName, int quantityToBuy)
        {
            string sql = $"SELECT price FROM items WHERE item_name = @Item_Name";
            var queryArguments = new
            {
                Item_Name = itemName,
            };
            decimal totalPrice = quantityToBuy*(_connection.QuerySingleOrDefault<decimal>(sql, queryArguments, null));
            return totalPrice;
        }

       

        public void DeleteItem(string itemName)
        {
            string sql = $"UPDATE items SET quantity = 0, is_deleted = 'true' WHERE item_name = @Item_Name";
            var queryArguments = new
            {
                Item_Name = itemName
            };
            _connection.Execute(sql, queryArguments);
        }

        public void UpdateItemQuantity(string itemName, int reducedQuantity)
        {
            string sql = $"UPDATE items SET quantity = @ReducedQuantity WHERE item_name = @Item_Name";
            var queryArguments = new
            {
                ReducedQuantity = reducedQuantity,
                Item_Name = itemName
            };
            _connection.Execute(sql, queryArguments);
        }
        public int AddNewItem(int id, string name, decimal price, int amount)
        {
            string sql = $"INSERT INTO items (item_id, item_name, price, amount) VALUES (@id, @name, @price, @amount)";
            var queryArguments = new
            {
                item_name = name,
                item_id = id,
                price = price,
                amount = amount
            };
            return _connection.Execute(sql, queryArguments);
        }
        public int UpdateItem(int id, string name, decimal price, int amount)
        {
            string sql = $"UPDATE items SET item_id = @id WHERE  ";
            return 0;
        }
    }
}
