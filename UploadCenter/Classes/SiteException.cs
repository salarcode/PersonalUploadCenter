using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace UploadCenter.Classes
{
	public partial class SiteException
	{
		public static bool SiteExceptionsEnabled = true;

		public static void LogException(Exception ex)
		{
			LogException(ex, "", "");
		}
		public static void LogException(Exception ex, string errorMessage)
		{
			LogException(ex, errorMessage, "");
		}

		public static void LogException(Exception ex, string errorMessage, string errorKey)
		{
			if (!SiteExceptionsEnabled)
				return;

			if (ex is System.Threading.ThreadAbortException)
				return;

			if (errorMessage == null)
				errorMessage = "";

			if (errorKey == null)
				errorKey = "";

			StreamWriter exFile = null;
			try
			{
				var context = HttpContext.Current;
				string exceptionFile = GetExceptionResourceFile(context);
				exFile = System.IO.File.AppendText(exceptionFile);

				exFile.WriteLine();
				exFile.WriteLine("====================");
				exFile.WriteLine("Date time:     " + DateTime.Now.ToString());

				exFile.WriteLine("Error message: " + errorMessage);
				exFile.WriteLine("Error key:     " + errorKey);

				if (context != null)
				{
					HttpRequest req = context.Request;
					if (req != null)
					{
						if (req.UrlReferrer != null)
							exFile.WriteLine("Referrer url: " + req.UrlReferrer.ToString());
						exFile.WriteLine("Current url:   " + req.Url.ToString());
						exFile.WriteLine("UserHostAddress: " + req.UserHostAddress);
						exFile.WriteLine("UserAgent:     " + req.UserAgent);
						exFile.WriteLine("HttpMethod:    " + req.HttpMethod);
						exFile.WriteLine("HTTP_COOKIE:    " + getHeader(req, "HTTP_COOKIE"));

						if (ex != null && (ex is System.Data.SqlClient.SqlException || ex.Message.ToLower().LastIndexOf("does not exist") != -1))
						{
							exFile.WriteLine("Params data==>");
							exFile.WriteLine("HTTP_CONTENT_TYPE: " + getHeader(req, "HTTP_CONTENT_TYPE"));
							exFile.WriteLine("HTTP_ACCEPT_LANGUAGE: " + getHeader(req, "HTTP_ACCEPT_LANGUAGE"));
							exFile.WriteLine("HTTP_CONNECTION: " + getHeader(req, "HTTP_CONNECTION"));

							exFile.WriteLine("");

							if (req.Form.Count > 0)
							{
								exFile.WriteLine("Forms data-->");

								for (int i = 0; i < req.Form.Count; i++)
								{
									string key = req.Form.Keys[i];
									exFile.WriteLine(key + " --> " + req.Form[key]);
								}
							}
						}


						if (req.IsAuthenticated)
						{
							exFile.WriteLine("Identity Name: " + HttpContext.Current.User.Identity.Name);
						}
					}
				}

				exFile.WriteLine("Error details: ");
				if (ex != null)
					exFile.Write(ex.ToString());
				else
					exFile.Write("No details, exception is NULL");
			}
			catch
			{
			}
			finally
			{
				if (exFile != null)
					exFile.Close();
			}
		}

		static string getHeader(HttpRequest req, string header)
		{
			string h = req.Params[header];
			if (h != null) return h;
			else return "";
		}

		public static string GetExceptionResourceFile(HttpContext context)
		{
			const long maxSize = 512 * 1024;
			string result = "",
				tmp = context.Server.MapPath("~/App_Data/");

			int index = 0;
			FileInfo info;
			do
			{
				result = Path.Combine(tmp, string.Format("SiteException-{0}.log", index));

				info = new FileInfo(result);
				index++;

			} while (info.Exists && info.Length > maxSize);

			return result;
		}


	}
}
