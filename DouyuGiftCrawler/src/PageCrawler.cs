using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Douyu.Gift.Crawler
{
    public class PageCrawler
    {
        private ManualResetEvent _allDone = new ManualResetEvent(false);

        public PageCrawler()
        {
            Timeout = 3000;
            _encodingName = "utf-8";
        }

        public int Timeout { get; set; }

        WebProxy _webProxy;
        string _encodingName;
        Stopwatch _watch;

        public void SetProxy(WebProxy webProxy)
        {
            _webProxy = webProxy;
        }

        public void CrawlPage(string uri)
        {
            CrawlPage(uri, _encodingName);
        }

        public void CrawlPage(string uri, string encodingName)
        {
            _watch = Stopwatch.StartNew();
            var request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Timeout = Timeout;
            request.Proxy = _webProxy;
            //request.ServicePoint.ConnectionLimit = 100;

            request.KeepAlive = true;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "GET";
            request.Accept = "*/* ";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5";
            request.Referer = uri;

            // Start the asynchronous operation to get the response
            _encodingName = encodingName;
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
            _allDone.WaitOne();

            //var response = request.GetResponse();
            //var stream = response.GetResponseStream();
            //var page = "";
            //using (var reader = new StreamReader(stream, Encoding.GetEncoding(EncodingName))) {
            //    page = reader.ReadToEnd();
            //}

            //OnPageCrawlComplete(Url, page);
            return;
        }

        void GetResponseCallback(IAsyncResult asyncResult)
        {
            var request = asyncResult.AsyncState as HttpWebRequest;

            // End the operation
            var response = request.EndGetResponse(asyncResult) as HttpWebResponse;
            var responseStream = response.GetResponseStream();
            var streamReader = new StreamReader(responseStream, Encoding.GetEncoding(_encodingName));
            var webPage = streamReader.ReadToEnd();

            // close the stream object
            responseStream.Close();
            streamReader.Close();

            // Release the HttpWebResponse
            response.Close();
            _allDone.Set();

            OnPageCrawlCompleted(request.Address.ToString(), webPage);
        }

        public event EventHandler<PageCrawlCompletedEventArgs> PageCrawlCompleted;

        protected void OnPageCrawlCompleted(string url, string page)
        {
            if (PageCrawlCompleted != null)
                PageCrawlCompleted(this, new PageCrawlCompletedEventArgs(url, page, _watch.ElapsedMilliseconds));
        }
    }



    public class PageCrawlCompletedEventArgs : EventArgs
    {
        public PageCrawlCompletedEventArgs(string url, string page, long crawledTime)
        {
            Url = url;
            Page = page;
            CrawledTime = crawledTime;
        }

        public string Url { get; private set; }
        public string Page { get; private set; }
        public long CrawledTime { get; private set; }
    }
}
