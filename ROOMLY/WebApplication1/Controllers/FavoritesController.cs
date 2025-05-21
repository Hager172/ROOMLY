using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ROOMLY.DTOs.favourite;
using ROOMLY.models;
using ROOMLY.UnitOfwork;
using AutoMapper;

namespace ROOMLY.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly UnitOfWork uOW;
        private readonly IMapper map;

        public FavoritesController(UnitOfwork.UnitOfWork UOW,IMapper map)
        {
            uOW = UOW;
            this.map = map;
        }



        [HttpPost]
        public IActionResult AddToFavorites(AddFavoriteDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User is not authenticated.");

            try
            {
                var favorite = map.Map<Favourite>(model);
                favorite.UserId = userId;
                // استخدم الـ UnitOfWork عشان تضيف
                uOW.favoriteRepo.Add(favorite);
                uOW.Save();

                return Ok(new { message = "Added to favorites successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpDelete("{roomId}")]
        public IActionResult RemoveFavorite(int roomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var favorites = uOW.favoriteRepo.GetFavoritesByUserId(userId);
            var favoriteToRemove = favorites.FirstOrDefault(f => f.RoomId == roomId);
            if (favoriteToRemove == null)
                return NotFound("Favorite not found");

            uOW.favoriteRepo.delete(favoriteToRemove.Id);
            uOW.Save();

            return NoContent();
        }

        [HttpGet]
        public IActionResult GetUserFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var favorites = uOW.favoriteRepo.GetFavoritesByUserId(userId);
            var favoriteDtos = map.Map<List<AddFavoriteDto>>(favorites);

            return Ok(favoriteDtos);
        }
    }
}
