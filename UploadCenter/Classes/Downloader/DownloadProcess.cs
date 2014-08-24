// Using this new feature enables performance but disables speed limit of downloads
// #define UseNewFeatures

using System;
using System.IO;
using System.Web;

namespace Salar.ResumableDownload
{
	/// <summary>
	/// Resumable download processor by Salar Khalilzadeh
	/// http://www.salarcode.com
	/// http://blog.salarcode.com
	/// https://salarblogsources.svn.codeplex.com/svn/Projects/Salar.ResumableDownload/
	/// 
	/// Original class name is "ZIPHandler" in VB.Net by Alexander Schaaf
	/// http://www.devx.com
	/// 
	/// Last update: 2012-2-25
	/// </summary>
	/// <authors>
	/// ResumableDownload: Salar Khalilzadeh,
	/// ZIPHandler: Alexander Schaaf
	/// </authors>
	public class DownloadProcess : IDisposable
	{
		public enum DownloadProcessState
		{
			None,

			/// <summary>
			/// Download is currently in progress
			/// </summary>
			InProgress,

			/// <summary>
			/// Download was in progress, but was cancelled 
			/// </summary>
			Broken,

			/// <summary>
			/// Part of download was completed
			/// </summary>
			PartFinished,

			/// <summary>
			/// Last part of download is finished, which most probably can say all of the download is finished
			/// </summary>
			LastPartfinished
		}

		#region Header consts

		private const string MULTIPART_BOUNDARY = "<q1w2e3r4t5y6u7i8o9p0>";
		private const string MULTIPART_CONTENTTYPE = "multipart/byteranges; boundary=" + MULTIPART_BOUNDARY;
		private const string HTTP_HEADER_Content_Disposition = "Content-Disposition";
		private const string HTTP_HEADER_ACCEPT_RANGES = "Accept-Ranges";
		private const string HTTP_HEADER_ACCEPT_RANGES_BYTES = "bytes";
		private const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
		private const string HTTP_HEADER_CONTENT_RANGE = "Content-Range";
		private const string HTTP_HEADER_CONTENT_LENGTH = "Content-Length";
		private const string HTTP_HEADER_ENTITY_TAG = "ETag";
		private const string HTTP_HEADER_LAST_MODIFIED = "Last-Modified";
		private const string HTTP_HEADER_RANGE = "Range";
		private const string HTTP_HEADER_IF_RANGE = "If-Range";
		private const string HTTP_HEADER_IF_MATCH = "If-Match";
		private const string HTTP_HEADER_IF_NONE_MATCH = "If-None-Match";
		private const string HTTP_HEADER_IF_MODIFIED_SINCE = "If-Modified-Since";
		private const string HTTP_HEADER_IF_UNMODIFIED_SINCE = "If-Unmodified-Since";
		private const string HTTP_HEADER_UNLESS_MODIFIED_SINCE = "Unless-Modified-Since";
		private const string HTTP_METHOD_GET = "GET";
		private const string HTTP_METHOD_HEAD = "HEAD";

		#endregion

		private DownloadDataInfo _dataInfo;
		private const int _ioBufferSize = 5120;

		public void Dispose()
		{
			if (_dataInfo != null)
				_dataInfo.Dispose();
		}

		public DownloadProcess(DownloadDataInfo dataInfo)
		{
			_dataInfo = dataInfo;
		}

		public DownloadProcessState ProcessDownload(HttpResponse httpResponse)
		{
			var processState = DownloadProcessState.None;

			// Long Arrays for Range values:
			// ...Begin() contains start positions for each requested Range
			long[] rangesBegin = _dataInfo.RangeBegin;

			// ...End() contains end positions for each requested Range
			long[] rangesEnd = _dataInfo.RangeEnd;

			// Response Header value: Content Length...
			int responseContentLength = 0;

			// The Stream we//re using to download the file in chunks...
			System.IO.Stream dataStream;

			// Total Bytes to read (per requested range)
			int bytesToRead;

			// Size of the Buffer for chunk-wise reading
			int bufferSize = _ioBufferSize;

			// The Buffer itself
			var ioBuffer = new byte[bufferSize];

			// Amount of Bytes read
			int lengthOfReadChunk = -1;

			// Indicates if the download was interrupted
			bool downloadBroken = false;

			// Indicates if this is a multipart range request
			bool contentIsMultipart = false;

			// Loop counter used to iterate through the ranges
			int loopIndex;

			// Content-Disposition value
			string contentDispositionFile = "attachment; filename=" + _dataInfo.DisplayFileName;


			if (!(_dataInfo.RequestHttpMethod.Equals(HTTP_METHOD_GET) ||
				_dataInfo.RequestHttpMethod.Equals(HTTP_METHOD_HEAD)))
				// Currently, only the GET and HEAD methods 
				// are supported...
				httpResponse.StatusCode = 501;  // Not implemented
			else
			{
				// Preliminary checks where successful... 
				if (_dataInfo.IsRangeRequest)
				{
					// This is a Range request... 

					// if the Range arrays contain more than one entry,
					// it even is a multipart range request...
					contentIsMultipart = (rangesBegin.GetUpperBound(0) > 0);

					// Loop through each Range to calculate the entire Response length
					for (loopIndex = rangesBegin.GetLowerBound(0); loopIndex <= rangesBegin.GetUpperBound(0); loopIndex++)
					{
						// The length of the content (for this range)
						responseContentLength += Convert.ToInt32(rangesEnd[loopIndex] - rangesBegin[loopIndex]) + 1;

						if (contentIsMultipart)
						{
							//
							responseContentLength += HTTP_HEADER_Content_Disposition.Length;
							// if this is a multipart range request, calculate 
							// the length of the intermediate headers to send
							responseContentLength += MULTIPART_BOUNDARY.Length;
							responseContentLength += _dataInfo.ContentType.Length;
							responseContentLength += rangesBegin[loopIndex].ToString().Length;
							responseContentLength += rangesEnd[loopIndex].ToString().Length;

							// DataLength = Total size
							responseContentLength += _dataInfo.ContentLength.ToString().Length;

							// 49 is the length of line break and other 
							// needed characters in one multipart header
							responseContentLength += 49;
						}

					}

					if (contentIsMultipart)
					{
						// if this is a multipart range request,  
						// we must also calculate the length of 
						// the last intermediate header we must send
						responseContentLength += MULTIPART_BOUNDARY.Length;
						// 8 is the length of dash and line break characters
						responseContentLength += 8;
					}
					else
					{
						// This is no multipart range request, so
						// we must indicate the response Range of 
						// in the initial HTTP Header
						// DataLength = Total size
						httpResponse.AppendHeader(HTTP_HEADER_CONTENT_RANGE, "bytes " +
							rangesBegin[0].ToString() + "-" +
							rangesEnd[0].ToString() + "/" +
							_dataInfo.ContentLength.ToString());
					}

					// Range response 
					httpResponse.StatusCode = 206; // Partial Response

				}
				else
				{
					// This is not a Range request, or the requested Range entity ID
					// does not match the current entity ID, so start a new download

					// Indicate the file's complete size as content length
					// DataLength = Total size
					responseContentLength = Convert.ToInt32(_dataInfo.ContentLength);

					// Return a normal OK status...
					httpResponse.StatusCode = 200;
				}


				// Write file name into the Response
				httpResponse.AppendHeader(HTTP_HEADER_Content_Disposition, contentDispositionFile);

				// Write the content length into the Response
				httpResponse.AppendHeader(HTTP_HEADER_CONTENT_LENGTH, responseContentLength.ToString());

				// Write the Last-Modified Date into the Response
				httpResponse.AppendHeader(HTTP_HEADER_LAST_MODIFIED, _dataInfo.LastWriteTimeUtc.ToString("r"));

				// Tell the client software that we accept Range request
				httpResponse.AppendHeader(HTTP_HEADER_ACCEPT_RANGES, HTTP_HEADER_ACCEPT_RANGES_BYTES);

				// Write the file//s Entity Tag into the Response (in quotes!)
				httpResponse.AppendHeader(HTTP_HEADER_ENTITY_TAG, "\"" + _dataInfo.EntityTag + "\"");


				// Write the Content Type into the Response
				if (contentIsMultipart)
					// Multipart messages have this special Type.
					// In this case, the file//s actual mime type is
					// written into the Response at a later time...
					httpResponse.ContentType = MULTIPART_CONTENTTYPE;
				else
					// Single part messages have the files content type...
					httpResponse.ContentType = _dataInfo.ContentType;


				if (_dataInfo.RequestHttpMethod.Equals(HTTP_METHOD_HEAD))
				{
					// Only the HEAD was requested, so we can quit the Response right here... 
				}
				else
				{
					// Flush the HEAD information to the client...
					httpResponse.Flush();

					// Download is in progress...
					processState = DownloadProcessState.InProgress;

					// The steram
					dataStream = _dataInfo.DataStream;

					// read specific ranges from stream?
					if (!_dataInfo.ApplyRangeToStream)
					{
						// first range only
						loopIndex = 0;

						// Calculate the total amount of bytes for first range only
						bytesToRead = Convert.ToInt32(rangesEnd[loopIndex] - rangesBegin[loopIndex]) + 1;

						// sending header for multipart request
						if (contentIsMultipart)
						{
							// if this is a multipart response, we must add 
							// certain headers before streaming the content:

							// The multipart boundary
							httpResponse.Output.WriteLine("--" + MULTIPART_BOUNDARY);

							// The mime type of this part of the content 
							httpResponse.Output.WriteLine(HTTP_HEADER_CONTENT_TYPE + ": " + _dataInfo.ContentType);

							// The actual range
							// // DataLength = Total size
							httpResponse.Output.WriteLine(HTTP_HEADER_CONTENT_RANGE + ": bytes " +
								rangesBegin[loopIndex].ToString() + "-" +
								rangesEnd[loopIndex].ToString() + "/" +
								_dataInfo.ContentLength.ToString());

							// Indicating the end of the intermediate headers
							httpResponse.Output.WriteLine();
						}

						// flush the data

						// Declare variables
						int readed = -1;

#if UseNewFeatures
						if (_dataInfo.PhysicalFileAvailable)
						{
							// send file content directly
							httpResponse.TransmitFile(_dataInfo.PhysicalFileName);
						}
						else
#endif
						// Get the response stream for reading
						// Read the stream and write it into memory
						while ((int)(readed = dataStream.Read(ioBuffer, 0, ioBuffer.Length)) > 0)
						{
							if (httpResponse.IsClientConnected)
							{
								// write to response
								httpResponse.OutputStream.Write(ioBuffer, 0, readed);

								// send response
								httpResponse.Flush();
							}
							else
							{
								downloadBroken = true;
								break;
							}
						}

						// In Multipart responses, mark the end of the part 
						if (contentIsMultipart)
							httpResponse.Output.WriteLine();

						// No need to proceed to the next part if the 
						// client was disconnected
						if (downloadBroken)
						{
							//break;
						}
						// done!
					}
					else
					{
						// Now, for each requested range, stream the chunks to the client:
						for (loopIndex = rangesBegin.GetLowerBound(0); loopIndex <= rangesBegin.GetUpperBound(0); loopIndex++)
						{
							// Move the stream to the desired start position...
							dataStream.Seek(rangesBegin[loopIndex], SeekOrigin.Begin);

							// Calculate the total amount of bytes for this range
							bytesToRead = Convert.ToInt32(rangesEnd[loopIndex] - rangesBegin[loopIndex]) + 1;

							if (contentIsMultipart)
							{
								// if this is a multipart response, we must add 
								// certain headers before streaming the content:

								// The multipart boundary
								httpResponse.Output.WriteLine("--" + MULTIPART_BOUNDARY);

								// The mime type of this part of the content 
								httpResponse.Output.WriteLine(HTTP_HEADER_CONTENT_TYPE + ": " + _dataInfo.ContentType);

								// The actual range
								// // DataLength = Total size
								httpResponse.Output.WriteLine(HTTP_HEADER_CONTENT_RANGE + ": bytes " +
									rangesBegin[loopIndex].ToString() + "-" +
									rangesEnd[loopIndex].ToString() + "/" +
									_dataInfo.ContentLength.ToString());

								/*objResponse.AppendHeader(HTTP_HEADER_CONTENT_RANGE,": bytes " +
									alRequestedRangesBegin[iLoop].ToString() + "-" +
									alRequestedRangesend[iLoop].ToString() + "/" +
									objFile.Length.ToString());
								*/
								// Indicating the end of the intermediate headers
								httpResponse.Output.WriteLine();

							}

#if UseNewFeatures
							if (_dataInfo.PhysicalFileAvailable)
							{
								// send file content directly
								httpResponse.TransmitFile(_dataInfo.PhysicalFileName, rangesBegin[loopIndex], bytesToRead);
							}
							else
#endif
							// Now stream the range to the client...
							while (bytesToRead > 0)
							{
								if (httpResponse.IsClientConnected)
								{
									// Read a chunk of bytes from the stream
									lengthOfReadChunk = dataStream.Read(ioBuffer, 0, Math.Min(ioBuffer.Length, bytesToRead));

									// Write the data to the current output stream.
									httpResponse.OutputStream.Write(ioBuffer, 0, lengthOfReadChunk);

									// Flush the data to the HTML output.
									httpResponse.Flush();

									// Clear the buffer
									//bBuffer=new byte[iBufferSize];

									// Reduce BytesToRead
									bytesToRead -= lengthOfReadChunk;
								}
								else
								{
									// The client was or has disconneceted from the server... stop downstreaming...
									bytesToRead = -1;
									downloadBroken = true;
								}
							}

							// In Multipart responses, mark the end of the part 
							if (contentIsMultipart)
								httpResponse.Output.WriteLine();

							// No need to proceed to the next part if the 
							// client was disconnected
							if (downloadBroken)
								break;
						}
					}
					// At this point, the response was finished or cancelled... 

					if (downloadBroken)
						// Download is broken...
						processState = DownloadProcessState.Broken;
					else
					{
						if (contentIsMultipart)
						{
							// In multipart responses, close the response once more with 
							// the boundary and line breaks
							httpResponse.Output.WriteLine("--" + MULTIPART_BOUNDARY + "--");
							httpResponse.Output.WriteLine();
						}

						// The download was finished
						processState = DownloadProcessState.PartFinished;
					}
				}
			}

			// if this part finished notify the data info
			if (processState == DownloadProcessState.PartFinished)
			{
				if (rangesEnd.Length > 0 && rangesEnd[0] == _dataInfo.ContentLength - 1)
				{
					processState = DownloadProcessState.LastPartfinished;
				}
			}
			//====== return download state ======
			return processState;
		}
	}
}
