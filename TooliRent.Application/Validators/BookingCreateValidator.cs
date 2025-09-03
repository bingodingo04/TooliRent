using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class BookingCreateValidator : AbstractValidator<BookingCreateDto>
    {
        public BookingCreateValidator()
        {
            RuleFor(x => x.StartAt).LessThan(x => x.EndAt);
            RuleFor(x => x.ToolIds).NotEmpty();
        }
    }
}
