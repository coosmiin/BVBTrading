using Investments.Advisor.Trading;
using Investments.Domain.Stocks;
using Investments.Utils.Linq.Extensions;
using Investments.Utils.Serialization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Investments.Advisor.AzureProxies
{
	public class AzureTradeAdvisorProxy : ITradeAdvisor
	{
		private const string FUNCTION_URI_FORMAT = "api/calculateToBuyStocks?code={0}";

		private readonly HttpClient _httpClient;
		private readonly string _functionUri;

		public AzureTradeAdvisorProxy(HttpClient httpClient, string functionKey)
		{
			_httpClient = httpClient;
			_functionUri = string.Format(FUNCTION_URI_FORMAT, functionKey);
		}

		public async Task<Stock[]> CalculateToBuyStocksAsync(decimal toBuyAmount, Stock[] existingStocks, Stock[] bvbStocks)
		{
			var payload = new
			{
				ToBuyAmount = toBuyAmount,
				ExistingStocks = existingStocks,
				BvbStocks = bvbStocks
			};

			var result =
				await _httpClient.PostAsync(
					_functionUri,
					new StringContent(JsonSerializerHelper.Serialize(payload), Encoding.UTF8, "application/json"));

			var stocks = JsonSerializerHelper.Deserialize<Stock[]>(await result.Content.ReadAsStringAsync());
			
			return stocks.OrEmpty();
		}
	}
}
