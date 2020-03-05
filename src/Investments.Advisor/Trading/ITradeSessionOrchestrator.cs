﻿using Investments.Domain.Stocks;
using System.Threading.Tasks;

namespace Investments.Advisor.Trading
{
	public interface ITradeSessionOrchestrator
	{
		Task<Stock[]> GetBETStocksAsync();
	}
}