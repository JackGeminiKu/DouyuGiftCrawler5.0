using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Jack4net.Log;

namespace Douyu.Gift.Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var pageCrawler = new PageCrawler();
            pageCrawler.PageCrawlCompleted += new EventHandler<PageCrawlCompletedEventArgs>(pageCrawler_PageCrawlCompleted);

            for (var i = 0; i < 100; i++) {
                pageCrawler.CrawlPage(GetUrl());
                MyThread.Wait(1000);
            }


            Console.Read();
        }

        static void pageCrawler_PageCrawlCompleted(object sender, PageCrawlCompletedEventArgs e)
        {
            LogService.DebugFormat("{0}", e.CrawledTime);
        }

        static int _pageIndex = 0;

        static string GetUrl()
        {
            return @"http://open.douyucdn.cn/api/RoomApi/room/" + _pageIndex++;
        }
    }
}
