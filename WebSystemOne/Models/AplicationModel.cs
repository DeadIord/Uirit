using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSystemOne.Models
{
    public class AplicationModel
    {
        [Key]
        public int Id { get; set; }
        public int ServiceNumber { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string Body { get; set; }


        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public ServiceModel Service { get; set; }

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public StatusModel Status { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUserModel User { get; set; }


    }
}
