using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
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

    [Function(nameof(GetAll))]
    public async Task<IActionResult> GetAll(
    [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
    [CosmosDBInput(cosmosDbName, cosmosContainerName, Connection = connectionString, SqlQuery = "SELECT * FROM c")] IReadOnlyList<MyDocument> myDocuments
    )
    {
        _ = req;
        return await Task.FromResult(new JsonResult(myDocuments));
    }

    [Function(nameof(GetOne))]
    public async Task<IActionResult> GetOne(
    [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
    [CosmosDBInput(cosmosDbName, cosmosContainerName, Connection = connectionString, Id = "1", PartitionKey = "1")] MyDocument myDocument
    )
    {
        _ = req;
        return await Task.FromResult(new JsonResult(myDocument));
    }

    [Function(nameof(GetOne404Error))]
    public async Task<IActionResult> GetOne404Error(
    [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
    [CosmosDBInput(cosmosDbName, cosmosContainerName, Connection = connectionString, Id = "404", PartitionKey = "404")] MyDocument myDocuments
    )
    {
        JsonResult result;
        try
        {
            _ = req;
            result = new JsonResult(myDocuments);
        }
        catch (Exception ex)
        {
            logger.LogError("Exception: {message}", ex.Message);
            throw;
        }
        return await Task.FromResult(result);

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
        public string? Id { get; set; }
        public string? Message { get; set; }
    }
}