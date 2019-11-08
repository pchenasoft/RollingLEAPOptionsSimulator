// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StockQuote.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RollingLEAPOptionsSimulator.Models
{
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    [XmlRoot("quote", Namespace = "")]
    public class StockQuote 
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("bidPrice")]
        public float Bid { get; set; }

        [JsonProperty("askPrice")]
        public float Ask { get; set; }

        [JsonProperty("closePrice")]
        public float Close { get; set; }

        [JsonProperty("netChange")]
        public float Change { get; set; }

        [JsonProperty("lastPrice")]
        public float Last { get; set; }


        /**

        [XmlElement("bid-ask-size")]
        public string BidAskSize { get; set; }

        [XmlElement("last-trade-size")]
        public int LastTradeSize { get; set; }

        [XmlElement("last-trade-date")]
        public string LastTradeDate { get; set; }

        [XmlElement("open")]
        public float Open { get; set; }

        [XmlElement("high")]
        public float High { get; set; }

        [XmlElement("low")]
        public float Low { get; set; }

        [XmlElement("volume")]
        public long Volume { get; set; }

        [XmlElement("year-high")]
        public float YearHigh { get; set; }

        [XmlElement("year-low")]
        public float YearLow { get; set; }

        [XmlElement("real-time")]
        public bool IsRealTime { get; set; }

        [XmlElement("exchange")]
        public string Exchange { get; set; }

        [XmlElement("change-percent")]
        public string ChangePercent { get; set; }

    **/
    }
}
