using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace DriveQuickStart
{
	/// <remarks>
	/// helpful links
	/// 
	/// https://developers.google.com/drive/web/quickstart/quickstart-cs
	/// https://console.developers.google.com/project/494856075942/apiui/credential?clientType&authuser=0#
	/// http://stackoverflow.com/questions/tagged/google-drive-sdk
	/// </remarks>
	public class DriveCommandLineSample
	{
		public static void Main(string[] args)
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

			Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File
				{
					Title = "My document",
					Description = "A test document",
					MimeType = "text/plain"
				};

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
