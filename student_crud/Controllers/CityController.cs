using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_crud.Models;

namespace student_crud.Controllers
{

    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly StudentContext _studentContext;
        public CityController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [Route("api/City")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetStates()
        {
            return await _studentContext.Cities.ToListAsync();
        }

        [Route("api/City/{id}")]
        [HttpGet]
        public async Task<ActionResult<City>> GetState(int id)
        {
            var cityVar = await _studentContext.Cities.FindAsync(id);

            if (cityVar == null)
            {
                return NotFound();
            }

            return cityVar;
        }

        [Route("api/cities/{stateId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities(int stateId)
        {
            var filteredCities = await _studentContext.Cities.Where(city => city.StateId == stateId).ToListAsync();
            return Ok(filteredCities);
        }


    }
}