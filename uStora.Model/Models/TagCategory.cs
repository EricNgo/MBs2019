using uStora.Model.Abstracts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace uStora.Model.Models
{
    [Table("TagCategories")]
    public class TagCategory 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

 
        public virtual IEnumerable<Tag> Tags { get; set; }


    }
}