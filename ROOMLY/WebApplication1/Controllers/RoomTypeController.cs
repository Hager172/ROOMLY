using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ROOMLY.DTOs.RoomDTO;
using ROOMLY.UnitOfwork;

using WebApplication1.models;

namespace ROOMLY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly UnitOfWork unitOf;
        private readonly IMapper map;

        public RoomTypeController(UnitOfWork unitOf, IMapper map)
        {
            this.unitOf = unitOf;
            this.map = map;
        }

        #region get all
        [HttpGet]
        public IActionResult GetAll()
        {
            var types = unitOf.roomTypeRepo.GetAll();
            var typesdto = map.Map<List<RoomTypeDto>>(types);
            return Ok(typesdto);
        }
        #endregion

        #region get by id
        [HttpGet("{id}")]
        public IActionResult Getbyid(int id)
        {
            var type = unitOf.roomTypeRepo.GetbyId(id);

            if (type == null) return NotFound("Type not found");

            var typedto = map.Map<RoomTypeDto>(type);
            return Ok(typedto);
        }
        #endregion

        #region create type
        [HttpPost]
        public IActionResult Create(RoomTypeCreateDto roomdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exiexttype = unitOf.roomTypeRepo.GetAll()
                               .FirstOrDefault(t => t.Name == roomdto.Name);
            if (exiexttype != null) return Conflict("Type already exists");

            var type = map.Map<RoomType>(roomdto);
            unitOf.roomTypeRepo.Add(type);
            unitOf.Save();
          var newtyp =  map.Map<RoomTypeCreateDto>(type);
            
            return CreatedAtAction(nameof(Getbyid), new { id = type.RoomTypeId }, newtyp);
        }
        #endregion

        #region edit type
        [HttpPut("{id}")]
        public IActionResult UpdateType(int id, RoomTypeCreateDto typedto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var testtype = unitOf.roomTypeRepo.GetbyId(id);
            if (testtype == null) return NotFound("Type not found");

            map.Map(typedto, testtype);
            unitOf.roomTypeRepo.edit(testtype);
            unitOf.Save();
            return NoContent();
        }
        #endregion

        #region delete
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var type = unitOf.roomTypeRepo.GetbyId(id);
            if (type == null) return NotFound("Type not found");

            unitOf.roomTypeRepo.delete(id);
            unitOf.Save();
            return NoContent();
        }
        #endregion
    }


}
