using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
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
            // #######################################
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
            // #######################################

            Logger.Debug("The program has started");

            // Read in the option to list balances for all users or a specific user
            Console.Write("(List All) balances or (List [name]) the transactions for a specific user: ");
            string option = Console.ReadLine();

            // Path to the file to read data from
            string path = "C:/Users/benjaminb/Work/Training/SupportBank/Transactions2013.json";

            List<Person> balances = CalculateBalances(path);

            if (option == "List All")
            {
                ListAll(balances);
            }
            else if (Regex.IsMatch(option, @"List \[\w+\s*\w+\]"))
            {
                string name = option.Split('[', ']')[1];
                Console.WriteLine("Listing transactions for " + name);
                balances.Find(i => i.Name == name).ShowTransactions();
            }
            else
            {
                Console.WriteLine("Invalid option entered");
            }

            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static List<Person> CalculateBalances(string path)
        {
            List<Transaction> transactions = GetTransactions(path);

            // Dictionary of unique people referenced in the file, accessed by name
            Dictionary<string, Person> people = new Dictionary<string, Person>();

            foreach (Transaction t in transactions)
            {
                string fromName = t.From.Name;
                string toName = t.To.Name;
                // Check whether the people in the current transaction are already in the dictionary of people
                // If not, add them
                if (!people.TryGetValue(fromName, out Person pf))
                {
                    people.Add(fromName, t.From);
                }
                if (!people.TryGetValue(toName, out Person pt))
                {
                    people.Add(toName, t.To);
                }

                // Add the transaction to pFrom and pTo's list
                people[fromName].AddTransaction(t);
                people[toName].AddTransaction(t);

                // Deduct the transaction amount from pFrom
                people[fromName].Deduct(t.Amount);

                // Add the transaction amount to pTo
                people[toName].Pay(t.Amount);
            }

            /*
            foreach (var p in people)
            {
                Console.WriteLine(p.Key + ": " + p.Value.Balance);
            }
            */

            return people.Values.ToList();
        }

        private static List<Transaction> GetTransactions(string path)
        {
            List<Transaction> transactions = new List<Transaction>();

            if (path.EndsWith(".csv"))
            {
                string[] rows = File.ReadAllLines(path).Skip(1).ToArray();

                foreach (string row in rows)
                {
                    string[] vals = row.Split(',');
                    DateTime date = new DateTime();
                    try
                    {
                        date = DateTime.Parse(vals[0]);
                    }
                    catch
                    {
                        Logger.Debug("Invalid date");
                    }
                    string pFrom = vals[1];
                    string pTo = vals[2];
                    string narrative = vals[3];
                    decimal amount = 0;
                    try
                    {
                        amount = Convert.ToDecimal(vals[4]);
                    }
                    catch
                    {
                        Logger.Debug("Amount is not a number");
                    }

                    transactions.Add(new Transaction(date, new Person(pFrom), new Person(pTo), narrative, amount));
                }

                return transactions;
            }
            else if (path.EndsWith(".json"))
            {
                transactions = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(path));
            }
            return transactions;
        }

        private static void ListAll(List<Person> balances)
        {
            foreach (var item in balances)
            {
                if (item.Balance > 0)
                {
                    Console.WriteLine($"{item.Name} is owed £{item.Balance}");
                }
                else
                {
                    Console.WriteLine($"{item.Name} owes £{Math.Abs(item.Balance)}");
                }
            }
        }
    }
}
