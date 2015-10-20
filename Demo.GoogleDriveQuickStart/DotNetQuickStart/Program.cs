using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Nito.AsyncEx;

namespace DotNetQuickStart
{
	class Program
	{
		static string[] Scopes = { DriveService.Scope.DriveReadonly };
		static string ApplicationName = "Drive API .NET Quickstart";

		static void Main(string[] args)
		{
			AsyncContext.Run(() => RunExample());
		}

		private async static void RunExample()
		{
			UserCredential credential;

			using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
			{
				string credentialPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				credentialPath = Path.Combine(credentialPath, ".credentials/drive-dotnet-quickstart");

				credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credentialPath, true));
				Console.WriteLine("Credential file saved to: " + credentialPath);
			}

			// Create Drive API service.
			var service = new DriveService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			// Define parameters of request.
			FilesResource.ListRequest listRequest = service.Files.List();
			listRequest.MaxResults = 10;

			// List files.
			//IList<Google.Apis.Drive.v2.Data.File> files = listRequest.Execute().Items;
			FileList fileList = await listRequest.ExecuteAsync();
			IList<Google.Apis.Drive.v2.Data.File> files = fileList.Items;
			Console.WriteLine("Files:");
			if (files != null && files.Count > 0)
			{
				foreach (var file in files)
				{
					Console.WriteLine("{0} ({1})", file.Title, file.Id);
				}
			}
			else
			{
				Console.WriteLine("No files found.");
			}
			Console.Read();
		}
	}
}
