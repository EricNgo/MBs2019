using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uStora.Model.Models
{
    [Table("Manufactors")]
    public class Manufactor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
