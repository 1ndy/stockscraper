// Hunter DeMeyer
// 7/15/15
// Monitors stocks

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Globalization;

namespace Scraper
{
    class Program
    {
        static void Main()
        {
            Dictionary<string, StockScraper> scrapers = new Dictionary<string, StockScraper> { };
            //List<StockScraper> buffer = new List<StockScraper> { };
            string sName = prompt("Enter stock to monitor: ").ToLower();

            while (!sName.Equals("//"))
            {
                initialize(scrapers, sName);
                sName = prompt("Enter stock to monitor: ").ToLower();
            }
            Console.Clear();
            
            //Main loop of program
            while (true)
            {
                foreach (StockScraper scrape in scrapers.Values)
                {
                    scrape.pullStockPrice();
                }
                Console.Clear();
                displayBanner();
                Console.WriteLine("Report for: " + DateTime.Now);
                Console.WriteLine();
                foreach(StockScraper scrape in scrapers.Values)
                {
                    if (scrape.stockPrice == -1)
                    {
                        Console.WriteLine("Error");
                        continue;
                    }
                    if (checkHigh(scrape) || checkLow(scrape))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("========================================");
                        Console.ResetColor();
                        display(scrape);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("========================================");
                        Console.ResetColor();
                    }
                    else
                        display(scrape);
                    Console.WriteLine("\n\n");
                }
                System.Threading.Thread.Sleep(5000);
            }
            //End of main loop
        }


        //Methods

        //prompts string to user and returns response
        static string prompt(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        //creates a stockScraper object for given stock and returns it, Also used to provide user data and accept a response based on that information.
        static StockScraper initialize(Dictionary<string, StockScraper> scrapers, string sName)
        {
            foreach(string symbol in scrapers.Keys)
            {
                if (symbol.Equals(sName))
                {
                    Console.WriteLine("\nYou are already monitoring that stock. Enter a different one.\n");
                    Main();
                }
                else
                    continue;
            }
            StockScraper temp = new StockScraper(sName);
            try
            {
                temp.pullStockPrice();
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong...is that a real stock?\n");
                Main();
            }
            Console.WriteLine("  Current price: " + temp.stockPrice.ToString("C", CultureInfo.CurrentCulture));

            string high = prompt("\tHigh alert: ");
            string low = prompt("\tLow alert: ");
            string stocksOwned = prompt("\tShares Owned: ");
            string buyPrice = prompt("\tBought for: ");

            int ownNumStocks;
            double hi, lo, boughtPrice;
            if (high.Equals("//"))
                high = "1000000";
            if (low.Equals("//"))
                low = "0";
            if (stocksOwned.Equals("//"))
                ownNumStocks = 0;
            else
                ownNumStocks = int.Parse(stocksOwned);
            if (buyPrice.Equals("//"))
                boughtPrice = 0;
            else
                boughtPrice = Double.Parse(buyPrice);

            hi = Math.Abs(Double.Parse(high));
            lo = Math.Abs(Double.Parse(low));
            //put that in a try-catch and recall initialize with param names-it works!
            double aHi = Math.Max(hi, lo);
            double aLo = Math.Min(hi, lo);
            StockScraper scraper = new StockScraper(sName, aHi, aLo, ownNumStocks, boughtPrice);
            scrapers.Add(sName, scraper);
            return scraper;
        }

        //Attractively displays important info about stock. Uses colors (red and green) for change, given in dollars and percent
        static void display(StockScraper scraper)
        {
            string price = scraper.stockPrice.ToString("C", CultureInfo.CurrentCulture);
            string high = scraper.userHigh.ToString("C", CultureInfo.CurrentCulture);
            string low = scraper.userLow.ToString("C", CultureInfo.CurrentCulture);
            string stockName = scraper.getStockName();

            Console.ResetColor();
            Console.Write("Stock:            ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(scraper.stockSym.ToUpper());
            Console.WriteLine(" | " + stockName);
            Console.ResetColor();

            Console.Write("Price:          ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  " + price);
            Console.ResetColor();


            if (scraper.userHigh != 1000000)
            {
                Console.Write("High alert:       ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(high);
                Console.ResetColor();
            }

            if (scraper.userLow != 0)
            {
                Console.Write("Low alert:        ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(low);
                Console.ResetColor();
            }

            if(scraper.numStocks != 0 || scraper.buyPrice != 0)
            {
                Console.Write("Your invetsment:  ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(scraper.numStocks * scraper.stockPrice);
                Console.ResetColor();
            }

            if (scraper.stockHistory.Count >= 4)
            {
                Console.Write("Change:           ");
                double lastTrade = scraper.stockHistory[scraper.stockHistory.Count() - 2];
                double change = scraper.stockPrice - lastTrade;
                if (change > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("+");
                }
                else if (change < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("-");
                }
                else
                    Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(change.ToString("C", CultureInfo.CurrentCulture) + "\t" + (((scraper.stockPrice - lastTrade)/lastTrade) * 100).ToString("0.000") + "%");

                Console.ResetColor();
                Console.Write("History:          ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0}, {1}, {2}", scraper.stockHistory[scraper.stockHistory.Count - 2].ToString("C", CultureInfo.CurrentCulture), scraper.stockHistory[scraper.stockHistory.Count - 3].ToString("C", CultureInfo.CurrentCulture), scraper.stockHistory[scraper.stockHistory.Count - 4].ToString("C", CultureInfo.CurrentCulture));
            }
            Console.ResetColor();


        }

        //checks to see if the current stock price is greater than or equal to the user-defined high stock price.
        static bool checkHigh(StockScraper scraper)
        {
            return (scraper.stockPrice >= scraper.userHigh);
        }

        //checks to see if the current stock price is less than or equal to the user-defined low stock price.
        static bool checkLow(StockScraper scraper)
        {
            return (scraper.userLow >= scraper.stockPrice);
        }
        
        //shows the program logo at the top, above all the stock listings.
        static void displayBanner()
        {
            Console.WriteLine("#######################################################");
            Console.Write("###  ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("$tock$craper--The Standard in Stock Scraping");
            Console.ResetColor();
            Console.WriteLine("  ####");
            Console.WriteLine("#######################################################");
            Console.WriteLine();
        }
    }
}
