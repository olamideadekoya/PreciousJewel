using System.Text.Json.Serialization;

namespace WebApiLearning.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public virtual List<Product> Products { get; set; }
    }
}
