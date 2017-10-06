
namespace RollingLEAPOptionsSimulator.Models
{
    using System;

    public class Quote
    {
        public DateTime Date { get; set; }

        public float Open { get; set; }

        public float High { get; set; }

        public float Low { get; set; }

        public float Close { get; set; }

        public float Volume { get; set; }
    }
}
