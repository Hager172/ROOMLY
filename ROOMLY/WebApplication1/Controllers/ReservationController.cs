using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ROOMLY.DTOs.RoomDTO;
using WebApplication1.models;
using ROOMLY.UnitOfwork;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace ROOMLY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly UnitOfWork uTO;
        private readonly IMapper map;

        public ReservationController(UnitOfWork UTO, IMapper map)
        {
            uTO = UTO;
            this.map = map;
        }

        #region get all
        [HttpGet]
        public IActionResult GetAll() { 
        
            var reservation = uTO.reservationRepo.GetAll();
            var reservationdto = map.Map<List<ReservationDto>>(reservation);
            return Ok(reservationdto);
        
        
        
        }
        #endregion
        [HttpGet("user/myreservations")]
        [Authorize]
        public IActionResult GetMyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var reservations = uTO.reservationRepo.GetReservationsByUserId(userId);

            var reservationsDto = map.Map<List<ReservationDto>>(reservations);
            return Ok(reservationsDto);
        }


        #region get by id
        [HttpGet("id")]
        public IActionResult Getbyid(int id) {
        
        var reservation = uTO.reservationRepo.GetbyId(id);
            if (reservation == null) { return NotFound("reservation is not found"); }
            var reservationdto = map.Map<ReservationDto>(reservation); 
            return Ok(reservationdto);
        
        
        
        }
        #endregion

        [HttpPost("user/create")]
        public IActionResult CreateByUser(ReservationCreateUserDto reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // الحصول على الـ userId من الـ JWT Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            // التأكد إن الغرفة متاحة
            bool isAvailable = uTO.reservationRepo.IsRoomAvailable(
                reservationDto.RoomId,
                reservationDto.CheckInDate,
                reservationDto.CheckOutDate
            );

            if (!isAvailable)
                return Conflict("The room is not available for the selected period.");

            // تحويل الـ DTO إلى Entity
            var reservationEntity = map.Map<Reservation>(reservationDto);
            reservationEntity.UserId = userId;

            // إضافة الحجز
            uTO.reservationRepo.Add(reservationEntity);
            uTO.Save();

            // جلب الحجز مع بيانات الغرفة والمستخدم
            var fullReservation = uTO.reservationRepo.GetByIdWithDetails(reservationEntity.ResevationId);
            if (fullReservation == null)
                return NotFound("Reservation was created but couldn't be retrieved.");

            // تحويله إلى DTO للعرض
            var newReservationDto = map.Map<ReservationDto>(fullReservation);

            return CreatedAtAction(nameof(Getbyid), new { id = newReservationDto.ResevationId }, newReservationDto);
        }



        #region create
        [HttpPost]
        public IActionResult create(ReservationCreateDto reservation) {
        
            if(!ModelState.IsValid) return BadRequest(ModelState);

            bool isavailable = uTO.reservationRepo.IsRoomAvailable(reservation.RoomId,reservation.CheckInDate,reservation.CheckOutDate);
            if (isavailable) {
                var reservationold = map.Map<Reservation>(reservation);
                uTO.reservationRepo.Add(reservationold);
                uTO.Save();
                var newreserv = map.Map<ReservationCreateDto>(reservationold);
             
                return CreatedAtAction(nameof(Getbyid), newreserv);
            
            
            
            }
            return Conflict("The room is not available for the selected period.");






        }
        #endregion

        #region update
        [HttpPut("{id}")]
        public IActionResult update(int id, ReservationCreateDto reservation) {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var reserv = uTO.reservationRepo.GetbyId(id);
            if (reservation == null)  return NotFound("reservation is not found");

            bool isavailable = uTO.reservationRepo.IsRoomAvailable(reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate,id);
            if (isavailable) {
            map.Map(reservation,reserv);
                uTO.reservationRepo.edit(reserv);
                uTO.Save();
                return NoContent();
            
            
            
            
            }

            return Conflict("The room is not available for the selected period.");





        }


        #endregion

        [HttpPut("user/{id}")]
        public IActionResult UpdateReservationByUser(int id, ReservationCreateDto reservation)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var reserv = uTO.reservationRepo.GetbyId(id);
            if (reserv == null) return NotFound("Reservation not found");

            var now = DateTime.Now;
            var hoursDifference = (reserv.CheckInDate - now).TotalHours;

            if (hoursDifference < 24)
            {
                return BadRequest("Cannot update reservation less than 24 hours before check-in.");
            }



            bool isavailable = uTO.reservationRepo.IsRoomAvailable(reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate, id);
            if (isavailable)
            {
                map.Map(reservation, reserv);
                uTO.reservationRepo.edit(reserv);
                uTO.Save();
                return NoContent();
            }

            return Conflict("The room is not available for the selected period.");
        }


        #region cansel reservation
        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = uTO.reservationRepo.GetbyId(id);
            if (reservation == null) return NotFound("Reservation not found");

            uTO.reservationRepo.delete(id);
            uTO.Save();
            return NoContent();
        }

        #endregion

    }
}
