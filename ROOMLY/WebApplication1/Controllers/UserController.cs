using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ROOMLY.DTOs.AccountDTO;
using ROOMLY.DTOs.user;
using ROOMLY.UnitOfwork;
using WebApplication1.models;

namespace ROOMLY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly UnitOfWork unitOf;

        public UserController(UserManager<ApplicationUser> userManager, IMapper mapper,UnitOfwork.UnitOfWork unitOf)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.unitOf = unitOf;
        }
        //[Authorize(Roles ="Admin")]
        #region Get All Users (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = userManager.Users.ToList();
            var userDtos = new List<UsersDTO>();

            foreach (var user in users)
            {
                var userDto = mapper.Map<UsersDTO>(user);
                var roles = await userManager.GetRolesAsync(user);
                userDto.Role = roles.FirstOrDefault(); // ✅ إضافة الدور
                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }
        #endregion

        #region Get User By Id (Admin or User himself)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            // السماح فقط للأدمن أو لنفس المستخدم
            if (User.IsInRole("Admin") || User.Identity?.Name == user.UserName)
            {
                var userDto = mapper.Map<UsersDTO>(user);
                var roles = await userManager.GetRolesAsync(user);
                userDto.Role = roles.FirstOrDefault(); // ✅ إضافة الدور

                return Ok(userDto);
            }

            return Forbid();
        }
        #endregion


        #region Get Current User Profile (User Only)
        [HttpGet("profile")]
        //[Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await userManager.FindByNameAsync(User.Identity?.Name);
            if (user == null) return NotFound("User not found");

            var userDto = mapper.Map<UsersDTO>(user);
            return Ok(userDto);
        }
        #endregion

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // فقط الأدمن يقدر يحدث
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UsersDTO updatedUser)
        {
            if (id != updatedUser.Id)
                return BadRequest("User ID mismatch");

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // تحديث الحقول الأساسية
            user.FullName = updatedUser.FullName;
            user.Address = updatedUser.Address;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.UserName = updatedUser.UserName;

            // تحديث البريد الإلكتروني مع التأكد من تحديث UserManager أيضاً
            var setEmailResult = await userManager.SetEmailAsync(user, updatedUser.Email);
            if (!setEmailResult.Succeeded)
                return BadRequest(setEmailResult.Errors);

            // تحديث اسم المستخدم لو محتاج (اختياري)
            var setUserNameResult = await userManager.SetUserNameAsync(user, updatedUser.UserName);
            if (!setUserNameResult.Succeeded)
                return BadRequest(setUserNameResult.Errors);

            // تحديث الرول:
            var currentRoles = await userManager.GetRolesAsync(user);
            // إزالة المستخدم من كل الرولات القديمة
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors);

            // إضافة الرول الجديد
            var addRoleResult = await userManager.AddToRoleAsync(user, updatedUser.Role);
            if (!addRoleResult.Succeeded)
                return BadRequest(addRoleResult.Errors);

            // تحديث بيانات المستخدم الأساسية
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return NoContent();
        }


        #region Delete User (Admin Only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            // التحقق من وجود حجوزات مرتبطة
            List<Reservation> userReservations = unitOf.reservationRepo.GetAll().Where(r => r.UserId == id).ToList();
            if (userReservations.Any())
            {
                return Conflict("Cannot delete user because they have existing reservations.");
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return NoContent();
        }
        #endregion

        #region regesrer receptioist
        //[Authorize(Roles = "Admin")]
        [HttpPost("RegisterReceptionist")]
        public async Task<IActionResult> RegisterReceptionist(RegesterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Automatically assign the user to the "Receptionist" role
            await userManager.AddToRoleAsync(user, "Receptionist");

            return Ok("Receptionist registered successfully.");
        }


        #endregion


    }
}
