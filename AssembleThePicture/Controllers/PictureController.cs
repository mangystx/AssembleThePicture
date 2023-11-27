using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssembleThePicture.Models.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AssembleThePicture.Controllers
{
    public class PictureController : Controller
    {
        private readonly ILogger<PictureController> _logger;
        
        private readonly MongoClient _mongoClient;

        private readonly IMongoDatabase _db;
    
        public PictureController(MongoClient client, ILogger<PictureController> logger)
        {
            _mongoClient = client;
            _db = client.GetDatabase("AtpDb");
            _logger = logger;
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddImage()
        {
            var file = Request.Form.Files[0];
            if (file != null && file.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] byteArray = memoryStream.ToArray();

                var picture = new Picture
                {
                    UserName = User.Identity.Name,
                    ImageData = byteArray,
                    BestAttempts = new List<Attempt>()
                };

                await _db.GetCollection<Picture>("pictures").InsertOneAsync(picture);

                return Json(new { success = true, message = "Image uploaded successfully" });
            }

            return Json(new { success = false, message = "No file or empty file received" });
        }
    }
}