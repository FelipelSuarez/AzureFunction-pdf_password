using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace pdf_password
{
    public static class pdf_password
    {
        [FunctionName("pdf_password")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string senha = req.Query["senha"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var bytes = Convert.FromBase64String(requestBody);

                var pdf = PdfSharpCore.Pdf.IO.PdfReader.Open(new MemoryStream(bytes));

                pdf.SecuritySettings.UserPassword = senha;

                var responseStream = new MemoryStream();
                pdf.Save(responseStream);

                return new OkObjectResult(Convert.ToBase64String(responseStream.ToArray()));

            }
            catch (Exception e)
            {
                log.LogError(e, "erro no processo");
                return new OkObjectResult(false);
            }
           
        }
    }
}
