using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using WebGrease.Extensions;

namespace UploadCenter.Database
{
	public class User
	{
		[Key]
		public virtual int UserID { get; set; }

		public virtual string UserName { get; set; }

		public virtual string Password { get; set; }

		public virtual string Email { get; set; }

		public virtual bool IsAdmin { get; set; }

	}
}