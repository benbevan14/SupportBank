using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public Person To { get; set; }
        public Person From { get; set; }
        public string Narrative { get; set; }
        public float Amount { get; set; }

        public Transaction(DateTime date, Person to, Person from, string narrative, float amount)
        {
            Date = date;
            To = to;
            From = from;
            Narrative = narrative;
            Amount = amount;
        }
    }
}
