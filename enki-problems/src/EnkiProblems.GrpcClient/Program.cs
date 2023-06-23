using Asgard.Hermes;
using Grpc.Core;
using Grpc.Net.Client;

const string address = "http://localhost:4000";
using var channel = GrpcChannel.ForAddress(address);
var client = new HermesTestsService.HermesTestsServiceClient(channel);

try
{
    var data = new GetDownloadLinkForTestRequest
    {
        ProblemId = "stardust",
        TestId = "9"
    };
    var response = await client.GetDownloadLinkForTestAsync(data);

    Console.WriteLine(response);
}
catch (RpcException e)
{
    Console.WriteLine(e.Message);
}