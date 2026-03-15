using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Investments.Advisor.AzureProxies;
using Investments.Advisor.Exceptions;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Investments.Advisor.Tests.Providers
{
	public class AzureBVBDataProviderProxyTests
	{
		[Test]
		public async Task GetBvbStocksAsync_ValidResponse_StocksAreCorrect()
		{
			var httpClient = BuildHttpClient(File.ReadAllText(@"TestData/bvb-index.json"));

			var provider = new AzureBvbDataProviderProxy(httpClient, string.Empty);

			var result = await provider.GetBvbStocksAsync("BET");

			Assert.That(result.Length, Is.EqualTo(2));
			Assert.That(result[0].Symbol, Is.EqualTo("FP"));
			Assert.That(result[0].Price, Is.EqualTo(1.345));
			Assert.That(result[0].Weight, Is.EqualTo(0.2305));
		}

		[Test]
		public async Task GetBvbStocksAsync_InvalidCase_ThrowsException()
		{
			var httpClient = BuildHttpClient(File.ReadAllText(@"TestData/bvb-index.pascal-case.json"));

			var provider = new AzureBvbDataProviderProxy(httpClient, string.Empty);

			Assert.ThrowsAsync<InvalidBvbDataException>(async () => await provider.GetBvbStocksAsync("BET"));
		}

		[Test]
		public async Task GetBvbStocksAsync_EmptyArrayResult_ThrowsException()
		{
			var httpClient = BuildHttpClient("[]");

			var provider = new AzureBvbDataProviderProxy(httpClient, string.Empty);

			Assert.ThrowsAsync<InvalidBvbDataException>(async () => await provider.GetBvbStocksAsync("BET"));
		}

		private HttpClient BuildHttpClient(string testData)
		{
			var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
			httpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(testData)
				});

			var httpClient = new HttpClient(httpMessageHandlerMock.Object);
			httpClient.BaseAddress = new Uri("http://localhost");

			return httpClient;
		}
	}
}

