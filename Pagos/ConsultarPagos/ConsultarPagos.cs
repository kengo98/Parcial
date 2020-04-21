namespace ConsultarPagos
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using InsertarPendienteDePagos.Helpers;
    using InsertarPendienteDePagos.Models;

    public static class ConsultarPagos
    {
        [FunctionName(nameof(ConsultarPagos))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ConsultarPago/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: Constantes.COSMOS_DB_DATABASE_NAME,
                collectionName: Constantes.COSMOS_DB_CONTAINER_NAME,
                ConnectionStringSetting = "StrCosmos",
                SqlQuery ="SELECT * FROM c WHERE c.idCliente={id} and c.pagado = true")] IEnumerable<Pagos> productItem,
            ILogger log,
            string id)
        {
            if (productItem == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(productItem);
        }
    }
}
