using MongoDB.Bson;

namespace AppMongoDbExemplo.Data
{
    public class Entity
    {
        public ObjectId Id { get; set; }

        public Entity()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
