using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Enrollment.Commands.DeleteEnrollment
{
    public class DeleteEnrollmentCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<DeleteEnrollmentCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _unitOfWork.Enrollment.GetByIdAsync(request.Id);
            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Enrollment not found with id : {request.Id}");
            }
            await _unitOfWork.Enrollment.DeleteAsync(enrollment);
            await _unitOfWork.SaveAsync();
            return Unit.Value;
        }
    }
}
