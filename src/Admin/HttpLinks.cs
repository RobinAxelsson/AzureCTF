using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using CTF_shared;
using Newtonsoft.Json;
namespace Admin
{
    public static class HttpLinks
    {
        private static string accountEndpoint = Environment.GetEnvironmentVariable("accountEndpoint", EnvironmentVariableTarget.Process);
        private static string accountKey = Environment.GetEnvironmentVariable("accountKey", EnvironmentVariableTarget.Process);
        private static string databaseName = Environment.GetEnvironmentVariable("databaseName", EnvironmentVariableTarget.Process);
        private static string containerName = Environment.GetEnvironmentVariable("containerName", EnvironmentVariableTarget.Process);
        private static ILogger _log;

        [FunctionName("HttpLinks")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            _log = log;
            log.LogInformation("C# HTTP trigger function processed a request.");
            log.LogInformation(req.QueryString.Value);

            string cmd = req.Query["cmd"];

            CosmosClient cosmosClient = new CosmosClient(accountEndpoint, accountKey);
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            Container container = await database.CreateContainerIfNotExistsAsync(containerName, "/Name");

            if (cmd == "delete")
            {
                await database.DeleteAsync();
                return new OkObjectResult("Database deleted");
            }
            var answers = await GetAllAnswersAsync(container);
            var json = JsonConvert.SerializeObject(answers);
            return new OkObjectResult(json);
        }

        /// <summary>
        /// Runs a query (using Azure Cosmos DB SQL syntax) against the container "all" and retrieves all links.
        /// </summary>
        private static async Task<List<Answer>> GetAllAnswersAsync(Container container)
        {
            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM all");
            FeedIterator<Answer> queryResultSetIterator = container.GetItemQueryIterator<Answer>(queryDefinition);

            var answers = new List<Answer>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Answer> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Answer a in currentResultSet)
                    answers.Add(a);
            }
            return answers;
        }
    }
}
