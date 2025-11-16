using Application.Common.Results;
using Application.Common.Responses;
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

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            var query = new GetAllUsersQuery
            {
                Pagination = new Application.Common.Models.PaginationParams
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm
                }
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var query = new GetUserByIDQuery(id);
            var user = await _mediator.Send(query);
            if (user == null)
                return NotFound(ApiResponse<object>.Fail(
                    $"User with ID {id} not found.",
                    statusCode: 404,
                    errorCode: 2001,
                    error: null
                ));

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            
            // Handle Result<int> response
            if (result is Result<int> createResult)
            {
                if (createResult.IsSuccess)
                {
                    var userId = createResult.Data;
                    var response = ApiResponse<int>.Ok(
                        userId, 
                        message: "User created successfully", 
                        statusCode: 201
                    );
                    return CreatedAtAction(nameof(GetUserById), new { id = userId }, response);
                }
                else
                {
                    var response = ApiResponse<int>.Fail(
                        createResult.ErrorMessage ?? "Failed to create user",
                        statusCode: 400,
                        errorCode: createResult.ErrorCode
                    );
                    return BadRequest(response);
                }
            }

            // Fallback for wrapped ApiResponse<Result<int>>
            return Ok(result);
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
