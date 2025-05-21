using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ROOMLY.DTOs.RoomDTO;
using WebApplication1.models;
using ROOMLY.UnitOfwork;

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

        #region get by id
        [HttpGet("id")]
        public IActionResult Getbyid(int id) {
        
        var reservation = uTO.reservationRepo.GetbyId(id);
            if (reservation == null) { return NotFound("reservation is not found"); }
            var reservationdto = map.Map<ReservationDto>(reservation); 
            return Ok(reservationdto);
        
        
        
        }
        #endregion

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
        [HttpPut("id")]
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
