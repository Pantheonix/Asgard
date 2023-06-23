using Asgard.Hermes;
using Grpc.Core;
using Grpc.Net.Client;

const string address = "http://localhost:50010";
using var channel = GrpcChannel.ForAddress(address);
var client = new HermesTestsService.HermesTestsServiceClient(channel);

try
{
    var data = new GetDownloadLinkForTestRequest
    {
        ProblemId = "stardust",
        TestId = "9"
    };
    var metadata = new Grpc.Core.Metadata
    {
        { "dapr-app-id", "hermes-tests-service" }
    };
    
    var response = await client.GetDownloadLinkForTestAsync(data, metadata);

    Console.WriteLine(response);
}
catch (RpcException e)
{
    Console.WriteLine(e.Message);
}