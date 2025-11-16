using Application.Common.Results;
using Application.Interfaces;
using Application.Interfaces.SecurityInterface;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler(IUnitOfWork _unitOfWork, IPasswordHasher _passwordHasher, IEmailService _emailService) : IRequestHandler<CreateUserCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if email is already taken
                var emailTaken = await _unitOfWork.Users.IsEmailTakenAsync(request.Email);
                if (emailTaken)
                {
                    return Result<int>.Failure(
                        $"Email '{request.Email}' is already registered.",
                        errorCode: 1004); // DuplicateEmail
                }

                var hashedPassword = _passwordHasher.HashPassword(request.Password);

                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    Role = request.Role
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveAsync();

                // Send welcome email asynchronously (don't block on failure)
                try
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Welcome to RemoteLMS 🎉",
                        $"<h3>Hello {user.Name},</h3><p>Welcome aboard! Your account has been created successfully in my new LMS.</p>"
                    );
                }
                catch (Exception emailEx)
                {
                    // Log but don't fail the operation
                    System.Diagnostics.Debug.WriteLine($"Email send failed: {emailEx.Message}");
                }

                return Result<int>.Success(user.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.FailureFromException(
                    ex,
                    errorCode: 5001); // DatabaseError
            }
        }
    }
}
