using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Models.Validators
{
    public class PaginationQueryValidator : AbstractValidator<PaginationQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 10, 15 };

        public PaginationQueryValidator()
        {
            RuleFor( r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                {
                    context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPageSizes)}]");
                }
            });

        }
    }
}
