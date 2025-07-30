
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KC.Web.Extension
{
    public class SwaggerAuthorizeOperationFilter : IOperationFilter
    {
        //private Tenant _tenant;
        //public SwaggerAuthorizeOperationFilter(Tenant tenant)
        //{
        //    _tenant = tenant;
        //}

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>();
            var isAuthorized = authAttributes.Any();

            #region Swagger授权处理
            if (isAuthorized)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
                operation.Responses.TryAdd("500", new OpenApiResponse { Description = "Internal Server Error" });

                var oauth2SecurityScheme = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                };

                operation.Security.Add(new OpenApiSecurityRequirement()
                {
                    [oauth2SecurityScheme] = new[] { 
                        ApplicationConstant.OpenIdScope,  
                        ApplicationConstant.ProfileScope,
                        ApplicationConstant.AdminScope, 
                        ApplicationConstant.AppScope,       
                        ApplicationConstant.AccScope, 
                        ApplicationConstant.CfgScope, 
                        ApplicationConstant.DicScope, 
                        ApplicationConstant.CrmScope}

                });
            }
            #endregion

            #region Swagger 文件上传处理

            var files = context.ApiDescription.ActionDescriptor.Parameters.Where(n => n.ParameterType == typeof(IFormFile)).ToList();
            if (files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    if (i == 0)
                    {
                        operation.Parameters.Clear();
                    }
                    //operation.Parameters.Add(new NonBodyParameter
                    //{
                    //    Name = files[i].Name,
                    //    In = "formData",
                    //    Description = "Upload File",
                    //    Required = true,
                    //    Type = "file"
                    //});

                }
                //operation.Consumes.Add("multipart/form-data");
            }
            #endregion
        }
    }
}
