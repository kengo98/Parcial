namespace EliminarPendienteDePagos
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
    using Microsoft.Azure.Documents.Client;
    using System.Linq;
    using Microsoft.Azure.Documents;

    public class EliminarPendienteDePagos
    {
        [FunctionName("EliminarPendienteDePagos")]
        public async Task<IActionResult> UpdateTaskItem(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Eliminar/{id}")]HttpRequest req,
           [CosmosDB(
            databaseName: Constantes.COSMOS_DB_DATABASE_NAME,
            collectionName: Constantes.COSMOS_DB_CONTAINER_NAME,
            ConnectionStringSetting = "StrCosmos"
           )] DocumentClient pago,
           ILogger logger,
           string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var option = new FeedOptions { EnableCrossPartitionQuery = true };

            var updatedTask = JsonConvert.DeserializeObject<Pagos>(requestBody);

            Uri taskCollectionUri = UriFactory.CreateDocumentCollectionUri(Constantes.COSMOS_DB_DATABASE_NAME, Constantes.COSMOS_DB_CONTAINER_NAME);

            var document = pago.CreateDocumentQuery(taskCollectionUri, option)
                .Where(t => t.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            if (document == null)
            {
                logger.LogError($"TaskItem {id} not found. It may not exist!");
                return new OkObjectResult("No existe el ID ingresado");
            }

            //bool pagado = document.GetPropertyValue<bool>("pagado");
            //pagado = true;


            await pago.DeleteDocumentAsync(document.SelfLink, new RequestOptions { PartitionKey = new PartitionKey(document.Id) });


            return new OkObjectResult("se Elimino correctamente  ");
        }
    }
}