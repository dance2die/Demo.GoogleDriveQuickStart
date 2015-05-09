using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GetListOfFiles
{
	public class Program
	{
		public static void Main(string[] args)
		{
			UserCredential credential = GetCredential();
			DriveService service = GetService(credential);
			var files = GetFiles(service);
			//DumpFiles(files);
			DumpFolders(files);

			Console.ReadLine();
		}

		private static void DumpFolders(IEnumerable<File> files)
		{
			foreach (var file in files.OrderBy(f => f.Title))
			{
				if (IsFolder(file))
					Console.WriteLine("Title: {0}", file.Title);
			}
		}

		private static void DumpFiles(IEnumerable<File> files)
		{
			foreach (File file in files)
			{
				Console.WriteLine(file);
				Console.WriteLine("Title: {0}; ", file.Title);
			}
		}

		private static bool IsFolder(File file)
		{
			const string folderMimeType = "application/vnd.google-apps.folder";
			return string.Compare(file.MimeType, folderMimeType, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		/// <summary>
		/// Retrieve a list of File resources.
		/// </summary>
		/// <param name="service">Drive API service instance.</param>
		/// <returns>List of File resources.</returns>
		public static List<File> GetFiles(DriveService service)
		{
			List<File> result = new List<File>();
			FilesResource.ListRequest request = service.Files.List();

			do
			{
				try
				{
					FileList files = request.Execute();

					result.AddRange(files.Items);
					request.PageToken = files.NextPageToken;
				}
				catch (Exception e)
				{
					Console.WriteLine("An error occurred: " + e.Message);
					request.PageToken = null;
				}
			} while (!String.IsNullOrEmpty(request.PageToken));
			return result;
		}

		private static UserCredential GetCredential()
		{
			UserCredential credential;
			const string secreteJson = "client_secrets.json";
			using (var fs = new System.IO.FileStream(
				secreteJson, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(fs).Secrets,
					new[] {DriveService.Scope.Drive},
					"user",
					CancellationToken.None,
					new FileDataStore("DriveCommandLineSample")).Result;
			}
			return credential;
		}

		private static DriveService GetService(UserCredential credential)
		{
			var service = new DriveService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = "Get list of Google Drive Files",
			});
			return service;
		}
	}
}
