using Dapper;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using System.Data;
using System.Data.SqlTypes;

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
            return _connection.Query<Item>("SELECT * FROM items WHERE is_deleted = false");
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
        public Task<int> AddNewItem(int id, string name, decimal price, int quantity, string? created_by)
        {
            string sql = $"INSERT INTO items (item_id, item_name, price, quantity, created_by) VALUES (@id, @name, @price, @quantity, @created_by)";
            var queryArguments = new
            {
                item_name = name,
                item_id = id,
                price = price,
                quantity = quantity,
                created_by = created_by
            };
            return _connection.ExecuteAsync(sql, queryArguments);
        }
        public Task<int> UpdateItem(int id, string name, decimal price, int quantity)
        {
            string sql = $"UPDATE items SET item_name = @name, price = @price, quantity = @quantity WHERE item_id = @id";
            var queryArguments = new
            {
                item_name = name,
                item_id = id,
                price = price,
                quantity = quantity
            };
            return (_connection.ExecuteAsync(sql, queryArguments));
        }
        public int DeleteItem(int id) 
        {
            string sql = $"DELETE FROM items WHERE item_id = @id";
            var queryArguments = new
            {
                item_id = id
            };
            if (_connection.Execute(sql, queryArguments) == 0)
            {
                return 0;
            };

            string sqlSecond = $"UPDATE items SET is_deleted = true WHERE item_id = @id";
            var queryArgumentsSecond = new
            {
                item_id = id
            };
            var update = _connection.Execute(sqlSecond, queryArgumentsSecond);
            return update;
        }
    }
}
