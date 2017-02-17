using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureLibrary.AzureStorage
{
	public class AzureStorageManager 
	{
		private string _accountName;
		private string _accountKey;

		private CloudStorageAccount _account;

		private CloudBlobClient _client;

		private CloudBlobContainer _container;

		public AzureStorageManager(string accountName, string accountKey, string containerName, bool useSAS = true) 
		{
			this._accountName = accountName;
			this._accountKey = accountKey;

			if (!useSAS)
			{
				_account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=" + accountName.ToLower() + ";AccountKey=" + accountKey);

			}
			else
			{
				var cred = new StorageCredentials(GetAccountSASToken());
				_account = new CloudStorageAccount(cred, accountName, endpointSuffix: null, useHttps: true);
			}

			Init(containerName);
		}

		public async Task<bool> CreateContainerIfNotExists()
		{
			return await _container.CreateIfNotExistsAsync();
		}

		public async Task<bool> ContainerExists()
		{
			return await _container.ExistsAsync();
		}

		public async Task<string> UploadBlob(Stream fileStream)
		{
			try
			{
				var blob = await GetBlobReference(CreateBlobName());

				await blob.UploadFromStreamAsync(fileStream);

				return blob.Name;
			}
			catch (Exception ex)
			{
				throw new Exception("Error while uploading blob!", ex);
			}
		}

		public async Task<string> UploadBlob(byte[] byteArray)
		{
			try
			{
				var blob = await GetBlobReference(CreateBlobName());

				await blob.UploadFromByteArrayAsync(byteArray, 0, 0);

				return blob.Name;
			}
			catch (Exception ex)
			{
				throw new Exception("Error while uploading blob!", ex);
			}
		}

		public async Task<string> UploadBlob(string text)
		{
			try
			{
				var blob = await GetBlobReference(CreateBlobName());

				await blob.UploadTextAsync(text);

				return blob.Name;
			}
			catch (Exception ex)
			{
				throw new Exception("Error while uploading blob!", ex);
			}
		}

		public async Task<bool> DeleteBlob(String blobName)
		{
			try
			{
				var blob = await GetBlobReference(blobName.ToLower());

				if (!await blob.ExistsAsync())
					throw new Exception("Blob reference '" + blobName + "' does not exists!");

				return await blob.DeleteIfExistsAsync();
			}
			catch (Exception ex)
			{
				throw new Exception("Error while deleting blob!", ex);
			}
		}

		private void Init(string containerName)
		{
			_client = _account.CreateCloudBlobClient();

			if (!CheckContainerName(containerName))
				throw new Exception("Containername is not valid!");

			_container = _client.GetContainerReference(containerName.ToLower());
		}

		private async Task<CloudBlockBlob> GetBlobReference(string blobName)
		{
			await CreateContainerIfNotExists();

			return _container.GetBlockBlobReference(blobName);
		}

		private String CreateBlobName()
		{
			return Guid.NewGuid().ToString();
		}

		public async Task<bool> ServiceIsReachable()
		{
			return await _container.ExistsAsync();
		}

		private bool CheckContainerName(string containerName)
		{
			return (!String.IsNullOrEmpty(containerName) && containerName.Length < 63);
		}

		private string GetAccountSASToken()
		{
			string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=" + _accountName + ";AccountKey=" + _accountKey;
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

			SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
			{
				Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write | SharedAccessAccountPermissions.List,
				Services = SharedAccessAccountServices.Blob | SharedAccessAccountServices.File,
				ResourceTypes = SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object | SharedAccessAccountResourceTypes.Service,
				SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
				Protocols = SharedAccessProtocol.HttpsOnly
			};

			return storageAccount.GetSharedAccessSignature(policy);
		}
	}
}
