using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.Service.LoggingAdapter.Helpers
{
    public static class ErrorResponceBuilder
    {
        public static ErrorResponse CreateErrorResponce(this ModelStateDictionary modelState)
        {
            var response = new ErrorResponse();

            foreach (var state in modelState)
            {
                var messages = state.Value.Errors
                    .Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                    .Select(e => e.ErrorMessage)
                    .Concat(state.Value.Errors
                        .Where(e => string.IsNullOrWhiteSpace(e.ErrorMessage))
                        .Select(e => e.Exception.Message))
                    .ToList();

                response.ModelErrors.Add(state.Key, messages);
            }

            return response;
        }
    }
}
