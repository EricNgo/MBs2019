using uStora.Model.Abstracts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace uStora.Web.Models
{
    public class TagCategoryViewModel
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }



        //public virtual IEnumerable<PostTagViewModel> PostTags { get; set; }
    }
}