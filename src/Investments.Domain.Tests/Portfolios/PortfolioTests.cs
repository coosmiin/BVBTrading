using Investments.Domain.Portfolios;
using Investments.Domain.Stocks;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Investments.Domain.Tests.Portfolios
{
	public class PortfolioTests
	{
		[Test]
		public void Ctor_NullInitialStock_IsEmpty()
		{
			var portfolio = new Portfolio();

			Assert.That(portfolio.Any(), Is.False);
		}

		[Test]
		public void Ctor_WithInitialStock_IsInitializedCorrectly()
		{
			var portfolio = new Portfolio(new[] { new Stock("FP") { Count = 100, Price = 10 } });

			Assert.That(portfolio["FP"].Count, Is.EqualTo(100));
		}

		[Test]
		public void TotalValue_EmptyPortfolio_IsZero()
		{
			Assert.That(new Portfolio().TotalValue, Is.EqualTo(0));
		}

		[Test]
		public void TotalValue_NonEmptyPortfolio_IsCorrect()
		{
			var portfolio = new Portfolio(
				new[]
				{
					new Stock("TVL") { Price = 10, Count = 2, Weight = 49 },
					new Stock("TVL") { Price = 3, Count = 7, Weight = 51 }
				});

			Assert.That(portfolio.TotalValue, Is.EqualTo(41));
		}

		[Test]
		public void RecalculateWeights_WithInitialStocks_WeightsAreCorrect()
		{
			var portfolio = new Portfolio(new[]
			{
				new Stock("EL") { Count = 1, Price = 30 },
				new Stock("FP") { Count = 1, Price = 20 },
				new Stock("TLV") { Count = 1, Price = 10 }
			});

			Assert.That(portfolio["EL"].Weight, Is.EqualTo(0.5m));
			Assert.That(portfolio["FP"].Weight, Is.EqualTo(0.33m));
			Assert.That(portfolio["TLV"].Weight, Is.EqualTo(0.17m));
		}

		[Test]
		public void RecalculateWeights_StocksAddedSequentially_WeightsAreCorrectlyCaclulatedOnlyOnce()
		{
			var portfolioMock = new Mock<Portfolio> { CallBase = true };

			portfolioMock.Object.AddStock(new Stock("EL") { Count = 1, Price = 30 });
			portfolioMock.Object.AddStock(new Stock("FP") { Count = 1, Price = 20 });
			portfolioMock.Object.AddStock(new Stock("TLV") { Count = 1, Price = 10 });

			Assert.That(portfolioMock.Object["EL"].Weight, Is.EqualTo(0.5m));
			Assert.That(portfolioMock.Object["FP"].Weight, Is.EqualTo(0.33m));
			Assert.That(portfolioMock.Object["TLV"].Weight, Is.EqualTo(0.17m));

			portfolioMock.Verify(p => p.RecalculateWeights(), Times.Once);
		}

		[Test]
		public void AddStock_NoCurrentStock_StockIsCorrectlyAdded()
		{
			var portfolio = new Portfolio();

			portfolio.AddStock(new Stock("FP") { Count = 10, Price = 10 });

			Assert.That(portfolio["FP"].Count, Is.EqualTo(10));
		}

		[Test]
		public void AddStock_WithCurrentStock_StockIsCorrectlyAdded()
		{
			var portfolio = new Portfolio(new[] { new Stock("FP") { Count = 2, Price = 10 } });

			portfolio.AddStock(new Stock("FP") { Count = 10, Price = 20 });

			Assert.That(portfolio["FP"].Count, Is.EqualTo(12));
		}

		[Test]
		public void GetEnumerator_NoIndexerCalled_WeightsAreRefreshed()
		{
			var portfolio = new Portfolio(new[]
{
				new Stock("EL") { Count = 1, Price = 30 },
				new Stock("FP") { Count = 1, Price = 20 },
				new Stock("TLV") { Count = 1, Price = 10 }
			});

			Assert.That(portfolio.Any(s => s.Weight == 0), Is.False);
		}
	}
}
