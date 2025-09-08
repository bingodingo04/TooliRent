using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class CategoryCreateUpdateValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        }
    }
}
