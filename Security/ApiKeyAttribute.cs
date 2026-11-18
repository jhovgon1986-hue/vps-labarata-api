using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace VPS_LaBarata.API.Security
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "X-Api-Key"; // El nombre de la cabecera que buscaremos

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Verificar si la petición trae la cabecera X-Api-Key
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Result = new ContentResult() { StatusCode = 401, Content = "Acceso Denegado: API Key no proporcionada." };
                return;
            }

            // 2. Leer la llave oficial desde el appsettings.json
            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>("ApiKeySecret");

            // 3. Compararlas
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult() { StatusCode = 403, Content = "Acceso Denegado: API Key incorrecta." };
                return;
            }

            // 4. Si todo coincide, lo dejamos pasar al Controlador
            await next();
        }
    }
}