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
    }
}
