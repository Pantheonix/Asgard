using System.Threading.Tasks;
using Asgard.Hermes;

namespace EnkiProblems.Problems.Tests;

public interface ITestsService
{
    Task<UploadResponse> UploadTestAsync(UploadTestStreamDto input);
    Task<DownloadTestStreamDto> DownloadTestAsync(DownloadRequest input);
    Task<DeleteTestResponse> DeleteTestAsync(DeleteTestRequest input);
    Task<GetDownloadLinkForTestResponse> GetDownloadLinkForTestAsync(
        GetDownloadLinkForTestRequest input
    );
}
