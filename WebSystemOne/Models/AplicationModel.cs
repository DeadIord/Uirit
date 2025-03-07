using System.ComponentModel.DataAnnotations;

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
        public ServiceModel Service { get; set; }

        public int StatusId { get; set; }
        public StatusModel Status { get; set; }

        public string UserId { get; set; }
        public ApplicationUserModel User { get; set; }

    }
}
