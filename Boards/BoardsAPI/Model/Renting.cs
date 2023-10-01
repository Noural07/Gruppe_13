using System.ComponentModel.DataAnnotations;
namespace BoardsAPI.Model
{
    public class Renting
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please choese a startdate")]

        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Please choese a enddate")]
        public DateTime EndDate { get; set; }
    }
}
