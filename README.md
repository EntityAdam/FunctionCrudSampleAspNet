# FunctionCrudSampleAspNet

Sample functions application.

This sample is using:
- .NET 8  
- Azure Functions V4 isolated  
- AspNetCore integration  
- CosmosDb Function input and output bindings  

# Prerequisites
- .NET 8  
- Cosmos Db emulator or CosmosDb instance  
- Azurite Storage Emulator  

# To start

## local.settings.json

You should create a file named `local.settings.json` with the following configuration to specify the worker runtime of "dotnet-isolate" as well as using the Azureite storage emulator for the functions host to work.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

## CosmosDb ConnectionString
This project is using `dotnet user-secrets`, so you may set the connection string with the expected key of `CosmosDBConnection` using the following:  

```sh
dotnet user-secrets init
# This connection string is a well-known AccountKey for CosmosDB emulator
dotnet user-secrets set "CosmosDBConnection" "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
```

Or you may set your connection string in `local.settings.json`  

```js
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDBConnection": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
  }
}
```

## Run
In Visual Studio, use F5 or run to start the application with debugging.  

Once the function host starts you will see the index of the functions  

```sh
Functions:
        Create: [POST] http://localhost:7069/api/Create
        GetAll: [GET] http://localhost:7069/api/GetAll
        GetOne: [GET] http://localhost:7069/api/GetOne
        GetOneSingle: [GET] http://localhost:7069/api/GetOneSingle
```

## `api/Create` endpoint

In the `Tests` folder I have created a `.http` file with a sample POST to create a 'MyDocument'

The `api/Create` endpoint will ensure the database named `example `and container named `documents` are created, as well as set the partition key to `/id` for the container.


## ``api/GetAll` endpoint

The `api/GetAll` endpoint will retrieve all documents by using a SQL Query of `"SELECT * FROM c"`, and bind it to a `ReadOnlyList<T>` of a POCO named `MyDocument` which is a nested class under the Api class.

## ``api/GetOne` endpoint

The `api/GetAll` endpoint will retrieve a single `MyDocument` with the Id of "1" (string) and PartitionKey of "1", demonstrating a _point query_. This will bind to the `MyDocument` POCO.

If you used the example `.http` file to create the document in the database, you will get a result of:

```json
{
    "id": "1",
    "message": "Hello, World!"
}
```

## ``api/GetOne404Error` endpoint

This endpoint is intentionally broken to reproduce an error. Unless you create a CosmosDb document with the Id of "404". You will experience the following error.

The web browser will display a 500 internal server error.
The functions app host in the terminal will display the following error stack trace.

```sh
[2025-01-05T02:58:04.821Z] Executing 'Functions.GetOne404Error' (Reason='This function was programmatically called via the host APIs.', Id=c1ac3881-1699-4ede-a498-51b8501558b5)
[2025-01-05T02:58:07.710Z] Function 'GetOne404Error', Invocation id 'c1ac3881-1699-4ede-a498-51b8501558b5': An exception was thrown by the invocation.
[2025-01-05T02:58:07.711Z] Result: Function 'GetOne404Error', Invocation id 'c1ac3881-1699-4ede-a498-51b8501558b5': An exception was thrown by the invocation.
Exception: Microsoft.Azure.Functions.Worker.FunctionInputConverterException: Error converting 1 input parameters for Function 'GetOne404Error': Cannot convert input parameter 'myDocuments' to type 'FunctionCrudSampleAspNet.Api+MyDocument' from type 'Microsoft.Azure.Functions.Worker.Grpc.Messages.GrpcModelBindingData'. Error:Microsoft.Azure.Cosmos.CosmosException : Response status code does not indicate success: NotFound (404); Substatus: 0; ActivityId: 888741c2-6d9f-4b37-8ec1-25c94dc41ab1; Reason: (
[2025-01-05T02:58:07.712Z] code : NotFound
[2025-01-05T02:58:07.713Z] message : Message: {"Errors":["Resource Not Found. Learn more: https://aka.ms/cosmosdb-tsg-not-found"]}
```