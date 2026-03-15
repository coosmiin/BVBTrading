using System.Linq;
using Investments.Domain.Stocks;
using Investments.Logic.Calculus;
using Investments.Logic.Weights;
using NUnit.Framework;

namespace Investments.Logic.Tests.Weights
{
	public class FollowTargetAdjustmentStrategyTests
	{
		[Test]
		public void AdjustWeights_EmptyPortfolio_ToBuyWeightsCorrectlyCalculated()
		{
			var currentWeights = new StockWeights();

			var targetWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.3m }, { "EL", 0.5m } };

			var strategy = new FollowTargetAdjustmentStrategy();
			var toBuyWeights = strategy.AdjustWeights(currentWeights, targetWeights, toBuyInverseRatio: 0);

			Assert.That(toBuyWeights["TLV"], Is.EqualTo(0.2m));
			Assert.That(toBuyWeights["FP"], Is.EqualTo(0.3m));
			Assert.That(toBuyWeights["EL"], Is.EqualTo(0.5m));
		}

		[Test]
		public void AdjustWeights_InverseToBuyRatioIsOne_ToBuyWeightsCorrectlyCalculated() // Simulates second buying sessions => inverseToBuyRatio = 1
		{
			var currentWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.2m }, { "EL", 0.6m } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.3m }, { "EL", 0.5m } };

			var strategy = new FollowTargetAdjustmentStrategy();
			var toBuyWeights = strategy.AdjustWeights(currentWeights, targetWeights, toBuyInverseRatio: 1);

			Assert.That(toBuyWeights["TLV"], Is.EqualTo(0.2m));
			Assert.That(toBuyWeights["FP"], Is.EqualTo(0.4m));
			Assert.That(toBuyWeights["EL"], Is.EqualTo(0.4m));
		}

		[Test]
		public void AdjustWeights_InverseToBuyRatioVeryHigh_ToBuyWeightsSumEqualsOneIsCorrectlyEnforced()
		{
			var currentWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.2m }, { "EL", 0.6m } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.3m }, { "EL", 0.5m } };

			var strategy = new FollowTargetAdjustmentStrategy();
			var toBuyWeights = strategy.AdjustWeights(currentWeights, targetWeights, toBuyInverseRatio: 10);

			Assert.That(toBuyWeights.Sum(w => w.Value).IsApproxOne(), Is.True);
		}

		[Test]
		public void AdjustWeights_TargetWeightsHasMoreSymbols_ToBuyWeightsCorrectlyCalculated()
		{
			var currentWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.2m }, { "EL", 0.6m } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.2m }, { "EL", 0.5m }, { "SNG", 0.1m } };

			var strategy = new FollowTargetAdjustmentStrategy();
			var toBuyWeights = strategy.AdjustWeights(currentWeights, targetWeights, toBuyInverseRatio: 2);

			Assert.That(toBuyWeights["TLV"], Is.EqualTo(0.2m));
			Assert.That(toBuyWeights["FP"], Is.EqualTo(0.2m));
			Assert.That(toBuyWeights["EL"], Is.EqualTo(0.3m));
			Assert.That(toBuyWeights["SNG"], Is.EqualTo(0.3m));
		}

		[Test]
		public void AdjustWeights_CurrentWeightHigherThanTargetWeight_WouldNormallyResultInNegativeWeight_ToBuyWeightIsRemoved()
		{
			var currentWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.2m }, { "EL", 0.6m } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.1m }, { "FP", 0.3m }, { "EL", 0.5m }, { "SNG", 0.1m } };

			var strategy = new FollowTargetAdjustmentStrategy();
			var toBuyWeights = strategy.AdjustWeights(currentWeights, targetWeights, toBuyInverseRatio: 2);

			Assert.That(toBuyWeights.ContainsKey("TLV"), Is.False);
		}
	}
}
