using API.Controllers;
using Application.Features.Users.Queries.GetUser;
using Application.Features.Users.Queries.GetAllUsers;
using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Application.Common.Results;

namespace Tests.xUnit.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new UsersController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserNull()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetUserByIDQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserDto?)null);

            var result = await _controller.GetUserById(42);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetUserById_ReturnsOk_WhenUserExists()
        {
            var dto = new UserDto { Id = 1, Name = "John" };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetUserByIDQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var result = await _controller.GetUserById(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateUser_ReturnsCreated_OnSuccessResult()
        {
            // construct Result<int>.Success and mock mediator to return it
            var success = Application.Common.Results.Result<int>.Success(10);
            _mockMediator.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(success);

            var cmd = new CreateUserCommand { Name = "A", Email = "a@b.com", Password = "p", Role = "User" };
            var result = await _controller.CreateUser(cmd);

            result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}
