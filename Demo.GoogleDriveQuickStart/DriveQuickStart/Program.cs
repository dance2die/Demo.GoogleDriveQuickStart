using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace DriveQuickStart
{
	/// <remarks>https://developers.google.com/drive/web/quickstart/quickstart-cs</remarks>
	class DriveCommandLineSample
	{
		static void Main(string[] args)
		{
			UserCredential credential;
			const string secreteJson = "client_secrets.json";
			using (var fs = new FileStream(secreteJson, FileMode.Open, FileAccess.Read))
			{
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(fs).Secrets,
					new[] { DriveService.Scope.Drive },
					"user",
					CancellationToken.None,
					new FileDataStore("DriveCommandLineSample")).Result;
			}

			// Create the service.
			var service = new DriveService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = "Drive API Sample",
			});

			Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
			body.Title = "My document";
			body.Description = "A test document";
			body.MimeType = "text/plain";

			byte[] byteArray = System.IO.File.ReadAllBytes("document.txt");
			System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

			FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, "text/plain");
			request.Upload();

			Google.Apis.Drive.v2.Data.File file = request.ResponseBody;
			Console.WriteLine("File id: " + file.Id);
			Console.WriteLine("Press Enter to end this process.");
			Console.ReadLine();
		}
	}
}
