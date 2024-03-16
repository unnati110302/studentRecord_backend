using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_crud.Models;

namespace student_crud.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly StudentContext _studentContext;
        public StateController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<State>>> GetStates()
        {
            return await _studentContext.States.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<State>> GetState(int id)
        {
            var state = await _studentContext.States.FindAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            return state;
        }

    }
}