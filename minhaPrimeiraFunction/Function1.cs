using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace minhaPrimeiraFunction
{
    public static class Function1
    {
        [Function("function-app")]
        public async static Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("MinhaPrimeiraFunction");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var data = await req.GetBodyAsDeserializedAsync<RequestDataDTO>();

            // Mapear para uma entidade
            // Salvar no banco de dados (CosmosDB)

            return await req.Response(HttpStatusCode.OK, new { success = true, result = data } );
        }

        private async static Task<HttpResponseData> Response(this HttpRequestData req, HttpStatusCode statusCode, object jsonMessage = null)
        {
            var response = req.CreateResponse(statusCode);

            if (jsonMessage is not null)
            {
                await response.WriteAsJsonAsync(jsonMessage);
                return response;
            }

            return response;
        }
        private async static Task<T> GetBodyAsDeserializedAsync<T>(this HttpRequestData req) where T : class
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(requestBody);
        }
    }

    public class RequestDataDTO {
        [Required]
        public string ApplicationName { get; set; }
        public string LogMessage { get; set; }
        public string LogDetails { get; set; }

        public RequestDataDTO(string applicationName, string logMessage, string logDetails)
        {
            //if (applicationName.Length < 5)
            //    throw new Exception("Ocoreu um error"); // poderia usar o pattern de notificacoes

            ApplicationName = applicationName;
            LogMessage = logMessage;
            LogDetails = logDetails;
        }
    }

    internal class RequiredAttribute : Attribute
    {
    }
}
