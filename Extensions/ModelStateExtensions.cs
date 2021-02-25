using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using Develover.WebUI.Models;

namespace Develover.WebUI.Extensions
{
    public static class ModelStateExtensions
    {
        public static IEnumerable<ModelStateError> AllModels(this ModelStateDictionary modelState)
        {
            var result = new List<ModelStateError>();
            var erroneousFields = modelState.OrderByDescending(x => x.Value.Errors.Count)
                                            .Select(x => new { x.Key, x.Value.ValidationState, x.Value.Errors });

            foreach (var erroneousField in erroneousFields)
            {
                var fieldKey = erroneousField.Key.Replace("model.", "");
                var state = erroneousField.ValidationState.ToString();
                var fieldErrors = erroneousField.Errors
                                   .Select(error => new ModelStateError(fieldKey, state, error.ErrorMessage));
                if (erroneousField.Errors.Count == 0)
                {
                    fieldErrors = new List<ModelStateError> { new ModelStateError(fieldKey, state, "") };
                }

                if (result.Any(a => a.Key == fieldKey))
                    continue;

                result.AddRange(fieldErrors);
            }

            return result;
        }
    }
}
