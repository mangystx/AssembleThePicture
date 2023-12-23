using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using AssembleThePicture.Extensions;
using AssembleThePicture.Models;
using AssembleThePicture.Models.DataBase;
using AssembleThePicture.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Rectangle = SixLabors.ImageSharp.Rectangle;

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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewScore()
        {
            return Ok();
        }
    }
}