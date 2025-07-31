using Microsoft.Extensions.Options;
using MongoDB.Driver;
using testPortfolio.Models;

namespace testPortfolio.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _productsCollection;

        public ProductService(IOptions<MongoSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _productsCollection = database.GetCollection<Product>("Products");
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productsCollection.Find(product => true).ToListAsync();
        }

        public async Task<List<Product>> GetAllPublicAsync()
        {
            return await _productsCollection.Find(product => product.isPublic == true).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productsCollection.Find(product => product.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Product product)
        {
            await _productsCollection.InsertOneAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
            await _productsCollection.ReplaceOneAsync(filter, product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var result = await _productsCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<bool> IsProductExistsAsync(int id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var count = await _productsCollection.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}
