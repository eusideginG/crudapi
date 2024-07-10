
namespace CRUD_API.Data.Models
{
    public class FormViewModel
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, float>? DataValues { get; set; }
    }
}
