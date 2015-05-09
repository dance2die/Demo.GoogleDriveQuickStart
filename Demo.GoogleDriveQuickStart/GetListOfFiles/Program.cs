using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
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

		}

		private static UserCredential GetCredential()
		{
			UserCredential credential;
			const string secreteJson = "client_secrets.json";
			using (var fs = new FileStream(secreteJson, FileMode.Open, FileAccess.Read))
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
			var service = new DriveService(new BaseClientService.Initializer()
				{
				HttpClientInitializer = credential,
				ApplicationName = "Drive API Sample",
				});
			return service;
		}
	}
}
