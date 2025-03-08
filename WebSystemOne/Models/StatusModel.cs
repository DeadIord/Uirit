namespace WebSystemOne.Models
{
    public class StatusModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }

        public ICollection<AplicationModel> Aplications { get; set; }
    }
}
