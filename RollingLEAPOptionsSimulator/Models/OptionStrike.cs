using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollingLEAPOptionsSimulator.Models
{
    using System.Xml.Serialization;

    [XmlRoot("option-strike", Namespace = "")]
    public class OptionStrike
    {

        [XmlElement("strike-price")]
        public float StrikePrice { get; set; }


        [XmlElement("standard-option")]
        public bool StandardOption { get; set; }

        [XmlElement("put")]
        public Put Put { get; set; }

        [XmlElement("call")]
        public Call Call { get; set; }

        [XmlIgnore]
        public DateTime ExpirationDate { get; set; }

    }
}
