using Investments.Domain.Stocks.Extensions;
using Investments.Logic.Portfolios;
using Investments.Logic.Weights;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Threading.Tasks;

namespace Trading.Functions.Advisor
{
	public class TradeAdvisorFunctions
	{
		private const int MIN_ORDER_VALUE = 175;

		[Function(nameof(CalculateToBuyStocks))]
		public async Task<HttpResponseData> CalculateToBuyStocks(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = "calculateToBuyStocks")] HttpRequestData request)
		{
			var advisorRequest = await request.ReadFromJsonAsync<AdvisorRequest>();
			if (advisorRequest == null)
			{
				var errorResponse = request.CreateResponse(HttpStatusCode.BadRequest);
				await errorResponse.WriteStringAsync("Invalid request body");
				return errorResponse;
			}

			var stockPrices = advisorRequest.BvbStocks.AsStockPrices();
			var targetWeights = advisorRequest.BvbStocks.AsStockWeights();

			var existingStocks = advisorRequest.ExistingStocks.UpdatePrices(stockPrices);

			var strategy =
				new MinOrderValueCutOffStrategy(
					new FollowTargetAdjustmentStrategy(), MIN_ORDER_VALUE / advisorRequest.ToBuyAmount);

			var portfolio = new PortfolioBuilder()
				.UseStocks(existingStocks)
				.UsePrices(stockPrices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(advisorRequest.ToBuyAmount)
				.UseMinOrderValue(MIN_ORDER_VALUE)
				.UseWeightAdjustmentStrategy(strategy)
				.Build();

			var response = request.CreateResponse(HttpStatusCode.OK);
			await response.WriteAsJsonAsync(portfolio.DeriveToBuyStocks(existingStocks));
			return response;
		}
	}
}
