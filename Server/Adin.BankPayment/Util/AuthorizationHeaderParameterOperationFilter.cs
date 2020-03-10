using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Adin.BankPayment.Util
{
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;
			var isAuthorized = filterDescriptors.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
			var allowAnonymous = filterDescriptors.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

			if (isAuthorized && !allowAnonymous)
			{
				if (operation.Parameters == null)
				{
					operation.Parameters = new List<OpenApiParameter>();
				}

				operation.Parameters.Add(new OpenApiParameter
				{
					Name = "Authorization",
					In = ParameterLocation.Header,
					Description = "access token",
					Required = true,
					Schema = new OpenApiSchema
					{
						Type = "string",
						Default = new OpenApiString("Bearer {access token}"),
					}
				});
			}
		}
	}
}