using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uStora.Model.Models
{
    [Table("Tags")]
    public class Tag
    {
        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string ID { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }


        [Required]
        public int TagCategoryID { get; set; }



        [MaxLength(250)]
        public string Type { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("TagCategoryID")]
        public TagCategory TagCategory { get; set; }

        public virtual IEnumerable<PostTag> PostTags { get; set; }
    }
}