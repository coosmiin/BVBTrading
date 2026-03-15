using Investments.Domain.Stocks;
using NUnit.Framework;
using System;

namespace Investments.Domain.Tests.Stocks
{
	public class StockTests
	{
		[Test]
		public void Addition_DifferentSymbols_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => { _ = new Stock("TLV") + new Stock("FP"); });
		}

		[Test]
		public void Addition_StockCountsAreCorrectlyAdded()
		{
			Assert.That((new Stock("FP") { Count = 5 } + new Stock("FP") { Count = 2 }).Count, Is.EqualTo(7));
		}

		[Test]
		public void Addition_PricesAreDifferent_SecondPriceIsLeading()
		{
			Assert.That((new Stock("FP") { Price = 5 } + new Stock("FP") { Price = 2 }).Price, Is.EqualTo(2));
		}

		[Test]
		public void Addition_WeightIsReset()
		{
			Assert.That((new Stock("FP") { Weight = 0.2m } + new Stock("FP") { Weight = 0.3m }).Weight, Is.EqualTo(0));
		}

		[Test]
		public void UnarySubstraction_StockCountIsDecreasedByOne()
		{
			var stock = new Stock("FP") { Count = 3 };
			Assert.That(--stock.Count, Is.EqualTo(2));
		}

	}
}
