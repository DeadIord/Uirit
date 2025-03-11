using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models
{
    public class ApplicationModel
    {
        [Key]
        public int Id { get; set; }
        public int ServiceNumber { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string Body { get; set; }

        public int StatusId { get; set; }
        public StatusModel Status { get; set; }

        public bool Check { get; set; }
        public string FIO {  get; set; }

    }
}
