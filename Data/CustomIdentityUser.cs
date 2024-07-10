using CRUD_API.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CRUD_API.Data
{
    public class CustomIdentityUser : IdentityUser
    {
        public IEnumerable<FormModel>? FormModels { get; set; }
    }
}
