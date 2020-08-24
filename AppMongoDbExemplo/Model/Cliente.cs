using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AppMongoDbExemplo.Model
{
    public class Cliente
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Idade { get; set; }
        public decimal Renda { get; set; }
        public bool Ativo { get; set; }
    }
}
