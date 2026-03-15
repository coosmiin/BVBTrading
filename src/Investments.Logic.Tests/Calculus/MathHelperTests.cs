using Investments.Domain.Stocks;
using Investments.Logic.Calculus;
using NUnit.Framework;
using System.Linq;

namespace Investments.Logic.Tests.Calculus
{
	public class MathHelperTests
	{
		[Test]
		public void IsApproxOne_CloseEnough_ReturnsTrue()
		{
			Assert.That(MathHelper.IsApproxOne(1.02m), Is.True);
			Assert.That(MathHelper.IsApproxOne(0.98m), Is.True);
		}

		[Test]
		public void IsApproxOne_NotCloseEnough_ReturnsFalse()
		{
			Assert.That(MathHelper.IsApproxOne(1.12m), Is.False);
			Assert.That(MathHelper.IsApproxOne(0.88m), Is.False);
		}

		[Test]
		public void Redistribute_TotalWeightAlreadyOne_WeightsRemainUnchanged()
		{
			var weights = new StockWeights
			{
				{ "TLV", 0.3m }, { "FP", 0.6m }, { "EL", 0.1m }
			};

			weights = weights.Redistribute();

			Assert.That(weights["TLV"], Is.EqualTo(0.3));
			Assert.That(weights["FP"], Is.EqualTo(0.6));
			Assert.That(weights["EL"], Is.EqualTo(0.1));
		}

		[Test]
		public void Redistribute_TotalWeightLessThanOne_RedistributedWeightIsApproxOne()
		{
			var weights = new StockWeights
			{
				{ "TLV", 0.1m }, { "FP", 0.2m }, { "EL", 0.3m }
			};

			weights = weights.Redistribute();

			Assert.That(weights.Sum(w => w.Value).IsApproxOne(), Is.True);
		}
	}
}