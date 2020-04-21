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
    using System.Collections.Generic;
    using InsertarPendienteDePagos.Helpers;
    using InsertarPendienteDePagos.Models;
    using System.Web.Helpers;
    using Microsoft.Azure.Documents.Client;
    using System.Linq;

    public class PagarCompra
    {
        [FunctionName("PagarCompra")]
        public async Task<IActionResult> UpdateTaskItem(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "PagarCompra/{id}")]HttpRequest req,
           [CosmosDB(
            databaseName: Constantes.COSMOS_DB_DATABASE_NAME,
            collectionName: Constantes.COSMOS_DB_CONTAINER_NAME,
            ConnectionStringSetting = "StrCosmos"
           )] DocumentClient client,
           ILogger logger,
           string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var option = new FeedOptions { EnableCrossPartitionQuery = true };

            var updatedTask = JsonConvert.DeserializeObject<Pagos>(requestBody);

            Uri taskCollectionUri = UriFactory.CreateDocumentCollectionUri(Constantes.COSMOS_DB_DATABASE_NAME, Constantes.COSMOS_DB_CONTAINER_NAME);

            var document = client.CreateDocumentQuery(taskCollectionUri, option)
                .Where(t => t.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            if (document == null)
            {
                logger.LogError($"TaskItem {id} not found. It may not exist!");
                return new NotFoundResult();
            }

            //bool pagado = document.GetPropertyValue<bool>("pagado");
            //pagado = true;
            document.SetPropertyValue("pagado", true);

            await client.ReplaceDocumentAsync(document);

            Pagos updatedTaskItemDocument = (dynamic)document;

            return new OkObjectResult(updatedTaskItemDocument);
        }
    }
}