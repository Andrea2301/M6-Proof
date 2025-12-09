namespace TalentoPlus.Core.Entities
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // Navigation property
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
