using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boards.Models
{
    public class Board
    {
        public int ID { get; set; }
        public string ?Name { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Thickness { get; set; }
        public double Volume { get; set; }
        public string ?Type { get; set; }
        [Required]
        [Display(Name = "Price (€)")]
        [Column(TypeName = "decimal(18,2)")]
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
