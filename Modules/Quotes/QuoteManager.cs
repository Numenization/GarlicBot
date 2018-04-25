using System;
using System.Collections.Generic;
using System.IO;

namespace GarlicBot.Modules.Quotes
{
    /// <summary>
    /// Class used to get and add quotes
    /// </summary>
    class QuoteManager
    {
        /// <summary>
        /// Class used to get and add quotes
        /// </summary>
        static QuoteManager()
        {
            quotes = new List<Quote>();
            writer = File.AppendText($"{resourcesFolder}/{quotesFile}");
            ReadCSV($"{resourcesFolder}/{quotesFile}");
        }

        /// <summary>
        /// Adds a quote to the quote database
        /// </summary>
        /// <param name="quote">Object containing author, message, and time sent</param>
        public static void AddQuote(Quote quote)
        {
            quotes.Add(quote);
            writer.Write(QuoteToCSV(quote));
            writer.Flush();
        }

        /// <summary>
        /// Gets a random quote from the database
        /// </summary>
        /// <returns>Object containing author, message, and time sent</returns>
        public static Quote GetRandomQuote()
        {
            Random rand = new Random();
            Quote quote = quotes[rand.Next() % quotes.Count];
            return quote;
        }

        /// <summary>
        /// Gets a random quote from the database and returns as a string
        /// </summary>
        /// <returns>String with quote information</returns>
        public static string GetRandomQuoteString()
        {
            Random rand = new Random();
            Quote quote = quotes[rand.Next() % quotes.Count];
            return quote.ToString();
        }

        /// <summary>
        /// Prints all the quotes to the console
        /// </summary>
        public static void PrintAllQuotes()
        {
            foreach(Quote quote in quotes)
            {
                Console.WriteLine(quote.ToString());
            }
        }

        /// <summary>
        /// Parses quotes from a CSV file deliniated by null characters
        /// </summary>
        /// <param name="filepath">File path of the CSV file</param>
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

        /// <summary>
        /// Converts the current quote list to a string CSV format
        /// </summary>
        /// <param name="quoteList">The list of quotes to convert</param>
        /// <returns>The output in CSV form</returns>
        public static string QuotesToCSV(List<Quote> quoteList)
        {
            string str = "";
            foreach(Quote quote in quoteList)
            {
                str += $"{quote.author}\0{quote.text}\0{quote.date}\0";
            }
            return str;
        }

        /// <summary>
        /// Converts a single quote to CSV format
        /// </summary>
        /// <param name="quote">Quote to convert</param>
        /// <returns>The output in CSV form</returns>
        public static string QuoteToCSV(Quote quote)
        {
            return $"{quote.author}\0{quote.text}\0{quote.date}\0";
        }

        /// <summary>
        /// The current list of quotes
        /// </summary>
        public static List<Quote> quotes;

        private static string resourcesFolder = "Resources";
        private static string quotesFile = "quotes.csv";
        private static StreamWriter writer;
    }

    /// <summary>
    /// Object containing quote information
    /// </summary>
    public class Quote
    {
        /// <summary>
        /// The message represented by the quote
        /// </summary>
        public string text = "None";
        /// <summary>
        /// The author of the message
        /// </summary>
        public string author = "None";
        /// <summary>
        /// The date and time the quote was sent
        /// </summary>
        public string date = "0/0/0 [0:0]";

        public override string ToString()
        {
            return $"[{author}, {date}] {text}";
        }
    }
}
