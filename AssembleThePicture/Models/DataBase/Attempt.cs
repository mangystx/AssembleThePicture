using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssembleThePicture.Models.DataBase;

public class Attempt
{
    [BsonElement("UserId")]
    public ObjectId UserId { get; set; }

    [BsonElement("Score")]
    public int Score { get; set; }
}