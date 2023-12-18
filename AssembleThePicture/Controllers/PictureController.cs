using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AssembleThePicture.Models;
using AssembleThePicture.Models.DataBase;
using AssembleThePicture.Models.Requests;
using Microsoft.AspNetCore.Authorization;
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

        public List<Piece> Pieces { get; set; }

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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Puzzle([FromBody] string pictureId)
        {
            Pieces = new List<Piece>();
            const int rows = 4;
            const int cols = 4;

            if (pictureId == null) throw new ArgumentNullException();
            
            ObjectId objectId = ObjectId.Parse(pictureId);
          
            byte[] imageData = _db.GetCollection<Picture>("pictures").Find(p => p.Id == objectId)
                .ToList()[0].ImageData;
            
            using var stream = new MemoryStream(imageData);
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream);
            int pieceWidth = image.Width / 4;
            int pieceHeight = image.Height / 4;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var pieceImage = image.Clone(x => x
                        .Crop(new Rectangle(col * pieceWidth, row * pieceHeight, pieceWidth, pieceHeight)));

                    using var memoryStream = new MemoryStream();
                    await pieceImage.SaveAsync(memoryStream, new JpegEncoder());
                    var pieceImageData = memoryStream.ToArray();

                    var piece = new Piece
                    {
                        ImageData = pieceImageData,
                        CurrentRow = row,
                        CurrentCol = col,
                        RightRow = row,
                        RightCol = col
                    };

                    Pieces.Add(piece);
                }
            }

            var random = new Random();
            int n = Pieces.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (Pieces[k].CurrentCol, Pieces[k].CurrentRow, Pieces[n].CurrentCol, Pieces[n].CurrentRow) =
                    (Pieces[n].CurrentCol, Pieces[n].CurrentRow, Pieces[k].CurrentCol, Pieces[k].CurrentRow);
            }
            
            return View(Pieces);
        }
    }
}