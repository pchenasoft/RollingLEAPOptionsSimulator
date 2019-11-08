using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollingLEAPOptionsSimulator.Models
{
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    [XmlRoot("option-strike", Namespace = "")]
    public class OptionStrike
    {


        [JsonProperty("putCall")]
        public string putCall { get; set; }


        [JsonIgnore]
        public bool IsCall => putCall == "CALL";

        [JsonProperty("strikePrice")]
        public float StrikePrice { get; set; }


        [JsonProperty("nonStandard")]
        public bool NonStandard { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonIgnore]
        public float? Bid => getFloat(BidStr);

        [JsonIgnore]
        public float? Ask => getFloat(AskStr);

        [JsonIgnore]
        public float? Delta => getFloat(DeltaStr);

        [JsonIgnore]
        public float? Gamma => getFloat(GammaStr);

        [JsonIgnore]
        public float? Theta => getFloat(ThetaStr);

        [JsonIgnore]
        public float? Vega => getFloat(VegaStr);

        [JsonIgnore]
        public float? Rho => getFloat(RhoStr);

        [JsonIgnore]
        public float? ImpliedVolatitily => getFloat(ImpliedVolatitilyStr);


        [JsonProperty("bid")]
        public string BidStr { get; set; }

        [JsonProperty("ask")]
        public string AskStr { get; set; }


        [JsonProperty("delta")]
        public string DeltaStr { get; set; }

        [JsonProperty("gamma")]
        public string GammaStr { get; set; }

        [JsonProperty("theta")]
        public string ThetaStr { get; set; }

        [JsonProperty("vega")]
        public string VegaStr { get; set; }

        [JsonProperty("rho")]
        public string RhoStr { get; set; }

        [JsonProperty("volatility")]
        public string ImpliedVolatitilyStr { get; set; }


        [JsonIgnore]
        public DateTime ExpirationDate { get; set; }

        [JsonIgnore]
        public string Underlyer { get; set; }


        private float? getFloat(string floatVal)
        {
            float result;
            return float.TryParse(floatVal, out result) ? result : default(float);
        }

    }
}
