using CRUD_API.Data;
using CRUD_API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;


namespace CRUD_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        private readonly ApplicationDataContext _context;
        private string _userId;
        public FormController(ApplicationDataContext applicationDataContext, IHttpContextAccessor httpContextAccessor) 
        {
            _context = applicationDataContext;
            //_userId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "no user id";
            _userId = "57bd4ad7-4d25-412f-9df7-26a6388875ea";
        }
        // GET: api/<FormController>
        [HttpGet]
        public async Task<string> Get()
        {

            var formList = await _context.FormModels!
                .GroupJoin(_context.FormDataModels!,
                form => form.Id,
                formData => formData.FormId,
                (form, formData) => new
                {
                    Id = form.Id,
                    UserId = form.UserId,
                    Title = form.Title,
                    Description = form.Description,
                    CreatedTime = form.CreatedTime,
                    UpdatedTime = form.UpdatedTime,
                    FormData = formData.Select(fd => new { Id = fd.Id, Name = fd.Name, Value = fd.Value, FormId = fd.FormId })
                })
                .Where(f => f.UserId == _userId)
                .ToListAsync();

            return JsonSerializer.Serialize(formList);
        }

        // GET api/<FormController>/5
        [HttpGet("{id}")]
        public async Task<string> Get(Guid id)
        {
            var formList = await _context.FormModels!
                .GroupJoin(_context.FormDataModels!,
                form => form.Id,
                formData => formData.FormId,
                (form, formData) => new
                {
                    Id = form.Id,
                    UserId = form.UserId,
                    Title = form.Title,
                    Description = form.Description,
                    CreatedTime = form.CreatedTime,
                    UpdatedTime = form.UpdatedTime,
                    FormData = formData.Select(fd => new { Id = fd.Id, Name = fd.Name, Value = fd.Value, FormId = fd.FormId })
                })
                .FirstOrDefaultAsync(f => f.UserId == _userId && f.Id == id);

            return JsonSerializer.Serialize(formList);
        }

        // POST api/<FormController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FormViewModel viewModel)
        {
            if (viewModel.Title.IsNullOrEmpty())
            {
                return ValidationProblem("Title required");
            }
            if (viewModel.Description?.Length > 500)
            {
                return ValidationProblem("Description is too long");
            }
            if (viewModel.DataValues?.Count() > 10)
            {
                return ValidationProblem("You can provide up to ten (10) viewModel data fields");
            }

            try
            {

                //var aspNetUser = await _context.Users.FirstAsync(u => u.Id == _userId);

                var formModel = new FormModel
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    //UserId = aspNetUser.Id,
                    UserId = _userId,
                };

                await _context.FormModels!.AddAsync(formModel);

                int fromModelAdded = await _context.SaveChangesAsync();

                if (fromModelAdded > 0)
                {
                    if (viewModel.DataValues?.Count() > 0)
                    {
                        List<FormDataModel> formDataModel = new List<FormDataModel>();

                        foreach (var item in viewModel.DataValues)
                        {
                            formDataModel.Add(new FormDataModel
                            {
                                Name = item.Key,
                                Value = item.Value,
                                FormId = formModel.Id,
                            });
                        }

                        await _context.AddRangeAsync(formDataModel);

                        int formDataModelAdded = await _context.SaveChangesAsync();

                        if (formDataModelAdded > 0)
                        {
                            return Ok("Form added");
                        }
                        else 
                        {
                            return BadRequest("Problem adding formData");
                        }
                    }
                }
                else 
                {
                    return BadRequest("Problem adding form");
                }
                return Ok($"Form added");
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<FormController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] FormViewModel viewModel)
        {
            try
            {
                var form = await _context.FormModels!
                    .GroupJoin(_context.FormDataModels!,
                    form => form.Id,
                    formData => formData.FormId,
                    (form, formData) => new
                    {
                        Id = form.Id,
                        UserId = form.UserId,
                        Title = form.Title,
                        Description = form.Description,
                        CreatedTime = form.CreatedTime,
                        UpdatedTime = form.UpdatedTime,
                        FormData = formData.Select(fd => new { Id = fd.Id, Name = fd.Name, Value = fd.Value, FormId = fd.FormId })
                    })
                    .FirstOrDefaultAsync(f => f.UserId == _userId && f.Id == id);

                if (form == null || viewModel == null)
                {
                    return BadRequest("Problem updating the form");
                }

                var formDataList = await _context.FormDataModels!.Where(fd => fd.FormId == form.Id).ToListAsync();


                if (viewModel.DataValues!.Count() + formDataList.Count() > 10)
                {
                    return BadRequest($"You tried to enter {viewModel.DataValues!.Count() + formDataList.Count()} entries, but you can enter up to ten (10)");
                }

                // TODO check viewModel.DataValues.Keys for duplicates

                List<FormDataModel> dataListToUpdate = new List<FormDataModel>();

                List<FormDataModel> dataListToAdd = new List<FormDataModel>();

                if (viewModel.DataValues!.Count() > 0)
                {



                    foreach (var data in formDataList)
                    {
                        foreach (var item in viewModel.DataValues!)
                        {
                            Console.WriteLine($"{(dataListToAdd.Where(v => v.Name == item.Key).Count() == 0) && formDataList.All(v => v.Name != item.Key)} => {item.Key}");
                            if (data.Name == item.Key)
                            {
                                dataListToUpdate.Add(new FormDataModel
                                {
                                    Id = data.Id,
                                    Name = item.Key,
                                    Value = item.Value,
                                    FormId = data.FormId,
                                });
                            }
                            else if ((dataListToAdd.Where(v => v.Name == item.Key).Count() == 0) && formDataList.All(v => v.Name != item.Key))
                            {
                                dataListToAdd.Add(new FormDataModel
                                {
                                    Name = item.Key,
                                    Value = item.Value,
                                    FormId = form.Id,
                                });
                            }
                        }
                    }

                }

                FormModel newForm = new FormModel
                {
                    Id = form.Id,
                    UserId = form.UserId,
                    Title = form.Title != viewModel.Title ? viewModel.Title : form.Title,
                    Description = form.Description != viewModel.Description ? viewModel.Description : form.Description,
                    CreatedTime = form.CreatedTime,
                    UpdatedTime = DateTime.Now,
                };

                _context.FormModels!.Update(newForm);

                _context.SaveChanges();

                _context.ChangeTracker.Clear();

                _context.FormDataModels!.UpdateRange(dataListToUpdate);

                _context.SaveChanges();

                await _context.FormDataModels.AddRangeAsync(dataListToAdd);

                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex) 
            {
                return BadRequest($"Cannot update, error: {ex.Message} {ex.StackTrace}");
            }
        }

        // DELETE api/<FormController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, IEnumerable<Guid>? ids)
        {
            try
            {
                var formAccess = await _context.FormModels!.Where(f => f.Id == id && f.UserId == _userId).FirstOrDefaultAsync();

                if (formAccess == null)
                {
                    return Unauthorized("Access denied");
                }

                if (ids != null && ids!.Count() > 0)
                {
                    List<FormDataModel> formDataModels = await _context.FormDataModels!.Where(f => ids!.Any(v => v == f.Id)).ToListAsync();

                    if (formDataModels != null)
                    {
                        _context.FormDataModels!.RemoveRange(formDataModels);

                        await _context.SaveChangesAsync();

                        return Ok("Form data deleted");
                    }
                }

                FormModel? form = await _context.FormModels!.Where(f => f.Id == id).FirstOrDefaultAsync();

                if (form != null)
                {
                    _context.FormModels!.Remove(form);

                    await _context.SaveChangesAsync();

                    return Ok("Form deleted");
                }

                return BadRequest("No Delete operation performed");
            }
            catch (Exception ex)
            {
                return BadRequest($"Cannot delete, error: {ex.Message} {ex.StackTrace}");
            }
        }
    }
}
