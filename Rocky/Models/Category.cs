using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rocky.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Name of Category")]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage ="Must be more than 0")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
