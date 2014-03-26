using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using UploadCenter.Classes;

namespace UploadCenter.Account
{
	/// <summary>
	/// Summary description for Signout
	/// </summary>
	public class Signout : IHttpHandler, IRequiresSessionState
	{

		public void ProcessRequest(HttpContext context)
		{
			UserManager.SignOut();
			context.Response.Redirect("Login.aspx");
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}
	}
}