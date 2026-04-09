using System.Text.Json;
using Azure.Core.Serialization;
using Investments.Advisor.AzureProxies;
using Investments.Advisor.Providers;
using Investments.Advisor.Trading;
using Trading.BvbScraper;
using Trading.Functions.Environments;

namespace Trading.Functions
{
	public class Program
	{
		private const string BVB_HTTP_CLIENT = "BvbHttpClient";
		private const string TRADE_AUTOMATION_CLIENT = "TradeAutomationHttpClient";
		private const string TRADE_ADVISOR_CLIENT = "TradeAdvisorHttpClient";
		private const string AZURE_TRADE_ORCHESTRATION_KEY_NAME = "Azure-TradeOrchestrationFuncKey";
		private const string AZURE_TRADE_AUTOMATION_KEY_NAME = "Azure-TradeAutomationFuncKey";

		public static void Main()
			{
				var host = new HostBuilder()
						.ConfigureFunctionsWorkerDefaults(worker =>
						{
							worker.Serializer = new JsonObjectSerializer(
								new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
						})
					.ConfigureServices(services =>
					{
						services.AddLogging();

						services
							.AddSingleton(new StockScraper())
							.AddSingleton<IEnvironment>(ResolveEnvironment)
							.AddSingleton<IBvbDataProvider>(ResolveBvbDataProvider)
							.AddSingleton<ITradeAutomation>(ResolveTradeAutomation)
							.AddSingleton<ITradeAdvisor>(ResolveTradeAdvisor);

						services.AddHttpClient(BVB_HTTP_CLIENT, ConfigureTradeOrchestrationClient);
						services.AddHttpClient(TRADE_AUTOMATION_CLIENT, ConfigureTradeAutomationClient);
						services.AddHttpClient(TRADE_ADVISOR_CLIENT, ConfigureTradeOrchestrationClient);

						services.AddSingleton<ITradeSessionOrchestrator, TradeSessionOrchestrator>();
					})
					.Build();

				host.Run();
			}

		private static IBvbDataProvider ResolveBvbDataProvider(IServiceProvider provider)
		{
			var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient(BVB_HTTP_CLIENT);
			return new AzureBvbDataProviderProxy(httpClient, GetEnvironmentVariable(AZURE_TRADE_ORCHESTRATION_KEY_NAME));
		}

		private static ITradeAutomation ResolveTradeAutomation(IServiceProvider provider)
		{
			var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient(TRADE_AUTOMATION_CLIENT);
			return new AzureTradeAutomationProxy(httpClient, GetEnvironmentVariable(AZURE_TRADE_AUTOMATION_KEY_NAME));
		}

		private static ITradeAdvisor ResolveTradeAdvisor(IServiceProvider provider)
		{
			var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient(TRADE_ADVISOR_CLIENT);
			return new AzureTradeAdvisorProxy(httpClient, GetEnvironmentVariable(AZURE_TRADE_ORCHESTRATION_KEY_NAME));
		}

		private static void ConfigureTradeAutomationClient(IServiceProvider provider, HttpClient client)
		{
			var environment = provider.GetService<IEnvironment>();
			client.BaseAddress = environment?.TradeAutomationFunctionsHost;
		}

		private static void ConfigureTradeOrchestrationClient(IServiceProvider provider, HttpClient client)
		{
			var environment = provider.GetService<IEnvironment>();
			client.BaseAddress = environment?.TradingFunctionsHost;
		}

		private static IEnvironment ResolveEnvironment(IServiceProvider provider)
		{
			bool isDevelopment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development";
			return isDevelopment ? new LocalEnvironment() : new ProductionEnvironment();
		}

		private static string GetEnvironmentVariable(string key)
			=> Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process) ?? string.Empty;
	}
}
