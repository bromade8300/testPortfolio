using Microsoft.Extensions.Options;
using MongoDB.Driver;
using testPortfolio.Models;

namespace testPortfolio.Services
{
    public class PictureService
    {
        private readonly IMongoCollection<Picture> _picturesCollection;

        public PictureService(IOptions<MongoSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _picturesCollection = database.GetCollection<Picture>("Pictures");
        }

        public async Task<List<Picture>> GetAllAsync()
        {
            return await _picturesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Picture?> GetByIdAsync(int id)
        {
            return await _picturesCollection.Find(picture => picture.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Picture picture)
        {
            // Générer un ID unique (max + 1) pour éviter les conflits MongoDB
            var maxId = await _picturesCollection
                .Find(_ => true)
                .SortByDescending(p => p.Id)
                .Limit(1)
                .FirstOrDefaultAsync();

            picture.Id = (maxId?.Id ?? 0) + 1;

            await _picturesCollection.InsertOneAsync(picture);
        }

        public async Task UpdateAsync(Picture picture)
        {
            var filter = Builders<Picture>.Filter.Eq(p => p.Id, picture.Id);
            await _picturesCollection.ReplaceOneAsync(filter, picture);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var filter = Builders<Picture>.Filter.Eq(p => p.Id, id);
            var result = await _picturesCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}
