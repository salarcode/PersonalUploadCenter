using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace UploadCenter.Database
{
	public class UploadDb : DbContext
	{
		public UploadDb()
			: base("UploadDbConnection")
		{
		}

		public static void CreateIfNotExists()
		{
			using (var db = new UploadDb())
			{
				db.Database.CreateIfNotExists();
			}
		}

		public DbSet<User> Users { get; set; }
		public DbSet<UploadedFile> Files { get; set; }

	}
}