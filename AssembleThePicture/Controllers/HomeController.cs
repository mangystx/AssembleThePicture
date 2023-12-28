using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AssembleThePicture.Models;
using AssembleThePicture.Models.DataBase;
using AssembleThePicture.Models.ViewModels;
using MongoDB.Driver;
using AssembleThePicture.Models.ViewModels.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace AssembleThePicture.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    private readonly MongoClient _mongoClient;

    private readonly IMongoDatabase _db;
    
    public HomeController(MongoClient client, ILogger<HomeController> logger)
    {
        _mongoClient = client;
        _db = client.GetDatabase("AtpDb");
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.Pictures = _db.GetCollection<Picture>("pictures").Find(_ => true).ToList();
        if (ViewBag.Pictures == null || ViewBag.Pictures is List<Picture> { Count: 0 })
        { 
            _logger.LogWarning("ViewBag.Pictures is null");
        }

        ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
        
        return View();
    }
    
    [HttpGet]
    public IActionResult Login() => PartialView();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _db.GetCollection<User>("users").Find(u => u.Name == model.Name)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                ModelState.AddModelError("", "Such user does not exist");
                ViewBag.OpenLoginForm = true;
                ViewBag.Pictures = _db.GetCollection<Picture>("pictures").Find(_ => true).ToList();
                ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                
                return View("Index");
            } 
            
            if (user.Password != model.Password){
                ModelState.AddModelError("", "Wrong password");
                ViewBag.OpenLoginForm = true;
                ViewBag.Pictures = _db.GetCollection<Picture>("pictures").Find(_ => true).ToList();
                ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                
                return View("Index");
            }
            
            var claims = new List<Claim> { new(ClaimTypes.Name, user.Name) };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity), authProperties);
            ViewBag.Pictures = _db.GetCollection<Picture>("pictures").Find(_ => true).ToList();
            ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            
            return RedirectToAction("Index");
        }

        ViewBag.OpenLoginForm = true;
        ViewBag.Pictures = _db.GetCollection<Picture>("pictures").Find(_ => true).ToList();
        ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
        
        return View("Index");
    }

    [HttpGet]
    public IActionResult Register() => PartialView();
  
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _db.GetCollection<User>("users").Find(u => u.Name == model.Name)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                ModelState.AddModelError("", "User already exists");
                ViewBag.OpenRegisterForm = true;
                ViewBag.Pictures = _db.GetCollection<Picture>("pictures").Find(_ => true).ToList();
                ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                
                return View("Index");
            }

            user = new User { Name = model.Name, Password = model.Password};
            await _db.GetCollection<User>("users").InsertOneAsync(user);
            
            var claims = new List<Claim> { new(ClaimTypes.Name, user.Name) };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };
    
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity), authProperties);
            ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            
            return RedirectToAction("Index");
        }
    
        ViewBag.OpenRegisterForm = true;
        ViewBag.Pictures = _db.GetCollection<Picture>("pictures").Find(_ => true).ToList();
        ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
        
        return View("Index");
    }

    [HttpGet]
    public IActionResult IsAuthorized()
    {
        var context = ControllerContext.HttpContext;
        if (context.User.Identity != null && context.User.Identity.IsAuthenticated) return Ok();
        
        return Unauthorized();
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await ControllerContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        ViewBag.Pictures = _db.GetCollection<Picture>("pictures").Find(_ => true).ToList();
        ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
        
        return View("Index");
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
                UserName = User.Identity!.Name!,
                ImageData = byteArray,
            };

            await _db.GetCollection<Picture>("pictures").InsertOneAsync(picture);

            return Json(new { success = true, message = "Image uploaded successfully" });
        }

        return Json(new { success = false, message = "No file or empty file received" });
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}