using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssembleThePicture.Models.DataBase;

public class Attempt
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("UserName")]
    public string UserName { get; set; }
    
    [BsonElement("PictureId")]
    public ObjectId PictureId { get; set; }

    [BsonElement("Score")]
    public int Score { get; set; }
}