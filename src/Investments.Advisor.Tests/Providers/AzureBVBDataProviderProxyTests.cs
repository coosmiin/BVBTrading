﻿using System;
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

			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("FP", result[0].Symbol);
			Assert.AreEqual(1.345, result[0].Price);
			Assert.AreEqual(0.2305, result[0].Weight);
		}

		[Test]
		public void GetBvbStocksAsync_InvalidCase_ThrowsException()
		{
			var httpClient = BuildHttpClient(File.ReadAllText(@"TestData/bvb-index.pascal-case.json"));

			var provider = new AzureBvbDataProviderProxy(httpClient, string.Empty);

			Assert.ThrowsAsync<InvalidBvbDataException>(async () => await provider.GetBvbStocksAsync("BET"));
		}

		[Test]
		public void GetBvbStocksAsync_EmptyArrayResult_ThrowsException()
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
