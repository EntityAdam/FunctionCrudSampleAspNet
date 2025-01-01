using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace FunctionCrudSampleAspNet;

public class Api(ILogger<Api> logger)
{
    private const string cosmosDbName = "example";
    private const string cosmosContainerName = "documents";
    private const string connectionString = "CosmosDBConnection";

    [Function(nameof(Create))]
    public Task<CreateResponse> Create(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [FromBody] MyDocument myDocument
        )
    {
        _ = req;
        logger.LogInformation("C# HTTP trigger function processed a request.");
        CreateResponse response = new() { Response = new JsonResult(myDocument), Document = myDocument };
        return Task.FromResult(response);
    }

    public class CreateResponse
    {
        [HttpResult]
        public required IActionResult Response { get; set; }

        [CosmosDBOutput(cosmosDbName, cosmosContainerName, Connection = connectionString, CreateIfNotExists = true, PartitionKey = "/id")]
        public MyDocument? Document { get; set; }
    }

    public class MyDocument
    {
        public string? id { get; set; }
        public string? message { get; set; }
    }
}