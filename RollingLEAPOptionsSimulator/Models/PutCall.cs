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
        public float? Bid  => getFloat(BidStr);
       
        [XmlIgnore]
        public float? Ask => getFloat(AskStr);
       
        [XmlIgnore]
        public float? Delta => getFloat(DeltaStr);

        [XmlIgnore]
        public float? Gamma => getFloat(GammaStr);

        [XmlIgnore]
        public float? Theta => getFloat(ThetaStr);

        [XmlIgnore]
        public float? Vega => getFloat(VegaStr);

        [XmlIgnore]
        public float? Rho => getFloat(RhoStr);

        [XmlIgnore]
        public float? ImpliedVolatitily => getFloat(ImpliedVolatitilyStr);


        [XmlElement("bid")]
        public string BidStr { get; set; }

        [XmlElement("ask")]
        public string AskStr { get; set; }


        [XmlElement("delta")]
        public string DeltaStr { get; set; }

        [XmlElement("gamma")]
        public string GammaStr { get; set; }

        [XmlElement("theta")]
        public string ThetaStr { get; set; }

        [XmlElement("vega")]
        public string VegaStr { get; set; }

        [XmlElement("rho")]
        public string RhoStr { get; set; }

        [XmlElement("implied-volatility")]
        public string ImpliedVolatitilyStr { get; set; }

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
        private float? getFloat(string floatVal)
        {
            float result;
            return float.TryParse(floatVal, out result) ? result : default(float);
        }
    }
}
