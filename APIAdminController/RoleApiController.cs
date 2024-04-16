using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.APIAdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleApiController : Controller
    {
        private readonly QlkrContext _context;

        public RoleApiController(QlkrContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRole()
        {
            var Roles = await _context.Roles
                .ToListAsync();

            var RolessWithFullInfo = Roles.Select(s => new
            {
                s.RoleId,
                s.Name,
            }).ToList();

            return Ok(RolessWithFullInfo);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(Role createModel)
        {
            if (ModelState.IsValid)
            {
                var RoleExists = await _context.Roles.AnyAsync(b => b.Name == createModel.Name);
                if (RoleExists)
                {
                    return BadRequest(new { Message = "Role already exists." });
                }

                var newRole = new Role
                {
                    Name = createModel.Name,
                };

                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();

                var registrationSuccessResponse = new
                {
                    Message = "Role created successful",
                    RoleId = newRole.RoleId
                };
                return Ok(registrationSuccessResponse);
            }

            var invalidDataErrorResponse = new
            {
                Message = "Invalid Role data",
                Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            };
            return BadRequest(invalidDataErrorResponse);
        }

        [HttpPut("update/{roleId}")]
        public async Task<IActionResult> UpdateRole(int roleId, Role updateModel)
        {
            var Roles = await _context.Roles.FindAsync(roleId);
            if (Roles == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(updateModel.Name))
            {
                Roles.Name = updateModel.Name;
            }

            _context.Entry(Roles).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Roles updated successfully"
            };

            return Ok(updateSuccessResponse);
        }

        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            var deleteSuccessResponse = new
            {
                Message = "Role deleted successfully"
            };

            return Ok(deleteSuccessResponse);
        }
    }
}
