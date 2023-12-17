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
            return _connection.Query<Item>("SELECT item_id, item_name, price, quantity, created_at, created_by, is_deleted FROM items WHERE is_deleted = false");
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
            decimal totalPrice = quantityToBuy * (_connection.QuerySingleOrDefault<decimal>(sql, queryArguments, null));
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

        public Task<int> AddNewItem(string name, decimal price, int quantity, string? created_by)
        {
            string sql = $"INSERT INTO items (item_name, price, quantity, created_by) VALUES (@name, @price, @quantity, @created_by) returning item_id";
            var queryArguments = new
            {
                name = name,
                price = price,
                quantity = quantity,
                created_by = created_by
            };
            return _connection.ExecuteScalarAsync<int>(sql, queryArguments);
        }

        public async Task<bool> UpdateItem(int id, string name, decimal price, int quantity)
        {
            string sql = $"UPDATE items SET item_name = @name, price = @price, quantity = @quantity WHERE item_id = @id";
            var queryArguments = new
            {
                name = name,
                id = id,
                price = price,
                quantity = quantity
            };
            return (await _connection.ExecuteAsync(sql, queryArguments)) > 0;
        }

        public async Task<bool> DeleteItem(int id)
        {
            string sqlSecond = $"UPDATE items SET is_deleted = true WHERE item_id = @id";
            var queryArgumentsSecond = new
            {
                id = id
            };
            var update = await _connection.ExecuteAsync(sqlSecond, queryArgumentsSecond);
            return update > 0;
        }

        public async Task<Item> GetItemById(int id)
        {
            string sql = $"SELECT item_id, item_name, price, quantity, created_at, created_by FROM items WHERE item_id = @id AND is_deleted = false";
            var queryArguments = new
            {
                id = id
            };
            return await _connection.QuerySingleAsync<Item>(sql, queryArguments);
        }

        public async Task<bool> CheckIfItemExists(string name)
        {
            string sql = $"SELECT COUNT(*) FROM items WHERE item_name = @name AND is_deleted = false";
            return await _connection.QueryFirstAsync<int>(sql, new {name = name}) > 0;
        }

        public async Task<bool> CheckIfItemExistsById(int id)
        {
            string sql = $"SELECT COUNT(*) FROM items WHERE item_id = @id AND is_deleted = false";
            return await _connection.QueryFirstAsync<int>(sql, new { id = id }) > 0;
        }
    }
}
