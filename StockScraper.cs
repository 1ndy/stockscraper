//Hunter DeMeyer
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    class StockScraper : WebScraper
    {
        public string stockSym;
        public string stockName;
        public double userHigh;
        public double userLow;
        public List<double> stockHistory = new List<double> { }; //maybe make dictionary, store with time for advanced stats
        public double stockPrice;
        public double buyPrice;
        public int numStocks;


        public string wSite;
        public string sTag;

        //custom constructor
        public StockScraper(string name, double hi, double lo, int numOfStocks, double boughtFor) : base()
        {
            this.stockSym = name;
            this.wSite = "http://www.nasdaq.com/symbol/" + this.stockSym + "/real-time";
            this.stockName = this.getStockName();
            this.userHigh = hi;
            this.userLow = lo;
            this.stockSym = this.stockSym.Trim();
            this.numStocks = numOfStocks;
            this.buyPrice = boughtFor;
            this.sTag = "<div id=\"qwidget_lastsale\" class=\"qwidget-dollar\">";
        }

        //constructor used like a static implementation of the object, used just to get stock price.
        public StockScraper(string name)
        {
            this.stockSym = name;
            this.wSite = "http://www.nasdaq.com/symbol/" + this.stockSym + "/real-time";
            this.stockSym = this.stockSym.Trim();
            this.sTag = "<div id=\"qwidget_lastsale\" class=\"qwidget-dollar\">";
        }

        //finds the data in the <head></head> tag, trims it, and returns what should be the name of the company (or most of it).
        public string getStockName()
        {
            string page = this.getPage(this.wSite);
            int headerIndex = this.find("<title>", page);
            string header = this.extract(headerIndex, page, "<title>");
            int index = header.IndexOf("(");
            header = header.Substring(0, index);
            return header;
        }

        //uses method from WebScraper to get the current price of the stock
        public void pullStockPrice()
        {
            string page = this.getPage(this.wSite);
            int priceIndex = this.find(this.sTag, page);
            if (priceIndex == -1)
            {
                Console.WriteLine("\n404 Not Found");
            }

            string price = this.extract(priceIndex, page, this.sTag);
            char[] dollarSignIndex = { '$' };
            price = price.TrimStart(dollarSignIndex);

            this.stockPrice = Double.Parse(price);
            this.stockHistory.Add(this.stockPrice);
            //return Double.Parse(price);
        }

    }
}
