using Investments.Domain.Stocks;
using Investments.Logic.Calculus;
using Investments.Logic.Weights;
using NUnit.Framework;
using System.Linq;

namespace Investments.Logic.Tests.Weights
{
	public class MinOrderValueCutOffStrategyTests
	{
		[Test]
		public void AdjustWeights_WeightsBellowMinimalWeightAreCutOff_CutOffWeightIsRedistributed()
		{
			var currentWeights = new StockWeights
			{ { "TLV", 0.08m }, { "SNG", 0.09m }, { "FP", 0.22m }, { "EL", 0.61m } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.08m }, { "SNG", 0.09m }, { "FP", 0.22m }, { "EL", 0.61m } };

			var strategy = new MinOrderValueCutOffStrategy(new FollowTargetAdjustmentStrategy(), 0.1m);
			var toBuyWeights = strategy.AdjustWeights(currentWeights, targetWeights, toBuyInverseRatio: 2);

			Assert.That(toBuyWeights.ContainsKey("TLV"), Is.False);
			Assert.That(toBuyWeights.Sum(w => w.Value).IsApproxOne(), Is.True);
		}
	}
}
