using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssembleThePicture.Models.DataBase;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("Name")] 
    public string Name { get; set; }
    
    [BsonElement("Password")] 
    public string Password { get; set; }
}