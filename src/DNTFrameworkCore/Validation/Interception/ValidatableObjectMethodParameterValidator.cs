using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DNTFrameworkCore.Validation.Interception
{
    public class ValidatableObjectMethodParameterValidator : IMethodParameterValidator
    {
        public IEnumerable<ModelValidationResult> Validate(object parameter)
        {
            if (parameter == null || !(parameter is IValidatableObject validatable))
            {
                return Enumerable.Empty<ModelValidationResult>();
            }

            var failures = validatable.Validate(new ValidationContext(parameter));

            return ToModelValidationResult(failures);
        }

        private static IEnumerable<ModelValidationResult> ToModelValidationResult(
            IEnumerable<ValidationResult> failures)
        {
            foreach (var result in failures)
            {
                if (result == ValidationResult.Success) continue;

                if (result.MemberNames == null || !result.MemberNames.Any())
                {
                    yield return new ModelValidationResult(memberName: null, message: result.ErrorMessage);
                }
                else
                {
                    foreach (var memberName in result.MemberNames)
                    {
                        yield return new ModelValidationResult(memberName, result.ErrorMessage);
                    }
                }
            }
        }
    }
}