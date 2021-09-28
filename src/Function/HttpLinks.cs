using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Collections.Generic;
using CTF_shared;

namespace LinkSpace
{
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
                return new BadRequestObjectResult("Send data as query -> name=yourname&answer=this,is,my,guess");

            var attempt = new Answer()
            {
                Id = answer.Trim().ToLower(),
                Name = name
            };

            CosmosClient cosmosClient = new CosmosClient(accountEndpoint, accountKey);
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            Container container = await database.CreateContainerIfNotExistsAsync(containerName, "/Name");

            bool wasAddedToDb = await TryAddAnswer(attempt, container);

            if (wasAddedToDb && attempt.Id == SecretAnswer)
                return new OkObjectResult("Your answer was accepted... And correct!!! Flag=" + flag);

            if (wasAddedToDb)
            {
                var attemptArr = attempt.Id.Split(',');
                var targetArr = SecretAnswer.Split(',');

                //Counts the matches with Zip and Aggregate
                var countMatch = targetArr.Zip(attemptArr, (ta, att) => ta == att).Aggregate(0, (count, isMatch) => isMatch ? count + 1 : count);

                //If matches exist they get appended to the response
                var response = countMatch > 0 ? "You matched " + String.Join(',', targetArr.Zip(attemptArr, (tar, att) => tar == att ? tar : "*******")) :
                //else a default response
                "Your answer was accepted... But none of the words matched, you are welcome to try again!";
                return new OkObjectResult(response);
            }

            return new BadRequestObjectResult("Your link couldn't be added.");
        }

        /// <summary>
        /// Checks if item already exist with id and partition key.
        /// If it does not exist it creates the item in the container.
        /// </summary>
        /// <param name="answer">An Answer object</param>
        /// <param name="container">Database Container</param>
        /// <returns>Returns true if it was added, false if it wasn't.</returns>
        private static async Task<bool> TryAddAnswer(Answer answer, Container container)
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
