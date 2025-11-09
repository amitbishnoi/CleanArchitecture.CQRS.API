using Application.Features.Enrollment.Queries.GetEnrollmentById;
using Application.Features.Enrollment.Commands.CreateEnrollment;
using Application.Features.Enrollment.Commands.UpdateEnrollment;
using Application.Features.Enrollment.Commands.DeleteEnrollment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Enrollment.Queries.GetAllEnrollment;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController(IMediator _mediator) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var result = await _mediator.Send(new GetAllEnrollmentQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            var query = new GetEnrollmentByIdQuery(id);
            var enrollment = await _mediator.Send(query);
            if (enrollment == null)
                return NotFound(new { Message = $"Enrollment with ID {id} not found." });

            return Ok(enrollment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentCommand command)
        {
            var enrollmentId = await _mediator.Send(command);
            if (enrollmentId == 0)
            {
                return Conflict(new { Message = "Enrollment already exists for the given UserId and CourseId." });
            }
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollmentId }, new { Id = enrollmentId });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] UpdateEnrollmentCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");
            await _mediator.Send(command);
            return Ok("Successfully updated");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            await _mediator.Send(new DeleteEnrollmentCommand { Id = id});
            return Ok(new { Message = "Enrollment deleted successfully." });
        }
    }
}
