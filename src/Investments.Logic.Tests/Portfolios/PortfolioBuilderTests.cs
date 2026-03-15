using Investments.Domain.Stocks;
using Investments.Logic.Portfolios;
using Investments.Logic.Weights;
using NUnit.Framework;
using System;
using System.Linq;

namespace Investments.Logic.Tests.Portfolios
{
	public partial class PortfolioBuilderTests
	{
		[Test]
		public void BuildPortfolio_NotEnoughForAny_PortfolioValueIsZero()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.5m }, {  "FP", 0.5m } };

			var portfolio = new PortfolioBuilder()
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(9)
				.Build();

			Assert.That(portfolio.TotalValue, Is.Zero);
		}

		[Test]
		public void BuildPortfolio_NotEnoughForAll_PortfolioValueIsCorrect()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.5m }, {  "FP", 0.5m } };

			var portfolio = new PortfolioBuilder()
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(20)
				.Build();

			Assert.That(portfolio.TotalValue, Is.EqualTo(10));
		}

		[Test]
		public void BuildPortfolio_AlmostEnoughForAll_SmallestEligibleStockIsDecreased()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 }, { "EL", 30 } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.3m }, { "EL", 0.5m } };

			var portfolio = new PortfolioBuilder()
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(119)
				.Build();

			Assert.That(portfolio["EL"].Count, Is.EqualTo(2));
			Assert.That(portfolio["FP"].Count, Is.EqualTo(2));
			Assert.That(portfolio["TLV"].Count, Is.EqualTo(1));

			Assert.That(portfolio.TotalValue, Is.EqualTo(110));
		}

		[Test]
		public void BuildPortfolio_AtLeastOneTargetWeightAboveAboveOneHundredPercent_ThrowsArgumentException()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 }, { "EL", 30 } };

			var targetWeights = new StockWeights
			{ { "TLV", 1.2m }, { "FP", 0.3m }, { "EL", 0.5m } };

			var builder = new PortfolioBuilder()
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(100);

			Assert.Throws<ArgumentException>(() => builder.Build());
		}

		[Test]
		public void BuildPortfolio_SumOfTargetWeightNotApproxEqualWithOneHundredPercent_ThrowsArgumentException()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 }, { "EL", 30 } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.3m }, { "FP", 0.3m }, { "EL", 0.5m } };

			var builder = new PortfolioBuilder()
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(100);

			Assert.Throws<ArgumentException>(() => builder.Build());
		}

		[Test]
		public void BuildPortfolio_SumOfTargetWeightApproxEqualWithOneHundredPercent_DoesNotThrow()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 }, { "EL", 30 } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.21m }, { "FP", 0.3m }, { "EL", 0.5m } };

			var builder = new PortfolioBuilder()
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(100);

			Assert.DoesNotThrow(() => builder.Build());
		}

		[Test]
		public void BuildPortfolio_MissingPriceForTargetSymbol_ThrowsArgumentException()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 }, { "EL", 30 } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.21m }, { "FP", 0.3m } };

			var builder = new PortfolioBuilder()
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(100);

			Assert.Throws<ArgumentException>(() => builder.Build());
		}

		[Test]
		public void BuildPortfolio_MissingPriceForInitialStock_ThrowsArgumentException()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 }, { "EL", 30 } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.21m }, { "FP", 0.3m }, { "EL", 0.5m }  };

			var builder = new PortfolioBuilder()
				.UseStocks(new[] { new Stock("SNG") } )
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(100);

			Assert.Throws<ArgumentException>(() => builder.Build());
		}

		[Test]
		public void BuildPortfolio_InitialStocks_EnoughAvailableAmount_NewStocksAreAdded()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 }, { "EL", 30 } };

			var stocks = new[]
			{
				new Stock("TLV") { Count = 2, Price = 10, Weight = 0.2m }, // should add 2
				new Stock("FP") { Count = 1, Price = 20, Weight = 0.2m }, // should add 3
				new Stock("EL") { Count = 2, Price = 30, Weight = 0.6m } // should add 2
			};

			var targetWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.3m }, { "EL", 0.5m } }; // target stock 4 * TLV + 4 * FP + 4 * EL = 240

			var portfolio = new PortfolioBuilder()
				.UsePrices(prices)
				.UseStocks(stocks)
				.UseWeightAdjustmentStrategy(new FollowTargetAdjustmentStrategy())
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(140)
				.Build();

			Assert.That(portfolio["EL"].Count, Is.EqualTo(4));
			Assert.That(portfolio["FP"].Count, Is.EqualTo(4));
			Assert.That(portfolio["TLV"].Count, Is.EqualTo(4));

			Assert.That(portfolio.TotalValue, Is.EqualTo(240));
		}

		[Test]
		public void BuildPortfolio_MinOrderValueIsSet_StockTotalValueLessThanMinimal_StockIsNotAddedToPortfolio()
		{
			var prices = new StockPrices
			{ { "TLV", 10 }, { "FP", 20 }, { "EL", 30 } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.3m }, { "EL", 0.5m }  };

			var portfolio = new PortfolioBuilder()
				.UsePrices(prices)
				.UseTargetWeights(targetWeights)
				.UseToBuyAmount(140)
				.UseMinOrderValue(31)
				.Build();

			Assert.That(portfolio.Any(s => s.Symbol == "TLV"), Is.False);
		}
	}
}