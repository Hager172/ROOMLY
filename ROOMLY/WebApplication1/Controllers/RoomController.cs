using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ROOMLY.DTOs.RoomDTO;
using WebApplication1.models;
using ROOMLY.UnitOfwork;
using ROOMLY.models;

namespace ROOMLY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper map;

        public RoomController(  UnitOfWork  unitOfWork, IMapper map)
        {
            this._unitOfWork = unitOfWork;
            this.map = map;
        }

        [HttpGet("GetRooms")]
        public IActionResult GetAllRooms()
        {
            var rooms = _unitOfWork.roomRepo.GetAll();
            var roomDtos = map.Map<List<ROOMdto>>(rooms);
            return Ok(roomDtos);
        }
        [HttpGet("GetAvailableRooms")]
        public IActionResult GetAvailableRooms()
        {
            var rooms = _unitOfWork.roomRepo.GetAvailableRooms();
            var roomsdtos = map.Map<List<ROOMdto>>(rooms);


            return Ok(roomsdtos);
        }





        [HttpGet("{id}")]
        public IActionResult GetRoomById(int id)
        {
            var room = _unitOfWork.roomRepo.GetbyId(id);
            if (room == null) return NotFound("Room not found");

            var roomDto = map.Map<ROOMdto>(room);
            return Ok(roomDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoomm([FromForm] RoomCreateDto roomcreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingRoom = _unitOfWork.roomRepo.GetAll().FirstOrDefault(r => r.RoomNumber == roomcreate.RoomNumber);
            if (existingRoom != null)
                return Conflict("Room number already exists.");

            var room = new Room
            {
                RoomNumber = roomcreate.RoomNumber,
                RoomTypeId = roomcreate.RoomTypeId,
                MainImage = null  // هنعبيها بعد رفع الصورة
            };

            _unitOfWork.roomRepo.Add(room);
            _unitOfWork.Save();


            // رفع الصورة الرئيسية MainImage
            if (roomcreate.MainImage != null && roomcreate.MainImage.Length > 0)
            {
                var mainImageFileName = await SaveImage(roomcreate.MainImage);
                room.MainImage = "/images/" + mainImageFileName;
            }

            // رفع صور المعرض GalleryImages
            if (roomcreate.GalleryImages != null && roomcreate.GalleryImages.Count > 0)
            {
                foreach (var img in roomcreate.GalleryImages)
                {
                    if (img.Length > 0)
                    {
                        var fileName = await SaveImage(img);
                        var roomImage = new RoomImage
                        {
                            RoomId = room.RoomId,
                            ImageUrl = "/images/" + fileName
                        };
                        _unitOfWork.roomImageRepository.Add(roomImage);
                    }
                }
            }

            _unitOfWork.Save();

            var roomDto = map.Map<RoomDetailsDto>(room);
            return Ok(roomDto);

        }

        // دالة رفع الصورة على السيرفر وتوليد اسم ملف جديد
        private async Task<string> SaveImage(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        [HttpPut("{id}")]

        public IActionResult RoomEdit(int id, RoomCreateDto roomdto) {

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var room = _unitOfWork.roomRepo.GetbyId(id);
            if (room == null) return NotFound("Room Not Found");
            map.Map(roomdto, room);
            _unitOfWork.roomRepo.edit(room);
            _unitOfWork.Save();
            return NoContent();




        }

        [HttpDelete("{id}")]

        public IActionResult DeleteRoom(int id) {

            var room = _unitOfWork.roomRepo.GetbyId(id);
            if (room == null) return NotFound("Room Is Not Found");
            if (room.Reservations != null && room.Reservations.Any())
                return BadRequest("Cannot delete a room that has reservations.");

            _unitOfWork.roomRepo.delete(id);
            _unitOfWork.Save();
            return NoContent();

       
        
        
        }



    }
}
