using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using EnkiProblems.Problems.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EnkiProblems.Controllers;

public class ProblemSubscriberController : EnkiProblemsController
{
    private readonly HttpClient _httpClient;
    private readonly DaprClient _daprClient;
    private readonly ILogger _logger;

    public ProblemSubscriberController(HttpClient httpClient, DaprClient daprClient, ILogger<ProblemSubscriberController> logger)
    {
        _httpClient = httpClient;
        _daprClient = daprClient;
        _logger = logger;
    }

    [Topic(EnkiProblemsConsts.PubSubName, EnkiProblemsConsts.TestUpsertedTopic)]
    [HttpPost(EnkiProblemsConsts.TestUpsertedTopic)]
    public async Task<ActionResult> HandleTestUpsertedAsync([FromBody] TestUpsertedEvent @event)
    {
        _logger.LogInformation("Received TestUpsertedEvent: {TestUpsertedEvent}", @event);

        // download input/output files locally from the URLs in the event
        // and persist them to the dapr statestore

        var inputResponse = await _httpClient.GetAsync(@event.InputDownloadUrl);
        var outputResponse = await _httpClient.GetAsync(@event.OutputDownloadUrl);

        if (inputResponse.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("Failed to download input file from {InputDownloadUrl}", @event.InputDownloadUrl);
            return BadRequest();
        }

        if (outputResponse.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("Failed to download output file from {OutputDownloadUrl}", @event.OutputDownloadUrl);
            return BadRequest();
        }

        var inputContent = await inputResponse.Content.ReadAsStringAsync();

        await _daprClient.DeleteStateAsync(
            EnkiProblemsConsts.StateStoreName,
            $"{@event.ProblemId}-{@event.Id}-{EnkiProblemsConsts.TestInputSuffix}"
            );

        await _daprClient.SaveStateAsync(
            EnkiProblemsConsts.StateStoreName,
            $"{@event.ProblemId}-{@event.Id}-{EnkiProblemsConsts.TestInputSuffix}",
            inputContent
        );

        var outputContent = await outputResponse.Content.ReadAsStringAsync();

        await _daprClient.DeleteStateAsync(
            EnkiProblemsConsts.StateStoreName,
            $"{@event.ProblemId}-{@event.Id}-{EnkiProblemsConsts.TestOutputSuffix}"
            );

        await _daprClient.SaveStateAsync(
            EnkiProblemsConsts.StateStoreName,
            $"{@event.ProblemId}-{@event.Id}-{EnkiProblemsConsts.TestOutputSuffix}",
            outputContent
        );

        return Ok();
    }

    [Topic(EnkiProblemsConsts.PubSubName, EnkiProblemsConsts.TestDeletedTopic)]
    [HttpPost(EnkiProblemsConsts.TestDeletedTopic)]
    public async Task<ActionResult> HandleTestDeletedAsync([FromBody] TestDeletedEvent @event)
    {
        _logger.LogInformation("Received TestDeletedEvent: {TestDeletedEvent}", @event);

        // delete the input/output files from the dapr statestore

        await _daprClient.DeleteStateAsync(
           EnkiProblemsConsts.StateStoreName,
           $"{@event.ProblemId}-{@event.Id}-{EnkiProblemsConsts.TestInputSuffix}"
           );

        await _daprClient.DeleteStateAsync(
           EnkiProblemsConsts.StateStoreName,
           $"{@event.ProblemId}-{@event.Id}-{EnkiProblemsConsts.TestOutputSuffix}"
           );

        return Ok();
    }
}