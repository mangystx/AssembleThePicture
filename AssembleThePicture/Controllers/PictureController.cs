using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using AssembleThePicture.Extensions;
using AssembleThePicture.Models;
using AssembleThePicture.Models.DataBase;
using AssembleThePicture.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Puzzle([FromBody] string pictureId)
        {
            var pieces = new List<Piece>();
            const int rows = 4;
            const int cols = 4;

            if (pictureId == null) throw new ArgumentNullException();
            
            var objectId = ObjectId.Parse(pictureId);
          
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

                    pieces.Add(piece);
                }
            }

            var random = new Random();
            int n = pieces.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (pieces[k].CurrentCol, pieces[k].CurrentRow, pieces[n].CurrentCol, pieces[n].CurrentRow) =
                    (pieces[n].CurrentCol, pieces[n].CurrentRow, pieces[k].CurrentCol, pieces[k].CurrentRow);
            }

            ViewBag.WholeImageData = imageData;
            
            var filter = Builders<Attempt>.Filter.Eq("PictureId", objectId);
            ViewBag.Attempts = _db.GetCollection<Attempt>("attempts").Find(filter)
                .SortByDescending(a => a.Score).Limit(5).ToList();
            
            HttpContext.Session.Set("Pieces", pieces);
            HttpContext.Session.Set("PictureId", pictureId);
            
            return View(pieces);
        }

        [HttpPost]
        [Authorize]
        public IActionResult MovePiece([FromBody] MovePieceRequest movePieceRequest)
        {
            var pieces = HttpContext.Session.Get<List<Piece>>("Pieces");

            var piece1 = pieces.First(p => p.CurrentCol == movePieceRequest.Piece1Col
                                                    && p.CurrentRow == movePieceRequest.Piece1Row);
            var piece2 = pieces.First(p => p.CurrentCol == movePieceRequest.Piece2Col 
                                           && p.CurrentRow == movePieceRequest.Piece2Row);
            
            piece1.CurrentCol = movePieceRequest.Piece2Col;
            piece1.CurrentRow = movePieceRequest.Piece2Row;
            piece2.CurrentCol = movePieceRequest.Piece1Col;
            piece2.CurrentRow = movePieceRequest.Piece1Row;
            
            HttpContext.Session.Remove("Pieces");
            HttpContext.Session.Set("Pieces", pieces);
                
            var response = pieces.All(p => p.IsOnRightPlace);
            
            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddNewScore([FromBody]int score)
        {
            try
            {
                var pictureId = ObjectId.Parse(HttpContext.Session.Get<string>("PictureId"));
                string userName = HttpContext.User.Identity!.Name!;
                var attempts = _db.GetCollection<Attempt>("attempts");

                var filter = Builders<Attempt>.Filter.Eq(a => a.UserName, userName)
                             & Builders<Attempt>.Filter.Eq(a => a.PictureId, pictureId);
                var existingAttempt = attempts.Find(filter).FirstOrDefault();

                if (existingAttempt == null || score > existingAttempt.Score)
                {
                    var update = Builders<Attempt>.Update.Set(a => a.Score, score)
                        .SetOnInsert(a => a.UserName, userName).SetOnInsert(a => a.PictureId, pictureId);
                    var options = new FindOneAndUpdateOptions<Attempt>
                        { IsUpsert = true, ReturnDocument = ReturnDocument.After };

                    attempts.FindOneAndUpdate(filter, update, options);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}