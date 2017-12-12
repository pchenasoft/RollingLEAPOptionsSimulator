
namespace RollingLEAPOptionsSimulator
{
    using Controls;
    using Extensions;
    using Models;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.Serialization;


    /// <summary>
    /// Represents a REST client for interacting with the TD Ameritrade Trading Platform.
    /// </summary>
    public class AmeritradeClient : IDisposable
    {
        private readonly HttpClient http;

        private string key;
        private readonly string name;
        private readonly string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmeritradeClient"/> class.
        /// </summary>
        /// <param name="key">Organization's unique identifier to be passed as part of every request to the TD Ameritrade Trading Platform.</param>
        /// <param name="name">Organization's name to be passed to the TD Ameritrade Trading Platform during authentication.</param>
        /// <param name="version">The package's version to be passed to the TD Ameritrade Trading Platform during authentication.</param>
        public AmeritradeClient(string name = "TD Ameritrade Client Library for .NET", string version = "2.0.0")
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(string.Format(Errors.CannotBeNullOrWhitespace, "name"));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException(string.Format(Errors.CannotBeNullOrWhitespace, "version"));
            }

            this.name = name;
            this.version = version;

            this.http = new HttpClient
            {
                BaseAddress = new Uri("https://apis.tdameritrade.com")
            };

            this.Reset();
        }

        ~AmeritradeClient()
        {
            this.Dispose(false);
        }

        public bool IsAuthenticated { get; private set; }

        public bool? LogIn()
        {
            var login = new LoginScreen(this);
            return login.ShowDialog();
        }

        public async Task<bool> LogIn(string userName, string password, string sourceID)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(string.Format(Errors.CannotBeNullOrWhitespace, "userName"), "userName");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(string.Format(Errors.CannotBeNullOrWhitespace, "password"), "password");
            }

            if (string.IsNullOrWhiteSpace(sourceID))
            {
                throw new ArgumentException(string.Format(Errors.CannotBeNullOrWhitespace, "sourceID"), "sourceID");
            }

            this.key = sourceID;

            var response = await this.http.PostAsync(
                "/apps/300/LogIn?source=" + Uri.EscapeDataString(this.key) + "&version=" + Uri.EscapeDataString(this.version),
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("userid", userName),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("source", this.key),
                    new KeyValuePair<string, string>("version", this.version)
                }));

            userName = password = null;
            var text = await response.Content.ReadAsStringAsync();
            var xml = XDocument.Parse(text);

            if (this.IsAuthenticated = xml.Root.Element("result").Value == "OK")
            {            
                return true;
            }

            this.Reset();
            return false;
        }

        public async Task LogOut()
        {
            var text = await this.http.GetStringAsync("/apps/100/LogOut?source=" + Uri.EscapeDataString(this.key));
            var xml = XDocument.Parse(text);

            if (xml.Root.Element("result").Value == "LoggedOut")
            {
                this.Reset();
            }
        }

        public void KeepAlive()
        {
            if (!IsAuthenticated)
            {
                return;
            }

            var task =  this.http.GetStringAsync("/apps/KeepAlive?source=" + Uri.EscapeDataString(this.key));
            task.Wait();

            if (task.IsCompleted &&  task.Result != "LoggedOn")
            {
                this.Reset();
            }
        }

        public async Task<List<object>> GetQuotes(params string[] symbols)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException("symbols");
            }

            var quotes = new List<object>();

            if (symbols.Length == 0)
            {
                return quotes;
            }

            if (!IsAuthenticated)
            {
                return quotes;
            }

            var url = "/apps/100/Quote?source=" + Uri.EscapeDataString(this.key) +
                "&symbol=" + string.Join(",", symbols.Select(x => Uri.EscapeDataString(x.Trim())));
            var text = await this.http.GetStringAsync(url);
            var xml = XDocument.Parse(text);

            if (xml.Root.Element("result").Value != "OK")
            {
                throw new Exception();
            }

            foreach (var quoteNode in xml.Root.Element("quote-list").Elements("quote"))
            {
                using (var reader = quoteNode.CreateReader())
                {
                    if (quoteNode.Element("error").Value != string.Empty)
                    {
                        quotes.Add((ErrorQuote)new XmlSerializer(typeof(ErrorQuote)).Deserialize(reader));
                        continue;
                    }

                    switch (quoteNode.Element("asset-type").Value)
                    {
                        case "E":
                            quotes.Add((StockQuote)new XmlSerializer(typeof(StockQuote)).Deserialize(reader));
                            break;
                        case "O":
                            quotes.Add((OptionQuote)new XmlSerializer(typeof(OptionQuote)).Deserialize(reader));
                            break;
                        case "I":
                            quotes.Add((IndexQuote)new XmlSerializer(typeof(IndexQuote)).Deserialize(reader));
                            break;
                        case "F":
                            quotes.Add((FundQuote)new XmlSerializer(typeof(FundQuote)).Deserialize(reader));
                            break;
                        default:
                            quotes.Add((ErrorQuote)new XmlSerializer(typeof(ErrorQuote)).Deserialize(reader));
                            break;
                    }
                }
            }

            return quotes;
        }

        public async Task<List<object>> GetOptionChain(string symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbols");
            }

            var quotes = new List<object>();


            if (!IsAuthenticated)
            {
                return quotes;
            }

            var url = "/apps/200/OptionChain?source=" + Uri.EscapeDataString(this.key) +
                "&symbol=" + symbol + "&quotes=true";
            var text = await this.http.GetStringAsync(url);
            var xml = XDocument.Parse(text);

            if (xml.Root.Element("result").Value != "OK")
            {
                throw new Exception();
            }

            foreach (var optionDate in xml.Root.Element("option-chain-results").Elements("option-date"))
            {

                DateTime date = DateTime.ParseExact(optionDate.Element("date").Value, "yyyyMMdd", null);
               

                foreach (var optionStrike in optionDate.Elements("option-strike"))
                {
                    using (var reader = optionStrike.CreateReader())
                    {
                        OptionStrike strike = (OptionStrike)new XmlSerializer(typeof(OptionStrike)).Deserialize(reader);
                        strike.ExpirationDate = date;
                        quotes.Add(strike);
                    }

                }
                  
            }           

            return quotes;
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool freeManagedObjects)
        {
            if (freeManagedObjects)
            {
                if (this.http != null)
                {
                    this.http.Dispose();
                }              
            }
        }

        protected void Reset()
        {
            this.IsAuthenticated = false;
        }
    }
}
