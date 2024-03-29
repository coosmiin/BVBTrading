﻿using System.Linq;
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

			Assert.AreEqual(0.2m, toBuyWeights["TLV"]);
			Assert.AreEqual(0.3m, toBuyWeights["FP"]);
			Assert.AreEqual(0.5m, toBuyWeights["EL"]);
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

			Assert.AreEqual(0.2m, toBuyWeights["TLV"]);
			Assert.AreEqual(0.4m, toBuyWeights["FP"]);
			Assert.AreEqual(0.4m, toBuyWeights["EL"]);
		}

		[Test]
		public void AdjustWeights_InverseToBuyRatioVeryHigh_ToBuyWeightsSumEqualsOneIsCorrecltyEnforced()
		{
			var currentWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.2m }, { "EL", 0.6m } };

			var targetWeights = new StockWeights
			{ { "TLV", 0.2m }, { "FP", 0.3m }, { "EL", 0.5m } };

			var strategy = new FollowTargetAdjustmentStrategy();
			var toBuyWeights = strategy.AdjustWeights(currentWeights, targetWeights, toBuyInverseRatio: 10);

			Assert.IsTrue(toBuyWeights.Sum(w => w.Value).IsApproxOne());
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

			Assert.AreEqual(0.2m, toBuyWeights["TLV"]);
			Assert.AreEqual(0.2m, toBuyWeights["FP"]);
			Assert.AreEqual(0.3m, toBuyWeights["EL"]);
			Assert.AreEqual(0.3m, toBuyWeights["SNG"]);
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

			Assert.IsFalse(toBuyWeights.ContainsKey("TLV"));
		}
	}
}
