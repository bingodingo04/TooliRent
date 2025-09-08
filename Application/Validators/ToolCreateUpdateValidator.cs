using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class ToolCreateUpdateValidator : AbstractValidator<ToolCreateUpdateDto>
    {
        public ToolCreateUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}
