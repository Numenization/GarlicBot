using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GarlicBot.Modules.Quotes
{
    class QuoteManager
    {
        static QuoteManager()
        {
            quotes = new List<Quote>();
            writer = File.AppendText($"{resourcesFolder}/{quotesFile}");
            ReadCSV($"{resourcesFolder}/{quotesFile}");
        }

        public static void AddQuote(Quote quote)
        {
            quotes.Add(quote);
            writer.Write(QuoteToCSV(quote));
            writer.Flush();
        }

        public static Quote GetRandomQuote()
        {
            Random rand = new Random();
            Quote quote = quotes[rand.Next() % quotes.Count];
            return quote;
        }

        public static string GetRandomQuoteString()
        {
            Random rand = new Random();
            Quote quote = quotes[rand.Next() % quotes.Count];
            return quote.ToString();
        }

        public static void PrintAllQuotes()
        {
            foreach(Quote quote in quotes)
            {
                Console.WriteLine(quote.ToString());
            }
        }

        public static void ReadCSV(string filepath)
        {
            writer.Close();
            string str = File.ReadAllText(filepath);
            string[] quotesArray = str.Split('\0', StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < quotesArray.Length; i = i + 3)
            {
                quotes.Add(new Quote
                {
                    author = quotesArray[i],
                    text = quotesArray[i + 1],
                    date = quotesArray[i + 2]
                });
            }
            writer =  File.AppendText($"{resourcesFolder}/{quotesFile}");
        }

        public static string QuotesToCSV(List<Quote> quoteList)
        {
            string str = "";
            foreach(Quote quote in quoteList)
            {
                str += $"{quote.author}\0{quote.text}\0{quote.date}\0";
            }
            return str;
        }

        public static string QuoteToCSV(Quote quote)
        {
            return $"{quote.author}\0{quote.text}\0{quote.date}\0";
        }

        public static List<Quote> quotes;

        private static string resourcesFolder = "Resources";
        private static string quotesFile = "quotes.csv";
        private static StreamWriter writer;
    }

    public class Quote
    {
        public string text = "None";
        public string author = "None";
        public string date = "0/0/0 [0:0]";

        public override string ToString()
        {
            return $"[{author}, {date}] {text}";
        }
    }
}
