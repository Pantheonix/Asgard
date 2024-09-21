using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asgard.Hermes;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace EnkiProblems.Problems.Tests;

public class HermesTestsGrpcService : ITestService
{
    private readonly HermesTestsService.HermesTestsServiceClient _grpcClient;
    private readonly DaprMetadata _metadata;
    private readonly ILogger _logger;

    public HermesTestsGrpcService(
        HermesTestsService.HermesTestsServiceClient grpcClient,
        DaprMetadata metadata,
        ILogger<HermesTestsGrpcService> logger
    )
    {
        _grpcClient = grpcClient;
        _metadata = metadata;
        _logger = logger;
    }

    public async Task<UploadResponse> UploadTestAsync(UploadTestStreamDto input)
    {
        _logger.LogInformation(
            "Uploading test {TestId} for problem {ProblemId} with size {TestSize}",
            input.TestId,
            input.ProblemId,
            input.TestArchiveBytes.Length
        );

        using var call = _grpcClient.UploadTest(_metadata.HermesContext);

        var uploadMetadata = new Metadata
        {
            ProblemId = input.ProblemId,
            TestId = input.TestId,
            TestSize = input.TestArchiveBytes.Length
        };

        await call.RequestStream.WriteAsync(new UploadRequest { Metadata = uploadMetadata });

        foreach (var chunk in input.TestArchiveBytes.Chunk(EnkiProblemsConsts.TestChunkSize))
        {
            await call.RequestStream.WriteAsync(
                new UploadRequest { Chunk = new() { Data = ByteString.CopyFrom(chunk) } }
            );
        }

        await call.RequestStream.CompleteAsync();
        return await call;
    }

    public async Task<DownloadTestStreamDto> DownloadTestAsync(DownloadRequest input)
    {
        _logger.LogInformation(
            "Downloading test {TestId} for problem {ProblemId}",
            input.TestId,
            input.ProblemId
        );

        using var call = _grpcClient.DownloadTest(input, _metadata.HermesContext);

        var metadata = new Metadata();
        var statusCode = StatusCode.Unknown;
        var dataStream = new MemoryStream();

        var ctk = new CancellationToken();
        var readBytes = 0;

        while (await call.ResponseStream.MoveNext(ctk))
        {
            var res = call.ResponseStream.Current;

            switch (res.PacketCase)
            {
                case DownloadResponse.PacketOneofCase.Metadata:
                    metadata = res.Metadata;
                    break;
                case DownloadResponse.PacketOneofCase.Status:
                    statusCode = res.Status.Code;
                    break;
                case DownloadResponse.PacketOneofCase.Chunk:
                    await dataStream.WriteAsync(res.Chunk.Data.ToByteArray(), ctk);
                    readBytes += res.Chunk.Data.Length;

                    if (readBytes == metadata.TestSize)
                    {
                        goto response;
                    }
                    break;
                case DownloadResponse.PacketOneofCase.None:
                    break;
                default:
                    _logger.LogError(
                        "Unknown packet type: {PacketType} for test {TestId} of problem {ProblemId}",
                        res.PacketCase,
                        metadata.TestId,
                        metadata.ProblemId
                    );
                    throw new BusinessException(
                        EnkiProblemsDomainErrorCodes.DownloadTestArchiveError,
                        $"Unknown packet type: {res.PacketCase} for test {metadata.TestId} of problem {metadata.ProblemId}"
                    );
            }
        }

        response:
        return new DownloadTestStreamDto
        {
            ProblemId = metadata.ProblemId,
            TestId = metadata.TestId,
            StatusCode = statusCode,
            TestArchiveBytes = dataStream.ToArray()
        };
    }

    public async Task<DeleteTestResponse> DeleteTestAsync(DeleteTestRequest input)
    {
        _logger.LogInformation(
            "Deleting test {TestId} for problem {ProblemId}",
            input.TestId,
            input.ProblemId
        );
        return await _grpcClient.DeleteTestAsync(input, _metadata.HermesContext);
    }

    public async Task<GetDownloadLinkForTestResponse> GetDownloadLinkForTestAsync(
        GetDownloadLinkForTestRequest input
    )
    {
        _logger.LogInformation(
            "Getting download link for test {TestId} for problem {ProblemId}",
            input.TestId,
            input.ProblemId
        );
        return await _grpcClient.GetDownloadLinkForTestAsync(input, _metadata.HermesContext);
    }
}
