using BeltExam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


public class UserController : Controller
{
    private JumpStarterContext db;
    public UserController(JumpStarterContext context)
    {
        db = context;
    }
        private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("UUID");
        }
    }
        private bool loggedIn
    {
        get
        {
            return uid != null;
        }
    }
    [HttpGet("/users/new")]
    public IActionResult New()
    {
        return View("NewUser");
    }

    [HttpPost("/users/create")]
    public IActionResult Create(User newUser)
    {
        if (!ModelState.IsValid)
        {

            return New();
        }
        Console.WriteLine(newUser.UserId);

        db.Users.Add(newUser);

        db.SaveChanges();
        Console.WriteLine(newUser.UserId);

        return RedirectToAction("All");

    }

    [HttpGet("/")]
public IActionResult Index()
    {
        if(loggedIn)
        {
            return RedirectToAction("All", "Project");
        }
        return View("Index");
    }


    // [HttpGet("/users/{oneUserId}")]
    // public IActionResult GetOneUser(int oneUserId)
    // {
    //     User? user = db.Users.FirstOrDefault(p => p.UserId == oneUserId);


    //     if (user == null)
    //     {
    //         return RedirectToAction("All");
    //     }

    //     return View("Details", user);
    // }

    // [HttpPost("/users/{deletedUserId}/delete")]
    // public IActionResult DeleteUser(int deletedUserId)
    // {
    //     User? user = db.Users.FirstOrDefault(p => p.UserId == deletedUserId);

    //     if (user != null)
    //     {
    //         db.Users.Remove(user);
    //         db.SaveChanges();
    //     }
    //     return RedirectToAction("All");
    // }

    // [HttpGet("/users/{userId}/edit")]
    // public IActionResult Edit(int userId)
    // {
    //     User? user = db.Users.FirstOrDefault(p => p.UserId == userId);

    //     if(user == null)
    //     {
    //         return RedirectToAction("All");
    //     }
    //     return View("Edit", user);
    // }

    // [HttpPost("/users/{userId}/update")]
    // public IActionResult Update(User editedUser, int userId)
    // {
    //     if (ModelState.IsValid == false)
    //     {
    //         return Edit(userId);
    //     }

    //     User? userInDb = db.Users.FirstOrDefault(user => user.UserId == userId);

    //     if (userInDb == null)
    //     {
    //         return RedirectToAction("All");
    //     }

    //     userInDb.FirstName = editedUser.FirstName;
    //     userInDb.LastName = editedUser.LastName;
    //     userInDb.Email = editedUser.Email;
    //     userInDb.Password = editedUser.Password;
    //     userInDb.UpdatedAt = DateTime.Now;

    //     db.Users.Update(userInDb);
    //     db.SaveChanges();


    //     return RedirectToAction("GetOneUser", new { oneUserId = userInDb.UserId });
    // }

    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (db.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "is taken");
            }
        }
        if (ModelState.IsValid == false)
        {
            return Index();
        }

        PasswordHasher<User> hashedPW = new PasswordHasher<User>();
        newUser.Password = hashedPW.HashPassword(newUser, newUser.Password);

        db.Users.Add(newUser);
        db.SaveChanges();

        HttpContext.Session.SetInt32("UUID", newUser.UserId);
        HttpContext.Session.SetString("Name", newUser.FirstName);
        return Index();
    }
    [HttpPost("/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if (ModelState.IsValid == false)
        {
            return Index();
        }

        User? userInDb = db.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail);

        if (userInDb == null)
        {
            ModelState.AddModelError("LoginEmail", "does not match any existing users");
            return Index();
        }


        PasswordHasher<LoginUser> hashedPW = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompareResult = hashedPW.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LoginPassword);


        if (pwCompareResult == 0)
        {
            ModelState.AddModelError("LoginPassword", "invalid password");
            return Index();
        }

        HttpContext.Session.SetInt32("UUID", userInDb.UserId);
        HttpContext.Session.SetString("Name", userInDb.FirstName);
        return Index();
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Index();
    }
}