using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssembleThePicture.Models.DataBase;

public class Picture
{
    [BsonId] 
    public ObjectId Id { get; set; }
    
    public string UserName { get; set; }
    
    [BsonElement("ImageData")]
    public byte[] ImageData { get; set; }
}