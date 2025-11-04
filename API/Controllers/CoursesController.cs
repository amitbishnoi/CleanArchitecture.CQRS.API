using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Commands.DeleteCourse;
using Application.Features.Courses.Commands.UpdateCourse;
using Application.Features.Courses.Queries.GetAllCourses;
using Application.Features.Courses.Queries.GetCourseById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCoursesQuery());
            return Ok(result);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var query = new GetCourseByIdQuery(id);
            var user = await _mediator.Send(query);
            if (user == null)
                return NotFound(new { Message = $"User with ID {id} not found." });

            return Ok(user);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseCommand command)
        {
            var courseId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCourseById), new { id = courseId }, new { Id = courseId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            await _mediator.Send(command);
            return Ok("Successfully updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteCourseCommand { Id = id });
            return Ok("Successfully deleted");
        }
    }
}
