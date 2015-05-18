using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v2.Data.File;

namespace Demo.GoogleDriveQuickStart.Web.Controllers
{
	public class HomeController : Controller
	{
		/// <summary>
		/// Display Top Level Google Drive Folders
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			UserCredential credential = GetCredential();
			DriveService service = GetService(credential);
			List<File> files = GetFiles(service);

			return View(files);
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
			const string secreteJson = @"~\Content\client_secrets.json";
			var path = System.Web.HttpContext.Current.Server.MapPath(secreteJson);
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(fs).Secrets,
					new[] { DriveService.Scope.Drive },
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