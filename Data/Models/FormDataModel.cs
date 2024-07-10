using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUD_API.Data.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class FormDataModel
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Value required")]
        public float Value { get; set; }

        public Guid FormId { get; set; }
        [ForeignKey("FormId")]
        public FormModel? Form { get; set; }
    }
}
