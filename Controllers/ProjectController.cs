using BeltExam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProjectController : Controller
{
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

    private JumpStarterContext db;
    public ProjectController(JumpStarterContext context)
    {
        db = context;
    }

    [HttpGet("/projects/new")]
    public IActionResult New()
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "Users");
        }
        return View("New");
    }

    [HttpPost("/projects/create")]
    public IActionResult Create(Project newProject)
    {
        if (!loggedIn || uid == null)
        {
            return RedirectToAction("Index", "Users");
        }
        if( newProject.EndDate < DateTime.Now)
        {
            ModelState.AddModelError("EndDate", "must be in the future");
        }
        if (!ModelState.IsValid )
        {
            return New();
        }


        newProject.UserId = (int)uid;

        Console.WriteLine(newProject.ProjectId);

        db.Projects.Add(newProject);

        db.SaveChanges();
        Console.WriteLine(newProject.ProjectId);

        return RedirectToAction("All");


    }

    [HttpGet("/projects")]
    public IActionResult All()
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "Users");
        }


        List<Project> allProjects = db.Projects

            .Include(v => v.Creator)
            .Include(v => v.Funders)
            .ToList();
        ViewBag.name = HttpContext.Session.GetString("Name");

        return View("All", allProjects);
    }

    [HttpGet("/projects/{oneProjectId}")]
    public IActionResult GetOneProject(int oneProjectId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "Users");
        }
        Project? project = db.Projects
            .Include(w => w.Creator)
            .Include(w => w.Funders)
            .ThenInclude(a => a.User)
            .FirstOrDefault(v => v.ProjectId == oneProjectId);
       

        if (project == null)
        {
            return RedirectToAction("All");
        }
         ViewBag.GoalPercentage = project.GoalPercentage();
        return View("Details", project);
    }

    [HttpPost("/projects/{deletedProjectId}/delete")]
    public IActionResult DeleteProject(int deletedProjectId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "Users");
        }
        Project? project = db.Projects.FirstOrDefault(p => p.ProjectId == deletedProjectId);

        if (project != null && project.UserId == uid)
        {
            db.Projects.Remove(project);
            db.SaveChanges();
        }
        return RedirectToAction("All");
    }

    [HttpGet("/Projects/{projectId}/edit")]
    public IActionResult Edit(int projectId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "Users");
        }
        Project? project = db.Projects.FirstOrDefault(p => p.ProjectId == projectId);

        if (project == null || project.UserId != uid)
        {
            return RedirectToAction("All");
        }

        return View("Edit", project);
    }

    [HttpPost("/projects/{projectId}/update")]
    public IActionResult Update(Project editedProject, int projectId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "Users");
        }
        if (ModelState.IsValid == false)
        {
            return Edit(projectId);
        }

        Project? dbProject = db.Projects.FirstOrDefault(project => project.ProjectId == projectId);

        if (dbProject == null || dbProject.UserId != uid)
        {
            return RedirectToAction("All");
        }

        dbProject.Name = editedProject.Name;
        dbProject.Goal = editedProject.Goal;
        dbProject.EndDate = editedProject.EndDate;
        dbProject.Description = editedProject.Description;
        dbProject.UpdatedAt = DateTime.Now;

        db.Projects.Update(dbProject);
        db.SaveChanges();

        return RedirectToAction("GetOneProject", new { ProjectId = dbProject.ProjectId });
    }

    [HttpPost("/projects/{projectId}/fund")]
    public IActionResult Funder(int amount, int projectId)
    {
        if (!loggedIn || uid == null)
        {
            return RedirectToAction("Index", "Users");
        }


            Funder newFunder = new Funder()
            {
                UserId = (int)uid,
                ProjectId = projectId,
                Amount = amount
            };

            db.Funders.Add(newFunder);
            Project? dbProject = db.Projects.FirstOrDefault(project => project.ProjectId == projectId);
            if (dbProject != null)
            {
                dbProject.Funds += newFunder.Amount;
                db.Projects.Update(dbProject);
            }
        



        db.SaveChanges();
        return RedirectToAction("GetOneProject", new {oneProjectId = projectId });
    }
}