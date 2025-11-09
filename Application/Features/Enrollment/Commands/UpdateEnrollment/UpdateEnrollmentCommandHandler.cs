using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Enrollment.Commands.UpdateEnrollment
{
    public class UpdateEnrollmentCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<UpdateEnrollmentCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _unitOfWork.Enrollment.GetByIdAsync(request.Id);
            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Enrollment not found with id : {request.Id}");
            }
            enrollment.UserId = request.UserId;
            enrollment.CourseId = request.CourseId;
            await _unitOfWork.Enrollment.UpdateAsync(enrollment);
            await _unitOfWork.SaveAsync();
            return Unit.Value;
        }
    }
}
