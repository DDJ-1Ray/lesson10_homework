using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace test12
{
    class SimpleCrawler
    {
        private Hashtable urls = new Hashtable();
        private int count = 0;
        static void Main(string[] args)
        {
            SimpleCrawler myCrawler = new SimpleCrawler();
            string startUrl = "https://www.cnblogs.com/dstang2000/";

            //string try1 = "../archive/2010/11/02/1867406.html";
            
            //Console.WriteLine(change(try1, startUrl));
            if (args.Length >= 1) startUrl = args[0];
            myCrawler.urls.Add(startUrl, false);//加入初始页面
            new Thread(()=>myCrawler.Crawl(startUrl)).Start();
        }

        private void Crawl(string startUrl)
        {
            Console.WriteLine("開始爬行了.... ");
            while (true)
            {
                string current = null;
                foreach (string url in urls.Keys)
                {
                    if ((bool)urls[url])
                        continue;
                    current = url;
                    
                }

                if (current == null || count > 10) break;
                Console.WriteLine("爬行" + current + "頁面!");
                string html = DownLoad(current); // 下载
                urls[current] = true;
                count++;
                Parse(html, startUrl);//解析,并加入新的链接
                Console.WriteLine("爬行結束");
            }
        }

        public string DownLoad(string url)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string html = webClient.DownloadString(url);

                //Console.WriteLine(url);
                string fileName = count.ToString();
                //Console.WriteLine(fileName);
                File.WriteAllText(fileName, html, Encoding.UTF8);
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        private void Parse(string html,string startUrl)
        {
            string strRef = @"(href|HREF)[]*=[]*[""'][^""'#>]+[""']";
            string strRef2 = mainbody(startUrl);

            string strRef3 = @"(html|HTML)+$";
            MatchCollection matches = new Regex(strRef).Matches(html);

            foreach (Match match in matches)
            {
                
                strRef = match.Value.Substring(match.Value.IndexOf('=') + 1)
                          .Trim('"', '\"', '#', '>');
                //strRef = change(strRef);

                //strRef = Path.GetFullPath(strRef);

                //Console.WriteLine(strRef);
                strRef = change(strRef, startUrl);
                if (!Regex.IsMatch(strRef, strRef3)|| !Regex.IsMatch(strRef, strRef2))
                {
                    
                    continue;
                }
                
                

                //if (!Regex.IsMatch(strRef, strRef3))
                //    continue;
                //Console.WriteLine(strRef);

                if (strRef.Length == 0) continue;
                if (urls[strRef] == null) urls[strRef] = false;
            }
        }
        private string change(string str,string startUrl)
        {
            Regex regex;

            if (str.StartsWith("http://"))
            {
                regex = new Regex("http://");
                str =regex.Replace(str,"https://");
                return str;
            }
            else if (Regex.IsMatch(str, "^[a-zA-Z]")&&(!Regex.IsMatch(str,"https://")))
            {
                str = startUrl + str;
                return str;
            }
            else if (Regex.IsMatch(str, "^/"))
            {

                str = startUrl.Substring(0,startUrl.Length-1) + str;
                return str;
            }
            else if (Regex.IsMatch(str, "^../"))
            {
                regex = new Regex("^../");
                str = regex.Replace(str, "/");
                return startUrl.Substring(0,startUrl.Length-1)+str;
            }
            return str;
        }
        private string mainbody(string str)
        {
            
            if(Regex.IsMatch(str, @"www.{^.}+"))
            {
                Regex regex = new Regex("(?<name>www.{^.}+)");
                str = regex.Replace(str,"${name}");
                return str;
            }
            return str;
        }
    }
}
//public static void Main(string[] args) {
//    string minHtml = string.Empty;
//    string url = @"http://www.agronet.com.cn/default.aspx";
//    string preurl = url.Remove(url.IndexOf('/', 8) + 1);
//    //获取url的根目录地址 
//    minHtml = GetRequestString(url, 6000, 1, System.Text.Encoding.UTF8);
//    //获取指定页面的内容
//    Console.WriteLine(preurl);
//    GetUrlListBHtml(minHtml,preurl);
//    Console.ReadKey(); } 
///// <summary> /// 
///// 获取html内容中的相对url地址，并向相对地址添加前缀 /// </summary> /// <param name="text">html内容</param> /// <param name="pre">要添加的绝对地址前缀</param>
//    public static void GetUrlListBHtml(string text,string pre) {
//    string pat = @"(?<=href\s*=)(?:[ \s""']*)(?!#|mailto|location.|javascript|.*css|.*this\.)[^""']*(?:[ \s>""'])"; // Compile the regular expression. 
//    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(pat, System.Text.RegularExpressions.RegexOptions.IgnoreCase); // Match the regular expression pattern against a text string.
//    System.Text.RegularExpressions.Match m = r.Match(text); int matchCount = 0; while (m.Success) { string urlX=m.Value.Replace("\"","");//替换引号 
//    if (urlX.IndexOf("/") == 0)//相对地址 
//    {
//        matchCount++;
//        Console.WriteLine("第" + matchCount+"个相对地址：");
//        Console.WriteLine("原地址是"+urlX);
//        Console.WriteLine("新的绝对地址是" + pre+urlX);
//        Console.WriteLine("------------------------------------");
//    }
//        m = m.NextMatch();
//    }
//}