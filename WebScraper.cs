//Hunter DeMeyer
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Scraper
{
    class WebScraper
    {
        public string tag;
        public string site;

        //deafult constructor, sets both values to empty strings
        public WebScraper()
        {
            this.tag = "";
            this.site = "";
        }

        //custom constructor
        public WebScraper(string tag, string webSite)
        {
            this.tag = tag;
            this.site = webSite;
        }
        
        //makes an http web request and returns html document as string
        public string getPage(string url)
        {
            //function that takes Uniform Resource Identifier(URI) and returns the html doc of given page
            string result = "";
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                result = reader.ReadToEnd();
                reader.Close();
                myResponse.Close();
            }
            catch (Exception e)
            {
                getPage(url);
            }
            return result;
        }

        //finds the index of the html tag and returns it
        public int find(string item, string body)
        {
            //takes the item to find and the body in which to find it
            // returns the index of the first character in the item
            body = body.ToLower();
            item = item.ToLower();
            int index = body.IndexOf(item);
            return index;
        }

        //finds data in html tag and returns it
        public string extract(int index, string body, string tag)
        {
            List<char> div = new List<char> { };
            char[] bodyCharArray = body.ToCharArray();
            string str;

            index += tag.Length;
            string condition = (body[index]).ToString() + (body[index + 1]).ToString();
            while (!(condition).Equals("</"))
            {
                div.Add(body[index]);
                index++;
                condition = (body[index]).ToString() + (body[index + 1]).ToString();
            }
            char[] priceArray = div.ToArray();
            str = string.Join("", priceArray);
            return str;
        }

    }
}
