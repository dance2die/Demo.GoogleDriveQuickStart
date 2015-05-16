using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
			return View();
		}
	}
}