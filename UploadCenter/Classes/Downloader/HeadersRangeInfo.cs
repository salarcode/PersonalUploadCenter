using System;
using System.Collections.Generic;
using System.Text;

namespace Salar.ResumableDownload
{
	public struct HttpRequestSimpleRangeInfo
	{
		/// <summary>
		/// Is the request range specified
		/// </summary>
		public bool IsRangeApplied;

		/// <summary>
		/// Range start
		/// </summary>
		public long Begin;

		/// <summary>
		/// Range end
		/// </summary>
		public long End;
	}

	public struct HttpRequestMultipleRangeInfo
	{
		/// <summary>
		/// Is the request range specified
		/// </summary>
		public bool IsRangeApplied;

		/// <summary>
		/// Range(s) start
		/// </summary>
		public long[] Begin;

		/// <summary>
		/// Range(s) end
		/// </summary>
		public long[] End;
	}

	public struct WebResponseRangeInfo
	{
		/// <summary>
		/// Is the response range specified
		/// </summary>
		public bool IsRangeApplied;

		/// <summary>
		/// Range start
		/// </summary>
		public long Begin;

		/// <summary>
		/// Range end
		/// </summary>
		public long End;

		/// <summary>
		/// length of the data
		/// </summary>
		public long RangeLength;

		/// <summary>
		/// length of the range
		/// </summary>
		public long DataLength;
	}

}
