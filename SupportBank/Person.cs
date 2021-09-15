using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    public class Person
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public List<Transaction> Transactions { get; set; }

        public Person(string name)
        {
            Name = name;
            Balance = 0;
            Transactions = new List<Transaction>();
        }

        public void Pay(decimal amount)
        {
            Balance += amount;
        }

        public void Deduct(decimal amount)
        {
            Balance -= amount;
        }

        public void AddTransaction(Transaction t)
        {
            Transactions.Add(t);
        }

        public void ShowTransactions()
        {
            foreach (Transaction t in Transactions)
            {
                t.Show();
            }
        }
    }
}
