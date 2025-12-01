using Application.Features.Authentication.Command;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Authentication endpoints for user login and token generation
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")] // Fallback for backward compatibility
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticate user and generate JWT token
        /// </summary>
        /// <param name="command">Login credentials (email and password)</param>
        /// <returns>JWT token and expiration details</returns>
        /// <response code="200">Returns JWT token on successful authentication</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return response == null ? Unauthorized("Invalid credentials.") : Ok(response);
        }
    }
}
