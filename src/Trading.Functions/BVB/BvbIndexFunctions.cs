using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Trading.BvbScraper;

namespace Trading.Functions.Bvb
{
	public class BvbIndexFunctions
	{
		private readonly StockScraper _stockScraper;
		private readonly ILogger<BvbIndexFunctions> _logger;

		public BvbIndexFunctions(StockScraper stockScraper, ILogger<BvbIndexFunctions> logger)
		{
			_stockScraper = stockScraper ?? throw new ArgumentNullException(nameof(stockScraper));
			_logger = logger;
		}

		[Function(nameof(ScrapeBvbIndex))]
		public async Task<HttpResponseData> ScrapeBvbIndex(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "ScrapeBvbIndex")] HttpRequestData request)
		{
			// Parse query string
			var query = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
			string? indexName = query["index"];

			if (string.IsNullOrEmpty(indexName))
			{
				var errorResponse = request.CreateResponse(HttpStatusCode.BadRequest);
				await errorResponse.WriteStringAsync("'index' query string param cannot be null or empty");
				return errorResponse;
			}

			_logger.LogInformation("Scraping BVB index: {IndexName}", indexName);

			var stocks = (await _stockScraper.ScrapeIndexdComposition(indexName)).ToArray();

			var response = request.CreateResponse(HttpStatusCode.OK);
			await response.WriteAsJsonAsync(stocks);
			return response;
		}
	}
}
