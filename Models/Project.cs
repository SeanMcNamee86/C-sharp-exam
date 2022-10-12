#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.ComponentModel.DataAnnotations;
namespace BeltExam.Models;
public class Project
{
    
    [Key] // Primary Key
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "is required")]
    [Display(Name = "Name")]
    [MinLength(2, ErrorMessage = "must be more than 2 characters.")]
    public string Name { get; set; }
    [Display(Name = "Goal")]
    [Range(0, Int32.MaxValue, ErrorMessage ="Goal must be a positive value!")]
    public int Goal { get; set; }
    [Display(Name = "Current Money Raised")]
    public int Funds { get; set; } = 0;
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }
    [MinLength(20, ErrorMessage = "Your Project Description must be 20 characters")]
    public string Description { get; set; }
    public int UserId { get; set; }

    public User? Creator { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<Funder> Funders { get; set; } = new List<Funder>();


    public int GoalPercentage()
    {
        int percent = this.Funds * 100 / this.Goal;
        return percent;
    }
}