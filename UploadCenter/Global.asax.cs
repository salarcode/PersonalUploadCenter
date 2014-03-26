using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using UploadCenter;
using UploadCenter.Classes;
using UploadCenter.Database;

namespace UploadCenter
{
	public class Global : HttpApplication
	{
		void Application_Start(object sender, EventArgs e)
		{
			// Code that runs on application startup
			AuthConfig.RegisterOpenAuth();
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			BundleConfig.RegisterRoutes(BundleTable.Bundles);

			UploadDb.CreateIfNotExists();
			SetNoAdminFlag();

		}


		void SetNoAdminFlag()
		{
			using (var db = new UploadDb())
			{
				var hasAdmin = db.Users.Any(a => a.IsAdmin);
				if (!hasAdmin)
				{
					Application["HasAdmin"] = false;
				}
			}
		}

		void Application_EndRequest(object sender, System.EventArgs e)
		{
			Response.TrySkipIisCustomErrors = true;
			if ((Response.StatusCode == 302) && (!Request.IsAuthenticated))
			{
				if (Request.Path.ToLower().Contains("download.ashx"))
				{
					UserManager.UnAuthorizationRequest(Context);
				}
			}
		}

		void Application_End(object sender, EventArgs e)
		{
			//  Code that runs on application shutdown

		}

		void Application_Error(object sender, EventArgs e)
		{
			// Code that runs when an unhandled error occurs

		}
	}
}
