
using System;
using AzureLibrary.AzureStorage;
using NUnit.Framework;

namespace TestDocArc
{
	[TestFixture]
	public class TestAzureStorage
	{
		private const string ACCOUNT_NAME = "docarc";
		private const string ACCOUNT_KEY = "whjSzQN0JTSaXRPvlfR0CeMTrjTGSpKQGnDNPrSwl5Hi91RGplXxa7vEqpYUF6keM+ZRIqqVyQZrUBAu3jeFHg==";

			private const string CONTAINER_NAME = "testdocarc";


		[Test]
		public async void ConnectToAzureStorage()
		{
			AzureStorageManager mng = new AzureStorageManager(ACCOUNT_NAME, ACCOUNT_KEY, CONTAINER_NAME, false);

			Assert.True(await mng.ServiceIsReachable());
		}

		[Test]
		public async void ConnectToAzureStorageSAS()
		{
			AzureStorageManager mng = new AzureStorageManager(ACCOUNT_NAME, ACCOUNT_KEY, CONTAINER_NAME, true);

			Assert.True(await mng.ServiceIsReachable());
		}
	}
}
