using System.ComponentModel.DataAnnotations;

namespace WebSystemTwo.Models
{
    public class StatusModel
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int StatusCode { get; set; }

        [StringLength(64)]
        public string StatusName { get; set; }

        public string Text { get; set; }
    }


}
