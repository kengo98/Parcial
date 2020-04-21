namespace fnc_cosmosdb
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using InsertarPendienteDePagos.Helpers;
    using InsertarPendienteDePagos.Models;

    public class InsertarPendientesDePago
    {
        [FunctionName("ProductInsert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                        databaseName:Constantes.COSMOS_DB_DATABASE_NAME,
                        collectionName:Constantes.COSMOS_DB_CONTAINER_NAME,
                        ConnectionStringSetting = "StrCosmos"
                       )] IAsyncCollector<object> pagos,
            ILogger log)
        {
            IActionResult returnValue = null;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Pagos>(requestBody);
                var pago = new Pagos
                {
                    id = data.id,
                    idCliente = data.idCliente,
                    productos = data.productos,
                    fecha = data.fecha,
                    montoTotal = data.montoTotal,
                    pagado = false
                };

                await pagos.AddAsync(pago);
                log.LogInformation($"Product Inserted {pago.id}");
                returnValue = new OkObjectResult(pago);
            }
            catch (Exception ex)
            {
                log.LogError($"Could not insert product. Exception: {ex.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return returnValue;
        }
    }
}
