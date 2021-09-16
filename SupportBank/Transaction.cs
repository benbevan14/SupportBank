using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupportBank
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        [JsonProperty("FromAccount")]
        public Person From { get; set; }
        [JsonProperty("ToAccount")]
        public Person To { get; set; }
        public string Narrative { get; set; }
        public decimal Amount { get; set; }

        public Transaction(DateTime date, Person to, Person from, string narrative, decimal amount)
        {
            Date = date;
            To = to;
            From = from;
            Narrative = narrative;
            Amount = amount;
        }

        public void Show()
        {
            string message = String.Format("On {0}, {1} paid {2} £{3} for {4}", Date.ToString("d"), To.Name, From.Name, Amount.ToString("0.00"), Narrative);
            Console.WriteLine(message);
        }
    }
}
