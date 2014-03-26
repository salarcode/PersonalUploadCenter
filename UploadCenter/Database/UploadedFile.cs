using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UploadCenter.Database
{
	public partial class UploadedFile
	{
		[Key]
		public virtual int UploadedFileID { get; set; }

		public virtual string Filename { get; set; }

		/// <summary>
		/// In bytes
		/// </summary>
		public virtual long FileSize { get; set; }

		public virtual string Extension { get; set; }
		public virtual string Comment { get; set; }
		public virtual DateTime UploadDate { get; set; }

		public virtual DateTime? LastDownload { get; set; }

		public virtual int Downloaded { get; set; }

		public virtual bool IsPublic { get; set; }

		public virtual int? UserId { get; set; }

		/// <summary>
		/// The uploader
		/// </summary>
		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}