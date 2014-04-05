using System;
using System.IO;

namespace Salar.ResumableDownload
{
	/// <summary>
	/// Rseumable download information
	/// </summary>
	/// <authors>
	/// Salar Khalilzadeh
	/// http://www.salarcode.com
	/// http://blog.salarcode.com
	/// https://salarblogsources.svn.codeplex.com/svn/Projects/Salar.ResumableDownload/
	/// </authors>
	public class DownloadDataInfo : IDisposable
	{
		public const string MimeStreamData = "application/octet-stream";
		private bool _streamCreated = false;

		/// <summary>
		/// The not throttled stream
		/// </summary>
		private Stream _originalStream = null;

		public DownloadDataInfo(string fileName)
		{
			DataId = Guid.NewGuid();
			var info = new FileInfo(fileName);
			PhysicalFileAvailable = true;
			PhysicalFileName = fileName;
			ContentLength = info.Length;
			LastWriteTimeUtc = info.LastWriteTimeUtc;
			ContentType = MimeStreamData;
			EntityTag = fileName.GetHashCode().ToString();

			_streamCreated = true;
			DataStream = File.OpenRead(fileName);
			_originalStream = DataStream;
			ApplyRangeToStream = true;

			RequestHttpMethod = "GET";
			RangeBegin = new long[] { 0 };
			RangeEnd = new long[] { ContentLength - 1 };
			IsRangeRequest = false;
			DisplayFileName = Path.GetFileName(fileName);
		}

		public DownloadDataInfo(Stream dataStream, string displayFileName)
		{
			DataId = Guid.NewGuid();
			if (dataStream.CanSeek)
				ContentLength = dataStream.Length;
			else
				ContentLength = -1;
			PhysicalFileAvailable = false;
			PhysicalFileName = string.Empty;
			LastWriteTimeUtc = DateTime.Now;
			ContentType = MimeStreamData;
			EntityTag = displayFileName.GetHashCode().ToString();

			_streamCreated = false;
			DataStream = dataStream;
			_originalStream = DataStream;
			ApplyRangeToStream = true;

			RequestHttpMethod = "GET";
			RangeBegin = new long[] { 0 };
			RangeEnd = new long[] { ContentLength - 1 };
			IsRangeRequest = false;
			DisplayFileName = Path.GetFileName(displayFileName);
		}

		public DownloadDataInfo(byte[] dataBytes, string displayFileName)
		{
			DataId = Guid.NewGuid();
			PhysicalFileAvailable = false;
			PhysicalFileName = string.Empty;
			ContentLength = dataBytes.Length;
			LastWriteTimeUtc = DateTime.Now;
			ContentType = MimeStreamData;
			EntityTag = displayFileName.GetHashCode().ToString();

			_streamCreated = true;
			DataStream = new MemoryStream(dataBytes);
			_originalStream = DataStream;
			ApplyRangeToStream = true;

			RequestHttpMethod = "GET";
			RangeBegin = new long[] { 0 };
			RangeEnd = new long[] { ContentLength - 1 };
			IsRangeRequest = false;
			DisplayFileName = Path.GetFileName(displayFileName);
		}

		public void Dispose()
		{
			Disposed = true;
			if (_streamCreated && DataStream != null)
			{
				DataStream.Close();
				DataStream.Dispose();
			}
			DataStream = null;
			if (_streamCreated && _originalStream != null)
			{
				_originalStream.Dispose();
			}
			_originalStream = null;


			// raise the events!
			OnDisposed();
		}

		/// <summary>
		/// When this part finished
		/// </summary>
		internal event Action<DownloadDataInfo> Finished;

		private void OnDisposed()
		{
			if (Finished != null) 
				Finished(this);
		}

		/// <summary>
		/// Throttles for the specified buffer size in bytes.
		/// </summary>
		/// <param name="maximumBytesPerSecond">The maximum bytes per second that can be transferred through the base stream. Specify zero to disable this feature.</param>
		public void LimitTransferSpeed(long maximumBytesPerSecond)
		{
			if (DataStream == null)
			{
				return;
				//throw new ArgumentNullException("DataStream");
			}
			if (DataStream is ThrottledStream)
			{
				((ThrottledStream)DataStream).MaximumBytesPerSecond = maximumBytesPerSecond;
			}
			else
			{
				DataStream = new ThrottledStream(DataStream, maximumBytesPerSecond);
			}
		}

		public void InitializeRanges(HttpRequestSimpleRangeInfo rangeInfo)
		{
			IsRangeRequest = rangeInfo.IsRangeApplied;
			if (rangeInfo.IsRangeApplied)
			{
				RangeBegin = new long[] { rangeInfo.Begin };
				RangeEnd = new long[] { rangeInfo.End };
			}
		}

		public void InitializeRanges(HttpRequestMultipleRangeInfo rangeInfo)
		{
			IsRangeRequest = rangeInfo.IsRangeApplied;
			if (rangeInfo.IsRangeApplied)
			{
				RangeBegin = rangeInfo.Begin;
				RangeEnd = rangeInfo.End;
			}
		}

		public void InitializeRanges(WebResponseRangeInfo rangeInfo)
		{
			IsRangeRequest = rangeInfo.IsRangeApplied;
			if (rangeInfo.IsRangeApplied)
			{
				RangeBegin = new long[] { rangeInfo.Begin };
				RangeEnd = new long[] { rangeInfo.End };
			}
		}

		public bool PhysicalFileAvailable { get; private set; }
		public string PhysicalFileName { get; private set; }

		private string _displayFileName;
		/// <summary>
		/// Name of the downoad file
		/// </summary>
		public string DisplayFileName
		{
			get { return _displayFileName; }
			set
			{
				if (string.IsNullOrEmpty(value))
					_displayFileName = value;
				else
					_displayFileName = value.Replace(' ', '_');
			}
		}

		internal bool Disposed { get; private set; }
		internal Guid DataId { get; private set; }

		internal string UserId { get; set; }

		/// <summary>
		/// The data
		/// </summary>
		public Stream DataStream { get; private set; }

		

		/// <summary>
		/// If true and if RangeRequest is true the specifed range values will apply to DataStream,
		/// otherwise the DataStream will send without change.
		/// </summary>
		public bool ApplyRangeToStream { get; set; }

		/// <summary>
		/// Indicates if this is a range request 
		/// </summary>
		public bool IsRangeRequest { get; set; }

		/// <summary>
		/// Range of data, the begining
		/// </summary>
		public long[] RangeBegin { get; set; }

		/// <summary>
		/// Range of data, the end
		/// </summary>
		public long[] RangeEnd { get; set; }

		/// <summary>
		/// Content total size in bytes
		/// </summary>
		public long ContentLength { get; set; }

		/// <summary>
		/// Content type
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Last write time
		/// </summary>
		public DateTime LastWriteTimeUtc { get; set; }

		/// <summary>
		/// This unique code must keep the same as long as the file does not change.
		/// </summary>
		public string EntityTag { get; set; }

		/// <summary>
		/// Request http method
		/// </summary>
		public string RequestHttpMethod { get; set; }

	}
}
