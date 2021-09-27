using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace LinkSpace
{
    public class Answer
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
    public static class HttpLinks
    {
        private static string accountEndpoint = Environment.GetEnvironmentVariable("accountEndpoint", EnvironmentVariableTarget.Process);
        private static string accountKey = Environment.GetEnvironmentVariable("accountKey", EnvironmentVariableTarget.Process);
        private static string databaseName = Environment.GetEnvironmentVariable("databaseName", EnvironmentVariableTarget.Process);
        private static string containerName = Environment.GetEnvironmentVariable("containerName", EnvironmentVariableTarget.Process);
        private static string SecretAnswer = Environment.GetEnvironmentVariable("secretAnswer", EnvironmentVariableTarget.Process);
        private static string flag = Environment.GetEnvironmentVariable("flag", EnvironmentVariableTarget.Process);
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
            string answer = req.Query["answer"];

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(answer))
                return new BadRequestObjectResult("Send data as query -> ?name=yourname&answer=this,is,my,guess");

            var attempt = new Answer()
            {
                Id = answer,
                Name = name
            };

            CosmosClient cosmosClient = new CosmosClient(accountEndpoint, accountKey);
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            Container container = await database.CreateContainerIfNotExistsAsync(containerName, "/Name");

            bool wasAddedToDb = await TryAddAnswer(attempt, container);

            if (wasAddedToDb && attempt.Id == SecretAnswer)
                return new OkObjectResult("Your answer was accepted... And correct!!! Flag=" + flag);

            if (wasAddedToDb && attempt.Id.Trim().ToLower() != SecretAnswer.Trim().ToLower())
                return new OkObjectResult("Your answer was accepted... But incorrect, you are welcome to try again!");

            return new BadRequestObjectResult("Your link couldn't be added.");
        }

        /// <summary>
        /// Checks if item already exist with id and partition key.
        /// If it does not exist it creates the item in the container.
        /// </summary>
        /// <param name="answer">An Answer object</param>
        /// <param name="container">Database Container</param>
        /// <returns>Returns true if it was added, false if it wasn't.</returns>
        public static async Task<bool> TryAddAnswer(Answer answer, Container container)
        {
            try
            {
                await container.ReadItemAsync<Answer>(answer.Id, new PartitionKey(answer.Name));
            }
            catch (CosmosException ex)//Exception will never be useful in this case if not specified with a "when" statement.
            {
                await container.CreateItemAsync<Answer>(answer, new PartitionKey(answer.Name));
                return true;
            }
            return false;
        }
    }
}
