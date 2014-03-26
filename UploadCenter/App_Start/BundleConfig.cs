using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace UploadCenter
{
	public class BundleConfig
	{
		public static void RegisterRoutes(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/lib").Include(
				"~/Scripts/jquery-1.11.0.min.js",
				"~/Scripts/modernizr-2.7.2.js",
				"~/Scripts/bootstrap.min.js",
				"~/Scripts/moment.min.js"));

			bundles.Add(new ScriptBundle("~/bundles/legacy").Include(
				"~/Scripts/respond.min.js",
				"~/Scripts/"));

			bundles.Add(new StyleBundle("~/content/css").Include(
				"~/Content/site.css",
				"~/Content/"));

			bundles.Add(new StyleBundle("~/content/boot/css").Include(
				"~/Content/bootstrap/bootstrap.Cerulean.min.css",
				"~/Content/"));


		}
	}
}