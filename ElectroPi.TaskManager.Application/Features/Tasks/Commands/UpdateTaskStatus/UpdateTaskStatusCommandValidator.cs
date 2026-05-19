using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Commands.UpdateTaskStatus
{

    public sealed class UpdateTaskStatusCommandValidator
        : AbstractValidator<UpdateTaskStatusCommand>
    {
        public UpdateTaskStatusCommandValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty().WithMessage("Task ID is required.");

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Invalid task status value.");
        }
    }
}
