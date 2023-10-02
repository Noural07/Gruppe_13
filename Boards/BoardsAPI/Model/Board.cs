using System.ComponentModel.DataAnnotations;

namespace BoardsAPI.Model
{
    public class Board
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Thickness { get; set; }
        public double Volume { get; set; }
        public string? Type { get; set; }
        public Decimal Price { get; set; }
        public string? Equipment { get; set; }
        public bool Reserved { get; set; }
        public string? Image { get; set; }
        [Required(ErrorMessage = "Please choese a startdate")]

        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Please choese a enddate")]
        public DateTime EndDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
