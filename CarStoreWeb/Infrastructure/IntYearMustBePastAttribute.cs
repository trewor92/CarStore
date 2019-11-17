using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CarStoreWeb.Infrastructure
{
    public class IntYearMustBePastAttribute : Attribute, IModelValidator
    {
        public bool IsRequired => true;

        private const int _minYear=1900;

        public string ErrorMessage { get; set; } = $"Year must be from {_minYear} to {DateTime.Now.Year}";



        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            int? value = context.Model as int?;
            if (value.HasValue && value.Value > _minYear && value.Value <= DateTime.Now.Year)
                return Enumerable.Empty<ModelValidationResult>();
            else
                return new List<ModelValidationResult> {
                    new ModelValidationResult("", ErrorMessage) };

        }
    }
}
