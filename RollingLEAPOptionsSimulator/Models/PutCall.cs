using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollingLEAPOptionsSimulator.Models
{
    using System.Xml.Serialization;
    
    public class PutCall
    {

        [XmlElement("option-symbol")]
        public string Symbol { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("underlying-symbol")]
        public string UnderlyingSymbol { get; set; }

        [XmlIgnore]
        public float? Bid
        {
            get
            {
                float floatVal;
                return float.TryParse(BidStr, out floatVal) ? floatVal : default(float);
            }
        }

        [XmlIgnore]
        public float? Ask
        {
            get
            {
                float floatVal;
                return float.TryParse(AskStr, out floatVal) ? floatVal : default(float);
            }
        }

        [XmlElement("bid")]
        public string BidStr { get; set; }

        [XmlElement("ask")]
        public string AskStr { get; set; }

        /**

        [XmlElement("bid-ask-size")]
        public string BidAskSize { get; set; }

        [XmlElement("last")]
        public float Last { get; set; }

        [XmlElement("volume")]
        public long Volume { get; set; }

        [XmlElement("open-interest")]
        public int OpenInterest { get; set; }

        [XmlElement("real-time")]
        public bool IsRealTime { get; set; }

        [XmlElement("delta")]
        public float Delta { get; set; }

        [XmlElement("gamma")]
        public float Gamma { get; set; }

        [XmlElement("theta")]
        public float Theta { get; set; }

        [XmlElement("vega")]
        public float Vega { get; set; }

        [XmlElement("rho")]
        public float Rho { get; set; }

        [XmlElement("implied-volatility")]
        public float ImpliedVolatitily { get; set; }

        [XmlElement("time-value-index")]
        public float TimeValueIndex { get; set; }

        [XmlElement("multiplier")]
        public float Multiplier { get; set; }

    **/
    }
}
