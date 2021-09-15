using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank
{
    public class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            // Set up NLog
            var config = new LoggingConfiguration();
            var target = new FileTarget
            {
                FileName = @"C:/Users/benjaminb/Work/Logs/SupportBank.log",
                Layout = @"${longdate} ${level} - ${logger}: ${message}"
            };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            Logger.Debug("The program has started");

            Console.Write("(List All) balances or (List [name]) the transactions for a specific user: ");
            string option = Console.ReadLine();

            Dictionary<string, Person> balances = CalculateBalances();

            if (option == "List All")
            {
                ListAll(balances);
            }
            else if (Regex.IsMatch(option, @"List \[\w+\s*\w+\]"))
            {
                string name = option.Split('[', ']')[1];
                Console.WriteLine("Listing transactions for " + name);
                balances[name].ShowTransactions();
            }
            else
            {
                Console.WriteLine("Invalid option entered");
            }

            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static Dictionary<string, Person> CalculateBalances()
        {
            string path = "C:/Users/benjaminb/Work/Training/SupportBank/DodgyTransactions2015.csv";

            string[] rows = File.ReadAllLines(path).Skip(1).ToArray();

            // Dictionary of unique people referenced in the file, accessed by name
            Dictionary<string, Person> people = new Dictionary<string, Person>();

            foreach (string row in rows)
            {
                // Get the values for the row
                string[] rowVals = row.Split(',');
                DateTime transactionDate = new DateTime();
                try
                {
                    transactionDate = DateTime.Parse(rowVals[0]);
                }
                catch
                {
                    Logger.Debug("Invalid date");
                }
                string pFrom = rowVals[1];
                string pTo = rowVals[2];
                string narrative = rowVals[3];
                decimal transactionAmount = 0;
                try
                {
                    transactionAmount = Convert.ToDecimal(rowVals[4]);
                }
                catch
                {
                    Logger.Debug("Amount is not a number");
                }

                // Check whether the people in the current transaction are already in the dictionary of people
                // If not, add them
                if (!people.TryGetValue(pFrom, out Person pf))
                {
                    people.Add(pFrom, new Person(pFrom));
                }
                if (!people.TryGetValue(pTo, out Person pt))
                {
                    people.Add(pTo, new Person(pTo));
                }

                // Create a transaction object
                Transaction t = new Transaction(transactionDate, people[pFrom], people[pTo], narrative, transactionAmount);

                // Add the transaction to pFrom and pTo's list
                people[pFrom].AddTransaction(t);
                people[pTo].AddTransaction(t);

                // Deduct the transaction amount from pFrom
                people[pFrom].Deduct(t.Amount);

                // Add the transaction amount to pTo
                people[pTo].Pay(t.Amount);
            }

            /*
            foreach (var p in people)
            {
                Console.WriteLine(p.Key + ": " + p.Value.Balance);
            }
            */

            return people;
        }

        private static void ListAll(Dictionary<string, Person> balances)
        {
            foreach (var item in balances)
            {
                if (item.Value.Balance > 0)
                {
                    Console.WriteLine($"{item.Value.Name} is owed £{item.Value.Balance}");
                }
                else
                {
                    Console.WriteLine($"{item.Value.Name} owes £{Math.Abs(item.Value.Balance)}");
                }
            }
        }
    }
}
