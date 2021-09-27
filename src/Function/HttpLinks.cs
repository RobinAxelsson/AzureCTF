using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace LinkSpace
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
            string name = req.Query["name"];
            string url = req.Query["url"];
            string group = req.Query["group"];
            //string tags = req.Query["tags"];

            if (string.IsNullOrEmpty(group)) //|| string.IsNullOrEmpty(url))
            {
                return new BadRequestObjectResult("Send data as query -> ?name=linkname&url=linkurl&group=linkgroup&tags=animals,happy,tv");
            }
            var link = new Link()
            {
                Id = url,
                Name = name,
                Group = group,
                //Tags = tags.Split(',')
            };
            CosmosClient cosmosClient = new CosmosClient(accountEndpoint, accountKey);
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            Container container = await database.CreateContainerIfNotExistsAsync(containerName, "/Group");

            if (await TryAddLink(link, container))
            {
                return new OkObjectResult("Link was added successfully!");
            }
            return new BadRequestObjectResult("Your link couldn't be added.");
        }

        /// <summary>
        /// Checks if item already exist with id and partition key.
        /// If it does not exist it creates the item in the container.
        /// </summary>
        /// <param name="link"></param>
        /// <returns>Returns true if it was added, false if it wasn't.</returns>

        public static async Task<bool> TryAddLink(Link link, Container container)
        {
            try
            {
                await container.ReadItemAsync<Link>(link.Id, new PartitionKey(link.Group));
            }
            catch (CosmosException ex)//Exception will never be useful in this case if not specified with a "when" statement.
            {
                await container.CreateItemAsync<Link>(link, new PartitionKey(link.Group));
                return true;
            }
            return false;
        }
    }
}
