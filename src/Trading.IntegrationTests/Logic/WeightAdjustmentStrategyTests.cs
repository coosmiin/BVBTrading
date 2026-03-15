using Investments.Logic.Calculus;
using Investments.Logic.Weights;
using NUnit.Framework;
using System.Linq;
using Trading.IntegrationTests.TestData;

namespace Trading.IntegrationTests.Logic
{
	public class WeightAdjustmentStrategyTests
	{
		private const int MIN_ORDER_VALUE = 250;

		[Test]
		public void FollowTargetAdjustmentStrategy_AsExpected()
		{
			var toBuyAmount = 2000m;
			var (currentWeights, targetWeights, portfolioValue) = TestResources.ReadWeights();

			var strategy = new FollowTargetAdjustmentStrategy();

			var adjustedWeights = 
				strategy.AdjustWeights(currentWeights, targetWeights, portfolioValue / toBuyAmount);

			Assert.That(adjustedWeights.Sum(w => w.Value).IsApproxOne(), Is.True);
		}

		[Test]
		public void MinOrderValueCutOffStrategy_AsExpected()
		{
			var toBuyAmount = 2070.2175m;
			var (currentWeights, targetWeights, portfolioValue) = TestResources.ReadWeights();

			var strategy = new MinOrderValueCutOffStrategy(new FollowTargetAdjustmentStrategy(), MIN_ORDER_VALUE / toBuyAmount);

			var adjustedWeights =
				strategy.AdjustWeights(currentWeights, targetWeights, portfolioValue / toBuyAmount);

			Assert.That(adjustedWeights.Sum(w => w.Value).IsApproxOne(), Is.True);
		}
	}
}