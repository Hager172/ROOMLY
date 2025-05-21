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

        #region Get All Users (Admin Only)
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers()
        {
            var users = userManager.Users.ToList();
            var userDtos = mapper.Map<List<UsersDTO>>(users);
            return Ok(userDtos);
        }
        #endregion

        #region Get User By Id (Admin or User himself)
        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            // Allow only admin or the user himself
            if (User.IsInRole("Admin") || User.Identity?.Name == user.UserName)
            {
                var userDto = mapper.Map<UsersDTO>(user);
                return Ok(userDto);
            }

            return Forbid("You are not allowed to access this resource.");
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

        #region Update User (User himself or Admin for some fields)
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto userDto)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            // Allow only admin or the user himself
            //if (User.IsInRole("Admin") || User.Identity?.Name == user.UserName)
            //{
                user.FullName = userDto.FullName ?? user.FullName;
                user.Address = userDto.Address ?? user.Address;
                user.PhoneNumber = userDto.PhoneNumber ?? user.PhoneNumber;

                //if (User.IsInRole("Admin"))
                //{
                    user.Email = userDto.Email ?? user.Email;
                //}

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded) return BadRequest(result.Errors);

                return NoContent();
            //}

            //return Forbid("You are not allowed to access this resource.");
        }
        #endregion

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

        //#region Reset Password (For User)
        //[HttpPost("ResetPassword")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        //{
        //    var user = await userManager.FindByEmailAsync(model.Email);
        //    if (user == null) return BadRequest("Invalid Request");

        //    var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(result.Errors);
        //    }

        //    return Ok("Password reset successful");
        //}
        //#endregion
    }
}
