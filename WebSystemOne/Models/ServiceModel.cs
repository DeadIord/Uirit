namespace WebSystemOne.Models
{
    public class ServiceModel
    {
        public int Id { get; set; }
        public string ServiceNumber { get; set; }

        public ICollection<AplicationModel> Aplications { get; set; }
    }
}
