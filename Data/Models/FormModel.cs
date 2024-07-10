using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUD_API.Data.Models
{
    [Index(nameof(Title), IsUnique = true)]
    public class FormModel
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Title required")]
        public required string Title { get; set; }
        [DisplayFormat(NullDisplayText = "No description provided")]
        public string? Description { get; set; }
        
        private DateTime? _createdDate;
        public DateTime? CreatedTime 
        { 
            get 
            { 
                return _createdDate;
            } 
            set 
            {
                _createdDate = value ?? DateTime.Now;
            } 
        }
        private DateTime? _updatedDate;
        public DateTime? UpdatedTime
        {
            get
            {
                return _updatedDate;
            }
            set
            {
                _updatedDate = value ?? DateTime.Now;
            }
        }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public CustomIdentityUser? User { get; set; }
        public ICollection<FormDataModel>? FormData { get; set; }
    }
}
