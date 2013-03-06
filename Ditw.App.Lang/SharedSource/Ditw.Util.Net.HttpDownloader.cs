using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;
using Ditw.Util.IO;

namespace Ditw.Util.Net
{
    internal class RequestUserState
    {
        internal RequestUserState(String url, HttpWebRequest req)
        {
            _request = req;
            _url = url;
        }

        internal HttpStringDownloaderSession Session
        {
            get;
            set;
        }

        private HttpWebRequest _request;
        internal HttpWebRequest Request
        {
            get { return _request; }
        }

        private String _url;
        internal String Url
        {
            get
            {
                return _url;
            }
        }

        internal Byte[] DownloadedData
        {
            get;
            set;
        }

        internal String RawText
        {
            get;
            set;
        }
    }

    public class HttpStringDownloaderSession
    {
        public String[] Keywords
        {
            get;
            set;
        }

        public Int32 DownloadLinkCount
        {
            get
            {
                return _requestUserStates.Keys.Count;
            }
        }

        public Int32 DownloadCompleteCount
        {
            get
            {
                lock (_requestUserStates)
                {
                    return _requestUserStates.Values.Where(
                        v => v.RawText != null
                            ).Count();
                }
            }
        }

        public Int32 IncompleteCount
        {
            get { return DownloadLinkCount - DownloadCompleteCount; }
        }

        //private HashSet<String> _linksToDownload = new HashSet<String>();
        internal void AddUserState(String l, RequestUserState userState)
        {
            lock (_requestUserStates)
            {
                if (!_requestUserStates.ContainsKey(l))
                {
                    //Debug.WriteLine("========= new link: " + l);
                    //if (_downloadedStrings.ContainsKey(l))
                    //{
                    //    throw new Exception("already downloaded!");
                    //}
                    _requestUserStates.Add(l, userState);
                }
            }
        }

        public Boolean AllCompleted
        {
            get { return IncompleteCount == 0; }
        }

        public Boolean PercentCompleted
        {
            get { return IncompleteCount / (double)DownloadLinkCount <= 0.2; }
        }

        //private List<String> _downloadedStringList = new List<String>();
        private Dictionary<String, RequestUserState> _requestUserStates =
            new Dictionary<String, RequestUserState>();

        internal void AddRawText(String url, String rawData)
        {
            lock (_requestUserStates)
            {
                if (!_requestUserStates.ContainsKey(url))
                {
                    throw new Exception("SSS");
                    //return;
                }
                _requestUserStates[url].RawText = rawData;
                Debug.WriteLine(String.Format("AddRawText: Download Status {0}/{1}. {2}",
                    IncompleteCount,
                    DownloadLinkCount,
                    _requestUserStates.Where(
                        u => u.Value.RawText == null
                        ).FirstOrDefault().Key
                    )
                    );
            }

            ResultReady(url, rawData);
        }

        internal Action<String, String> ResultReady;
    }

    public class HttpStringDownloader
    {
        //static Int32 _responseCallBackCount = 0;
        private static void ResponseCallback(IAsyncResult ar)
        {
            String result;
            RequestUserState userState = ar.AsyncState as RequestUserState;
            Debug.WriteLine("[RESP] " + userState.Url);
            //Debug.WriteLine("ResponseCallback #" + _responseCallBackCount++.ToString() + ": " + userState.Url);
            Byte[] bytes;
            try
            {
                String contentType;
                using (WebResponse resp = userState.Request.EndGetResponse(ar))
                {
                    //Debug.Write(userState.Url + ": ");
                    //Debug.WriteLine(resp.ContentType);
                    contentType = resp.ContentType;
                    if (resp.ContentType.Contains("text"))
                    {
                        using (BinaryReader reader = new BinaryReader(resp.GetResponseStream()))
                        {
                            bytes = reader.ReadAllBytes();
                        }
                    }
                    else
                    {
                        bytes = null;
                    }
                }

                if (bytes != null)
                {
                    result = Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    result = String.Format("[Non-text data encountered!]");
                }
                result += String.Format("\nContent type: [{0}]", contentType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error downloading, detail: {0}", ex);
                userState.Session.AddRawText(userState.Url, String.Empty);
                return;
            }

            if (!BriefCheck(result, userState.Session.Keywords))
            {
                String charset = WebPageUtility.GetPageCharset(result);
                if (!String.IsNullOrEmpty(charset) && bytes != null)
                {
                    Debug.WriteLine("[{0}] Charset [{1}]", userState.Url, charset);
                    result = Encoding.GetEncoding(charset).GetString(bytes);
                }
                else
                {
                    Debug.WriteLine("[{0}] Failed to identify charset, index of 'charset=' [{1}]",
                        userState.Url,
                        result.IndexOf("charset=", StringComparison.OrdinalIgnoreCase));
                }
            }
            //Debug.WriteLine("========== downloaded: " + userState.Url);
            userState.Session.AddRawText(userState.Url, result);
            //userState.Session.ResultReady(userState.Url, result);
        }

        private static Boolean BriefCheck(String raw, String[] keywords)
        {
            if (keywords == null || keywords.Length <= 0)
                return false;

            for (Int32 i = 0; i < keywords.Length; i++)
            {
                if (!raw.Contains(keywords[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void Download(
            out HttpStringDownloaderSession session,
            IEnumerable<String> urls,
            String[] keywords,
            Action<String, String> dataReady)
        {
            session = new HttpStringDownloaderSession()
            {
                Keywords = keywords,
                ResultReady = dataReady
            };

            Int32 i = 0;
            foreach (String url in urls)
            {
                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    RequestUserState userState = new RequestUserState(url, req)
                    {
                        Session = session
                    };
                    session.AddUserState(url, userState);

                    req.Timeout = 20000;
                    req.AutomaticDecompression = DecompressionMethods.GZip |
                        DecompressionMethods.Deflate;
                    req.BeginGetResponse(ResponseCallback, userState);
                    Debug.WriteLine(String.Format("[REQ {0} {1}/{2}] {3}",
                        i++,
                        session.IncompleteCount,
                        session.DownloadLinkCount,
                        userState.Url)
                        );
                }
                catch
                {
                    continue;
                }
            }

            Debug.WriteLine(String.Format("------ {0} Requests", urls.Count()));

            //return session;
        }
    }
}
