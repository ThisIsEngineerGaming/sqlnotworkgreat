using Microsoft.AspNetCore.Mvc;
using Film.Application.DTO;
using Film.Application.Interfaces;
using Film.Common.Exceptions;

namespace Film.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmsController : ControllerBase
    {
        private readonly IEntityService<FilmDTO> filmService;

        public FilmsController(IEntityService<FilmDTO> serv)
        {
            filmService = serv;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilmDTO>>> Get()
        {
            var films = await filmService.GetAll();
            return Ok(films);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FilmDTO>> Get(int id)
        {
            try
            {
                var film = await filmService.Get(id);
                return Ok(film);
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] FilmDTO film)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await filmService.Create(film);
                return Ok();
            }
            catch (BusinessRuleException ex)
            {
                // бізнес-правило (наприклад, дублікат назви), а не помилка анотації моделі
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] FilmDTO film)
        {
            if (id != film.Id)
            {
                return BadRequest("Ідентифікатор у маршруті не збігається з ідентифікатором у тілі запиту.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await filmService.Update(film);
                return NoContent();
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await filmService.Delete(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
