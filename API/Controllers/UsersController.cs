using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Commands.DeleteUser;
using Application.Features.Users.Commands.UpdateUser;
using Application.Features.Users.Queries.GetAllUsers;
using Application.Features.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var query = new GetUserByIDQuery(id);
            var user = await _mediator.Send(query);
            if (user == null)
                return NotFound(new { Message = $"User with ID {id} not found." });

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var userId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { Id = userId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
                return BadRequest("User ID in the URL and body do not match.");

            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { Message = $"User with ID {id} not found." });

            return Ok(new { Message = "User updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { Message = $"User with ID {id} not found." });

            return Ok(new { Message = "User deleted successfully." });
        }
    }
}
