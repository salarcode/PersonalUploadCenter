using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using UploadCenter.Database;

namespace UploadCenter.Classes
{
	public class UserManager
	{
		private const string UserContextName = "_SysUserContext";

		public static bool SignedIn()
		{
			var context = HttpContext.Current;
			return context.User.Identity.IsAuthenticated;
		}

		public static string GetUsername()
		{
			var user = GetUser();
			if (user != null)
				return user.UserName;
			return "";
		}
		public static bool UserIsAdmin()
		{
			var user = GetUser();
			if (user != null)
				return user.IsAdmin;
			return false;
		}
		public static int? GetUserId()
		{
			var user = GetUser();
			if (user != null)
				return user.UserID;
			return null;
		}

		/// <summary>
		/// Signouts out of the system and removes the cached items
		/// </summary>
		public static void SignOut()
		{
			FormsAuthentication.SignOut();

			var context = HttpContext.Current;
			var session = context.Session;
			var useSession = session.Mode != SessionStateMode.Off;

			if (useSession)
			{
				session.Remove(UserContextName);
			}
			else
			{
				context.Items.Remove(UserContextName);
			}
		}
		/// <summary>
		/// Signouts out of the system and removes the cached items
		/// </summary>
		public static void SignOut(HttpContext context)
		{
			FormsAuthentication.SignOut();
			var session = context.Session;
			var useSession = session.Mode != SessionStateMode.Off;

			if (useSession)
			{
				session.Remove(UserContextName);
			}
			else
			{
				context.Items.Remove(UserContextName);
			}
		}


		public static User GetUser()
		{
			var context = HttpContext.Current;
			var session = context.Session;
			var useSession = session.Mode != SessionStateMode.Off;
			User ua;
			if (useSession)
			{
				ua = session[UserContextName] as User;
			}
			else
			{
				ua = context.Items[UserContextName] as User;
			}

			if (ua != null)
			{
				return ua;
			}

			if (!context.Request.IsAuthenticated)
			{
				return null;
			}
			var idString = context.User.Identity.Name;
			int id;

			if (!int.TryParse(idString, out id))
			{
				// invalid data in the authenticated user!
				FormsAuthentication.SignOut();
				return null;
			}

			using (var db = new UploadDb())
			{
				var useraAccount = db.Users.AsNoTracking()
					.Where(a => a.UserID == id)
					.Select(a => a)
					.FirstOrDefault();
				if (useraAccount != null)
				{
					ua = useraAccount;

					if (useSession)
						session[UserContextName] = ua;
					else
						context.Items[UserContextName] = ua;
					return ua;
				}
			}

			// invalid data in the authenticated user!
			return null;
		}


		public static bool BasicAuthorize(HttpContext context)
		{
			var req = context.Request;
			var res = context.Response;

			string auth = req.Headers["Authorization"];

			if (!String.IsNullOrEmpty(auth))
			{
				byte[] encodedDataAsBytes = Convert.FromBase64String(auth.Replace("Basic ", ""));
				string value = Encoding.ASCII.GetString(encodedDataAsBytes);
				string username = value.Substring(0, value.IndexOf(':'));
				string password = value.Substring(value.IndexOf(':') + 1);

				using (var db = new UploadDb())
				{
					var user = db.Users.FirstOrDefault(a => a.UserName == username && a.Password == password);
					if (user == null)
					{
						UnAuthorizationRequest(context);
						return false;
					}
					else
					{
						context.User = new GenericPrincipal(new GenericIdentity(user.UserID.ToString()), null);
						return true;
					}
				}
			}
			else
			{
				UnAuthorizationRequest(context);
				return false;
			}
		}

		public static void UnAuthorizationRequest(HttpContext context)
		{
			var res = context.Response;
			res.Clear();
			res.StatusDescription = "Unauthorized";
			res.AddHeader("WWW-Authenticate", "Basic realm=\"Please login to gain access to your private downloads\"");
			res.Write("401, Please login to gain access to your private downloads");
			res.StatusCode = 401;
			res.End();
		}
	}
}