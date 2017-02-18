
using System;
using System.IO;
using System.Threading;
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
			AzureStorageManager mng = new AzureStorageManager(ACCOUNT_NAME, ACCOUNT_KEY, false);

			await mng.InitContainer(CONTAINER_NAME);

			Assert.True(await mng.ContainerIsReachable());
		}

		[Test]
		public async void ConnectToAzureStorageSAS()
		{
			AzureStorageManager mng = new AzureStorageManager(ACCOUNT_NAME, ACCOUNT_KEY, true);

			await mng.InitContainer(CONTAINER_NAME);

			Assert.True(await mng.ContainerIsReachable());
		}

		[Test]
		public async void WriteBlobToAzureStorage()
		{
			AzureStorageManager mng = new AzureStorageManager(ACCOUNT_NAME, ACCOUNT_KEY, true);

			await mng.InitContainer(CONTAINER_NAME);

			Assert.True(await mng.ContainerIsReachable());

			Assert.True(await mng.ContainerExists());

			string blobName = await mng.UploadBlob("TEST TEST TEST");

			Assert.True(!String.IsNullOrEmpty(blobName));
		}

		[Test]
		public async void ReadBlobFromAzureStorage()
		{
			AzureStorageManager mng = new AzureStorageManager(ACCOUNT_NAME, ACCOUNT_KEY, true);

			await mng.InitContainer(CONTAINER_NAME);

			Assert.True(await mng.ContainerIsReachable());

			Assert.True(await mng.ContainerExists());


			string text = string.Empty;

			using (MemoryStream stream = await mng.DownloadBlob("fdf39c19-4d1c-46de-8702-971c5fd3ed69")){
				text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
			}

			Assert.True(!String.IsNullOrEmpty(text));
		}

		[Test]
		public async void DeleteBlobFromAzureStorage()
		{
			AzureStorageManager mng = new AzureStorageManager(ACCOUNT_NAME, ACCOUNT_KEY, true);

			await mng.InitContainer(CONTAINER_NAME);

			string blobName = await mng.UploadBlob("TEST TEST TEST");

			Assert.True(!String.IsNullOrEmpty(blobName));

			Assert.True(await mng.DeleteBlob(blobName));

			Assert.False(await mng.BlobExists(blobName));
		}

		[Test]
		public async void DeleteContainerFromAzureStorage()
		{
			AzureStorageManager mng = new AzureStorageManager(ACCOUNT_NAME, ACCOUNT_KEY, true);

			await mng.InitContainer("TestContainerForDelete");

			Assert.True(await mng.ContainerExists());

			Thread.Sleep(2000);

			Assert.True(await mng.DeleteContainer());
		}
	}
}
