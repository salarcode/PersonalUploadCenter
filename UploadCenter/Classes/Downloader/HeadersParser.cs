using System;
using System.Net;
using System.Web;

namespace Salar.ResumableDownload
{
	/// <summary>
	/// Parsing request or response headers for download informations
	/// </summary>
	/// <authors>
	/// Salar Khalilzadeh
	/// http://www.salarcode.com
	/// http://blog.salarcode.com
	/// https://salarblogsources.svn.codeplex.com/svn/Projects/Salar.ResumableDownload/
	/// 
	/// ZIPHandler: Alexander Schaaf
	/// http://www.devx.com
	/// </authors>
	public class HeadersParser
	{
		const string HTTP_METHOD_GET = "GET";
		const string HTTP_METHOD_HEAD = "HEAD";
		const string HTTP_HEADER_CONTENT_RANGE = "Content-Range";
		const string HTTP_HEADER_CONTENT_LENGTH = "Content-Length";
		const string HTTP_HEADER_IF_RANGE = "If-Range";
		const string HTTP_HEADER_IF_MATCH = "If-Match";
		const string HTTP_HEADER_IF_NONE_MATCH = "If-None-Match";
		const string HTTP_HEADER_IF_MODIFIED_SINCE = "If-Modified-Since";
		const string HTTP_HEADER_IF_UNMODIFIED_SINCE = "If-Unmodified-Since";
		const string HTTP_HEADER_UNLESS_MODIFIED_SINCE = "Unless-Modified-Since";
		const string HTTP_HEADER_RANGE = "Range";


		/// <summary>
		/// Parses user request for range specifed parameters
		/// </summary>
		/// <param name="request">The request object</param>
		/// <returns>Returns false if the requested ranges are invalid, otherwise returns true </returns>
		public static HttpRequestSimpleRangeInfo ParseHttpRequestHeaderSimpleRange(HttpRequest request)
		{
			HttpRequestSimpleRangeInfo rangeInfo;
			string sSource;
			int iLoop;
			string[] sRanges;
			rangeInfo.Begin = 0;
			rangeInfo.End = -1;
			rangeInfo.IsRangeApplied = false;

			// Parses the Range header from the Request (if there is one)
			// returns true, if the Range header was valid, or if there was no 
			//               Range header at all (meaning that the whole 
			//               file was requested)
			// returns false, if the Range header asked for unsatisfieable 
			//                ranges

			// Retrieve Range Header value from Request (Empty if none is indicated)
			sSource = RetrieveHeader(request, HTTP_HEADER_RANGE, String.Empty);

			if (sSource.Equals(String.Empty))
			{
				// No Range was requested, return the entire file range...

				rangeInfo.Begin = 0;
				rangeInfo.End = -1;

				// no Range request
				rangeInfo.IsRangeApplied = false;
			}
			else
			{
				// A Range was requested... 

				// return true for the bRange parameter, telling the caller
				// that the Request is indeed a Range request...
				rangeInfo.IsRangeApplied = true;

				// Remove "bytes=" from the beginning, and split the remaining 
				// string by comma characters
				sRanges = sSource.Replace("bytes=", "").Split(",".ToCharArray());

				// Check each found Range request for consistency
				if (sRanges.Length > 0)
				{
					iLoop = 0;

					// Split this range request by the dash character, 
					// sRange(0) contains the requested begin-value,
					// sRange(1) contains the requested end-value...
					string[] sRange = sRanges[iLoop].Split("-".ToCharArray());

					// Determine the end of the requested range
					if (sRange[1].Equals(String.Empty))
						// No end was specified, take the entire range
						rangeInfo.End = -1;
					else
						// An end was specified...
						rangeInfo.End = long.Parse(sRange[1]);

					// Determine the begin of the requested range
					if (sRange[0].Equals(String.Empty))
					{
						// No begin was specified, which means that
						// the end value indicated to return the last n
						// bytes of the file:

						// Calculate the begin
						rangeInfo.Begin = -1;

						// ... to the end of the file...
						rangeInfo.End = -1;
					}
					else
						// A normal begin value was indicated...
						rangeInfo.Begin = int.Parse(sRange[0]);
				}
			}
			return rangeInfo;
		}


		/// <summary>
		/// Parses user request for range specifed parameters
		/// </summary>
		public static HttpRequestMultipleRangeInfo ParseHttpRequestHeaderMultipleRange(
			HttpRequest request,
			long contentLength)
		{
			//bool validRanges;
			string sSource;
			int iLoop;
			string[] sRanges;
			var rangeInfo = new HttpRequestMultipleRangeInfo();

			// Parses the Range header from the Request (if there is one)
			// returns true, if the Range header was valid, or if there was no 
			//               Range header at all (meaning that the whole 
			//               file was requested)
			// returns false, if the Range header asked for unsatisfieable 
			//                ranges

			// Retrieve Range Header value from Request (Empty if none is indicated)
			sSource = RetrieveHeader(request, HTTP_HEADER_RANGE, String.Empty);

			if (string.IsNullOrEmpty(sSource))
			{
				// No Range was requested, return the entire file range...

				rangeInfo.Begin = new long[1];
				rangeInfo.End = new long[1];

				rangeInfo.Begin[0] = 0;
				rangeInfo.End[0] = contentLength - 1;

				// no Range request
				rangeInfo.IsRangeApplied = false;
			}
			else
			{
				// A Range was requested... 

				// return true for the bRange parameter, telling the caller
				// that the Request is indeed a Range request...
				rangeInfo.IsRangeApplied = true;

				// Remove "bytes=" from the beginning, and split the remaining 
				// string by comma characters
				sRanges = sSource.Replace("bytes=", "").Split(",".ToCharArray());
				rangeInfo.Begin = new long[sRanges.GetUpperBound(0) + 1];
				rangeInfo.End = new long[sRanges.GetUpperBound(0) + 1];

				// Check each found Range request for consistency
				for (iLoop = sRanges.GetLowerBound(0); iLoop <= sRanges.GetUpperBound(0); iLoop++)
				{

					// Split this range request by the dash character, 
					// sRange(0) contains the requested begin-value,
					// sRange(1) contains the requested end-value...
					string[] sRange = sRanges[iLoop].Split("-".ToCharArray());

					// Determine the end of the requested range
					if (sRange[1].Equals(String.Empty))
						// No end was specified, take the entire range
						rangeInfo.End[iLoop] = contentLength - 1;
					else
						// An end was specified...
						rangeInfo.End[iLoop] = long.Parse(sRange[1]);

					// Determine the begin of the requested range
					if (sRange[0].Equals(String.Empty))
					{
						// No begin was specified, which means that
						// the end value indicated to return the last n
						// bytes of the file:

						// Calculate the begin
						rangeInfo.Begin[iLoop] = contentLength - 1 - rangeInfo.End[iLoop];
						// ... to the end of the file...
						rangeInfo.End[iLoop] = contentLength - 1;
					}
					else
						// A normal begin value was indicated...
						rangeInfo.Begin[iLoop] = long.Parse(sRange[0]);

				}
			}
			return rangeInfo;
		}


		/// <summary>
		/// Parses server response indicating returned bytes
		/// </summary>
		public static WebResponseRangeInfo ParseWebResponseHeaderRange(WebResponse webResponse)
		{
			var rangeInfo = new WebResponseRangeInfo();
			// Initial values
			rangeInfo.Begin = 0;
			rangeInfo.End = webResponse.ContentLength;
			rangeInfo.RangeLength = rangeInfo.End;
			rangeInfo.IsRangeApplied = false;
			//var dataLength = webResponse.ContentLength;

			if (webResponse is HttpWebResponse)
			{
				// only partial content ollowed range responses
				if ((webResponse as HttpWebResponse).StatusCode != HttpStatusCode.PartialContent)
				{
					rangeInfo.IsRangeApplied = false;
					return rangeInfo;
				}

				string contentRange = RetrieveHeader(webResponse.Headers[HTTP_HEADER_CONTENT_RANGE], String.Empty);
				string contentLength = RetrieveHeader(webResponse.Headers[HTTP_HEADER_CONTENT_LENGTH], String.Empty);

				if (contentRange.Length == 0 || contentLength.Length == 0)
				{
					rangeInfo.IsRangeApplied = false;
					return rangeInfo;
				}
				else
				{
					// the header whould be like this
					//HTTP/1.1 206 OK
					//Content-Range: bytes 500001-54216827/54216828
					//Content-Length: 53716827
					//Content-Type: application/x-zip-compressed
					//Last-Modified: Fri, 24 Jul 2009 21:45:22 GMT
					//ETag: "mTkR3PTZIofQXrAY3ZkNzA=="
					//Accept-Ranges: bytes
					//Binary content of DancingHampsters.zip from byte 500,001 onwards...

					rangeInfo.IsRangeApplied = true;

					string[] rangeParts = contentRange.Replace("bytes ", "").Split(new char[] { '/' });
					if (rangeParts.Length == 2)
					{
						string[] dataRange = rangeParts[0].Split(new char[] { '-' });

						rangeInfo.Begin = Convert.ToInt32(dataRange[0]);
						rangeInfo.End = Convert.ToInt64(dataRange[1]);

						// and the content length
						rangeInfo.DataLength = Convert.ToInt64(rangeParts[1]);

						// the range length is the content-length
						rangeInfo.RangeLength = Convert.ToInt64(contentLength);

						return rangeInfo;
					}

					rangeInfo.IsRangeApplied = false;
					// invalid header!
					return rangeInfo;
				}
			}

			rangeInfo.IsRangeApplied = false;
			return rangeInfo;
		}

		/// <summary>
		/// Validate
		/// </summary>
		public bool ValidateHeaderRange(HttpRequest request, HttpRequestSimpleRangeInfo rangeInfo, long contentLength)
		{
			var validRanges = true;

			// Begin and end must not exceed the file size
			if ((rangeInfo.Begin > (contentLength - 1)) || (rangeInfo.End > (contentLength - 1)))
				validRanges = false;

			// Begin and end cannot be < 0
			if ((rangeInfo.Begin < 0) || (rangeInfo.End < 0))
				validRanges = false;

			// End must be larger or equal to begin value
			if (rangeInfo.End < rangeInfo.Begin)
				// The requested Range is invalid...
				validRanges = false;

			return validRanges;
		}

		/// <summary>
		/// Validate
		/// </summary>
		public bool ValidateHeaderRange(HttpRequest request, HttpRequestMultipleRangeInfo rangeInfo, long contentLength)
		{
			var validRanges = true;
			// Check each found Range request for consistency
			for (int index = 0; index <= rangeInfo.Begin.Length; index++)
			{

				// Check if the requested range values are valid, 
				// return false if they are not.
				//
				// Note:
				// Do not clean invalid values up by fitting them into
				// valid parameters using Math.Min and Math.Max, because
				// some download clients (like Go!Zilla) might send invalid 
				// (e.g. too large) range requests to determine the file limits!

				// Begin and end must not exceed the file size
				if ((rangeInfo.Begin[index] > (contentLength - 1)) || (rangeInfo.End[index] > (contentLength - 1)))
					validRanges = false;

				// Begin and end cannot be < 0
				if ((rangeInfo.Begin[index] < 0) || (rangeInfo.End[index] < 0))
					validRanges = false;

				// End must be larger or equal to begin value
				if (rangeInfo.End[index] < rangeInfo.Begin[index])
					// The requested Range is invalid...
					validRanges = false;
			}

			return validRanges;
		}

		/// <summary>
		/// Validate
		/// </summary>
		public bool ValidateHeaderRange(WebResponse webResponse, WebResponseRangeInfo rangeInfo)
		{
			// range is not valid
			if (rangeInfo.RangeLength > rangeInfo.DataLength)
				return false;

			// range is not valid
			if (rangeInfo.Begin > rangeInfo.End)
				return false;

			// range can not be larger than total data size
			if (rangeInfo.End > rangeInfo.DataLength)
				return false;
			return true;
		}

		/// <summary>
		/// Validating partial request, extracts matched ETag
		/// </summary>
		/// <returns>true if pertial request is valid</returns>
		public static bool ValidatePartialRequest(
			HttpRequest request,
			DownloadDataInfo dataInfo,
			out string matchedETag,
			ref int statusCode)
		{
			matchedETag = null;

			if (!(dataInfo.RequestHttpMethod.Equals(HTTP_METHOD_GET) ||
				dataInfo.RequestHttpMethod.Equals(HTTP_METHOD_HEAD)))
			{
				// Currently, only the GET and HEAD methods 
				// are supported...
				statusCode = 501;  // Not implemented
				return false;
			}
			else if (!CheckIfModifiedSince(request, dataInfo))
			{
				// The entity is still unmodified...
				statusCode = 304;  // Not Modified
				return false;
			}
			else if (!CheckIfUnmodifiedSince(request, dataInfo))
			{
				// The entity was modified since the requested date... 
				statusCode = 412;  // Precondition failed
				return false;
			}
			else if (!CheckIfMatch(request, dataInfo))
			{
				// The entity does not match the request... 
				statusCode = 412;  // Precondition failed
				return false;
			}
			else if (!CheckIfNoneMatch(request, ref statusCode, out matchedETag, dataInfo))
			{
				// The entity does match the none-match request, the response 
				// code was set inside the CheckifNoneMatch function

				//matchedETag

				// valid but content is not required
				return false;
			}
			else
			{
				//valid
				return true;
			}
		}

		private static bool CheckIfNoneMatch(
			HttpRequest request,
			ref int statusCode,
			out string matchedETag,
			DownloadDataInfo dataInfo)
		{
			string requestHeaderIfNoneMatch;
			string[] entityIDs;
			bool breturn = true;
			string sreturn = "";
			matchedETag = "";
			// Checks the If-None-Match header if it was sent with the request.
			//
			// returns true if one of the header values matches the file//s entity tag,
			//              or if "*" was sent,
			// returns false if a header was sent, but does not match the file, or
			//               if no header was sent.

			// Retrieve If-None-Match Header value from Request (*, meaning any, if none is indicated)
			requestHeaderIfNoneMatch = RetrieveHeader(request, HTTP_HEADER_IF_NONE_MATCH, String.Empty);

			if (requestHeaderIfNoneMatch.Equals(String.Empty))
				// Perform the request normally...
				breturn = true;
			else
			{
				if (requestHeaderIfNoneMatch.Equals("*"))
				{
					// The server must not perform the request 
					statusCode = 412;  // Precondition failed
					breturn = false;
				}
				else
				{
					// One or more Match IDs where sent by the client software...
					entityIDs = requestHeaderIfNoneMatch.Replace("bytes=", "").Split(",".ToCharArray());

					// Loop through all entity IDs, finding one which 
					// does not match the current file//s ID will be
					// enough to satisfy the If-None-Match
					for (int iLoop = entityIDs.GetLowerBound(0); iLoop <= entityIDs.GetUpperBound(0); iLoop++)
					{
						if (entityIDs[iLoop].Trim().Equals(dataInfo.EntityTag))
						{
							sreturn = entityIDs[iLoop];
							breturn = false;
						}
					}

					if (!breturn)
					{
						// One of the requested entities matches the current file's tag,
						//objResponse.AppendHeader("ETag", sreturn);
						matchedETag = sreturn;
						statusCode = 304; // Not Modified
					}
				}
			}
			// return the result...
			return breturn;
		}

		private static bool CheckIfMatch(HttpRequest request, DownloadDataInfo responseData)
		{
			string requestHeaderIfMatch;
			string[] entityIDs;
			bool result = false;

			// Checks the If-Match header if it was sent with the request.
			//
			// returns true if one of the header values matches the file//s entity tag,
			//              or if no header was sent,
			// returns false if a header was sent, but does not match the file.


			// Retrieve If-Match Header value from Request (*, meaning any, if none is indicated)
			requestHeaderIfMatch = RetrieveHeader(request, HTTP_HEADER_IF_MATCH, "*");

			if (requestHeaderIfMatch.Equals("*"))
				// The server may perform the request as if the
				// If-Match header does not exists...
				result = true;
			else
			{
				// One or more Match IDs where sent by the client software...
				entityIDs = requestHeaderIfMatch.Replace("bytes=", "").Split(",".ToCharArray());

				// Loop through all entity IDs, finding one 
				// which matches the current file's ID will
				// be enough to satisfy the If-Match
				for (int iLoop = entityIDs.GetLowerBound(0); iLoop <= entityIDs.GetUpperBound(0); iLoop++)
				{
					if (entityIDs[iLoop].Trim().Equals(responseData.EntityTag))
						result = true;
				}
			}
			// return the result...
			return result;
		}

		private static bool CheckIfUnmodifiedSince(HttpRequest request, DownloadDataInfo dataInfo)
		{
			string dateHeader;
			DateTime theDate;
			bool breturn;


			// Checks the If-Unmodified or Unless-Modified-Since header, if 
			// one of them was sent with the request.
			//
			// returns true, if the file was not modified since the 
			//               indicated date (RFC 1123 format), or
			//               if no header was sent,
			// returns false, if the file was modified since the indicated date


			// Retrieve If-Unmodified-Since Header value from Request (Empty if none is indicated)
			dateHeader = RetrieveHeader(request, HTTP_HEADER_IF_UNMODIFIED_SINCE, String.Empty);

			if (dateHeader.Equals(String.Empty))
				// If-Unmodified-Since was not sent, check Unless-Modified-Since... 
				dateHeader = RetrieveHeader(request, HTTP_HEADER_UNLESS_MODIFIED_SINCE, String.Empty);


			if (dateHeader.Equals(String.Empty))
				// No date was indicated, 
				// so just give this as true 
				breturn = true;

			else
			{
				try
				{
					// ... to parse the indicated sDate to a datetime value
					theDate = DateTime.Parse(dateHeader);

					// return true if the file was not modified since the indicated date...
					breturn = dataInfo.LastWriteTimeUtc < theDate;
				}
				catch (Exception)
				{
					// Converting the indicated date value failed, return false 
					breturn = false;
				}
			}
			return breturn;
		}

		private static bool CheckIfModifiedSince(HttpRequest request, DownloadDataInfo dataInfo)
		{
			string headerDate;
			DateTime theDate;
			bool breturn;

			// Checks the If-Modified header if it was sent with the request.
			//
			// returns true, if the file was modified since the 
			//               indicated date (RFC 1123 format), or
			//               if no header was sent,
			// returns false, if the file was not modified since
			//                the indicated date


			// Retrieve If-Modified-Since Header value from Request (Empty if none is indicated)
			headerDate = RetrieveHeader(request, HTTP_HEADER_IF_MODIFIED_SINCE, string.Empty);

			if (headerDate.Equals(String.Empty))
				// No If-Modified-Since date was indicated, 
				// so just give this as true 
				breturn = true;

			else
			{
				try
				{
					// ... to parse the indicated sDate to a datetime value
					theDate = DateTime.Parse(headerDate);

					// return true if the file was modified since or at the indicated date...
					breturn = (dataInfo.LastWriteTimeUtc >= theDate);
				}
				catch
				{
					// Converting the indicated date value failed, return false 
					breturn = false;
				}
			}
			return breturn;
		}

		/// <summary>
		/// Retrieves the indicated Header//s value from the Request,
		/// if the header was not sent, strDefault is returned.
		/// </summary>
		static string RetrieveHeader(HttpRequest request, string headerName, string strDefault)
		{
			// Retrieves the indicated Header//s value from the Request,
			// if the header was not sent, strDefault is returned.
			//
			// If the value contains quote characters, they are removed.

			string sreturn = request.Headers[headerName];

			if (string.IsNullOrEmpty(sreturn))
				// The Header wos not found in the Request, 
				// return the indicated default value...
				return strDefault;

			else
				// return the found header value, stripped of any quote characters...
				return sreturn.Replace("\"", "");
		}

		/// <summary>
		/// Retrieves the indicated Header//s value from the Request,
		/// if the header was not sent, strDefault is returned.
		/// </summary>
		static string RetrieveHeader(string headerString, string sDefault)
		{
			string sreturn;

			// Retrieves the indicated Header//s value from the Request,
			// if the header was not sent, sDefault is returned.
			//
			// If the value contains quote characters, they are removed.

			sreturn = headerString;

			if (string.IsNullOrEmpty(sreturn))
				// The Header wos not found in the Request, 
				// return the indicated default value...
				return sDefault;

			else
				// return the found header value, stripped of any quote characters...
				return sreturn.Replace("\"", "");
		}

	}
}
