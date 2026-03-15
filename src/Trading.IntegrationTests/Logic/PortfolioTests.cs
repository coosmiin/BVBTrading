using System.Linq;
using Investments.Domain.Stocks.Extensions;
using Investments.Logic.Calculus;
using Investments.Logic.Portfolios;
using Investments.Logic.Weights;
using NUnit.Framework;
using Trading.IntegrationTests.TestData;

namespace Trading.IntegrationTests.Logic
{
	public class PortfolioTests
	{
		private const int MIN_ORDER_VALUE = 175;

		[Test]
		[TestCase("BET")]
		[TestCase("BETAeRO")]
		public void PortfolioBuilder_ExpectedDerivedPortfolio(string index)
		{
			var toBuyAmount = 1339.1025m;
			var (currentStocks, bvbStocks, _) = TestResources.ReadStocks(index);

			var strategy =
				new MinOrderValueCutOffStrategy(
					new FollowTargetAdjustmentStrategy(), MIN_ORDER_VALUE / toBuyAmount);

			var portfolio = new PortfolioBuilder()
				.UseStocks(currentStocks)
				.UsePrices(bvbStocks.AsStockPrices())
				.UseTargetWeights(bvbStocks.AsStockWeights())
				.UseToBuyAmount(toBuyAmount)
				.UseMinOrderValue(MIN_ORDER_VALUE)
				.UseWeightAdjustmentStrategy(strategy)
				.Build();

			var toBuyStocks = portfolio.DeriveToBuyStocks(currentStocks);
			var investedAmount = toBuyStocks.Sum(s => s.Count * s.Price);

			Assert.That((toBuyAmount / investedAmount).IsApproxOne(), Is.True);
		}
	}
}
