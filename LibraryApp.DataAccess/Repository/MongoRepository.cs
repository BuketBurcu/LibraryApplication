using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static LibraryApp.DataAccess.IRepository;

namespace LibraryApp.DataAccess
{
    public class MongoRepository<T> : IRepository<T>, IDisposable where T : class
    {
        private IMongoDatabase _database;
        private IMongoCollection<T> _collection;
        private MongoClient _client;

        public MongoRepository()
        {
            GetDatabase();
            GetCollection();
        }

        private void GetCollection()
        {
            if (_database.GetCollection<T>(typeof(T).Name) == null)
                _database.CreateCollection(typeof(T).Name);
            _collection = _database.GetCollection<T>(typeof(T).Name);
        }

        private void GetDatabase()
        {
            _client = new MongoClient(Environment.GetEnvironmentVariable("MONGO_URI"));
            _database = _client.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DB"));
        }

        public void Add(T entity)
        {
            this._collection.InsertOne(entity);
        }

        public void Delete(Expression<Func<T, bool>> predicate, bool forceDelete = false)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Where(predicate);
            this._collection.DeleteMany(filter);
        }

        public void Update(Expression<Func<T, bool>> predicate, T entity)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Where(predicate);
            this._collection.ReplaceOneAsync(filter, entity);
        }

        public List<T> GetAll()
        {
            return _collection.AsQueryable().ToList();
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Where(predicate);
            return this._collection.Find(filter).FirstOrDefault();
        }

        public void Dispose()
        {

        }
    }
}
