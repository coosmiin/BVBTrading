using System.Net;
using Investments.Advisor.Trading;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Trading.Functions.Orchestrator
{
	public class OrchestratorFunctions
	{
		private readonly ITradeSessionOrchestrator _orchestrator;
		private readonly ILogger<OrchestratorFunctions> _logger;

		public OrchestratorFunctions(
			ITradeSessionOrchestrator orchestrator,
			ILogger<OrchestratorFunctions> logger)
		{
			_orchestrator = orchestrator;
			_logger = logger;
		}

		[Function(nameof(StartTradingSession))]
		public async Task<HttpResponseData> StartTradingSession(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "startTradingSession")] HttpRequestData request)
		{
			_logger.LogInformation("Starting trading session via HTTP trigger");

			await _orchestrator.Run();

			var response = request.CreateResponse(HttpStatusCode.OK);
			await response.WriteStringAsync("Trading session started successfully");
			return response;
		}

		[Function(nameof(TriggerTradingSession))]
		public Task TriggerTradingSession([TimerTrigger("0 0 12 8 * *")] TimerInfo timer)
		{
			if (timer.IsPastDue)
			{
				_logger.LogInformation("Timer is running late");
				return Task.CompletedTask;
			}

			_logger.LogInformation("Starting scheduled trading session");
			return _orchestrator.Run();
		}
	}
}
