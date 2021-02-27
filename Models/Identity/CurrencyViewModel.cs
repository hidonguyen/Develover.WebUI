using System;

namespace Develover.WebUI.Models
{
    public class CurrencyViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double ExchangeRate { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }

    }
}
