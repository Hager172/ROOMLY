using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using ROOMLY.DTOs.AccountDTO;
using WebApplication1.models;
using System.Web;
using System.Text.Encodings.Web;
using Castle.Core.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using ROOMLY.DTOs.email;

namespace ROOMLY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly DTOs.email.IEmailSender emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ROOMLY.DTOs.email.IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.emailSender = emailSender;
        }


        #region register

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Regester(RegesterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }


            var exiextUser = await userManager.FindByEmailAsync(model.Email);
            if (exiextUser != null) {

                return BadRequest("user already exists with this email");

            }
            var user = new ApplicationUser()
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
                Address = model.Address


            };


            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) { return BadRequest(result.Errors); }

            await userManager.AddToRoleAsync(user, "User");

            var userdto = new UserDTO()
            {
                Email = model.Email,
                FullName = model.FullName,
                Token =await GenerateJwtToken(user,userManager)
            };

            return Ok(userdto);

        }
        #endregion

        #region login

        [HttpPost("login")]


        public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            { return Unauthorized("Invalid email.");
            }


            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid email or password.");
            }
            var userdto = new UserDTO()
            {
                Email = user.Email,
                FullName = user.FullName,
                Token = await GenerateJwtToken(user,userManager)
            };
            return Ok(userdto);



        }
        #endregion
        #region Generate JWT Token
        private async Task<string> GenerateJwtToken(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.FullName)
    };

            // جلب رولات المستخدم
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }




        #endregion



        #region reset password

        [HttpPost("SendResetPasswordEmail")]
        public async Task<IActionResult> SendResetPasswordEmail([FromBody] EmailDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // لتجنب كشف أن الإيميل غير موجود
                return Ok("If a user with that email exists, a reset email has been sent.");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = HttpUtility.UrlEncode(token);

            // رابط إعادة التعيين، ممكن يكون رابط موقعك مع الباث الخاص بالريست
            var resetLink = $"https://localhost:5001/api/yourcontroller/resetpassword?email={user.Email}&token={encodedToken}";

            //  var resetLink = $"https://yourfrontend.com/resetpassword?email={user.Email}&token={encodedToken}";

            // ترسل الإيميل (انت محتاجة خدمة لإرسال الإيميلات)
            await emailSender.SendEmailAsync(user.Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{HtmlEncoder.Default.Encode(resetLink)}'>link</a>");

            return Ok("If a user with that email exists, a reset email has been sent.");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Invalid Request");

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password reset successful");
        }

    }

    public class EmailDto
    {
        public string Email { get; set; }
    }

    #endregion


}
