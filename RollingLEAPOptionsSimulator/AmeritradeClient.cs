
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

        public void LogIn()
        {
            var login = new LoginScreen(this);
            login.ShowDialog();
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

        public async Task KeepAlive()
        {
            this.EnsureIsAuthenticated();

            var text = await this.http.GetStringAsync("/apps/KeepAlive?source=" + Uri.EscapeDataString(this.key));

            if (text != "LoggedOn")
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

            this.EnsureIsAuthenticated();

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
             
            this.EnsureIsAuthenticated();

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

        public Task<Dictionary<string, Quote[]>> GetHistoricalPrices(string symbol, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }

            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException(string.Format(Errors.CannotBeEmpty, "symbol"), "symbol");
            }

            return this.GetHistoricalPrices(new[] { symbol }, startDate: startDate, endDate: endDate);
        }

        public async Task<Dictionary<string, Quote[]>> GetHistoricalPrices(string[] symbols, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException("symbols");
            }

            if (symbols.Length == 0)
            {
                throw new ArgumentException(string.Format(Errors.CannotBeEmpty, "symbols"), "symbols");
            }

            this.EnsureIsAuthenticated();

            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            var fromDate = startDate.HasValue ? startDate.Value : now;
            var toDate = endDate.HasValue ? endDate.Value : now;

            var url = "/apps/100/PriceHistory?source=" + Uri.EscapeDataString(this.key) + "&requestidentifiertype=SYMBOL" +
                "&requestvalue=" + Uri.EscapeDataString(string.Join(",", symbols)) +
                "&intervaltype=DAILY&intervalduration=1&startdate=" + fromDate.ToString("yyyyMMdd") + "&enddate=" + toDate.ToString("yyyyMMdd");

            var result = new Dictionary<string, Quote[]>();

            using (var stream = await this.http.GetStreamAsync(url))
            using (var reader = new BinaryReader(stream))
            {
                var numSymbols = reader.ReadInt32BE();

                for (var i = 0; i < numSymbols; i++)
                {
                    var symbol = Encoding.ASCII.GetString(reader.ReadBytes(reader.ReadInt16BE()));

                    if (reader.ReadByte() == 1 /* has error */)
                    {
                        var error = Encoding.ASCII.GetString(reader.ReadBytes(reader.ReadInt16BE()));
                        
                        // TODO: Handle error
                        continue;
                    }

                    var numQuotes = reader.ReadInt32BE();
                    var quotes = new Quote[numQuotes];

                    for (var j = numQuotes - 1; j > -1; j--)
                    {
                        quotes[j] = new Quote
                        {
                            Close = reader.ToSingleBE(),
                            High = reader.ToSingleBE(),
                            Low = reader.ToSingleBE(),
                            Open = reader.ToSingleBE(),
                            Volume = reader.ToSingleBE(),
                            Date = new DateTime(1970, 1, 1).AddSeconds(reader.ReadInt64BE() / 1000)
                        };
                    }

                    result.Add(symbol, quotes);

                    if (i < numSymbols - 1)
                    {
                        var termBytes = reader.ReadBytes(2);

                        if (!termBytes.SequenceEqual(new byte[] { 255, 255 }))
                        {
                            throw new ApplicationException(Errors.UnexpectedEnfOfPriceData);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<Dictionary<string, string>> FindSymbols(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                throw new ArgumentException(string.Format(Errors.CannotBeNullOrWhitespace, "search"), "search");
            }

            this.EnsureIsAuthenticated();

            var text = await this.http.GetStringAsync(
                "/apps/100/SymbolLookup?source=" + Uri.EscapeDataString(this.key) +
                "&matchstring=" + Uri.EscapeDataString(search));

            var xml = XDocument.Parse(text);
            var result = new Dictionary<string, string>();

            if (xml.Root.Element("result").Value == "OK")
            {
                foreach (var symbol in xml.Root.Element("symbol-lookup-result").Elements("symbol-result"))
                {
                    result.Add(symbol.Element("symbol").Value, symbol.Element("description").Value);
                }
            }

            return result;
        }

        public Task<bool> CancelOrder(string orderID)
        {
            return CancelOrder(null, orderID);
        }


        public async Task<bool> CancelOrder(string accountID, string orderID)
        {
            if (orderID == null)
            {
                throw new ArgumentNullException("orderID");
            }

            if (orderID.Trim() == string.Empty)
            {
                throw new ArgumentException(string.Format(Errors.CannotBeEmpty, "orderID"), "orderID");
            }

            if (accountID != null && accountID.Trim() == string.Empty)
            {
                throw new ArgumentException(string.Format(Errors.CannotBeEmpty, "accountID"), "accountID");
            }

            this.EnsureIsAuthenticated();

            var url = "/apps/100/OrderCancel?source=" + Uri.EscapeDataString(this.key) +
                      (string.IsNullOrWhiteSpace(accountID) ? string.Empty : "&accountid=" + accountID) +
                      "&orderid=" + Uri.EscapeDataString(orderID);

            var xml = await this.http.GetXmlAsync(url);

            return xml.Root.Element("result").Value != "OK" && xml.Root.Element("order").Element("error").Value == string.Empty;
        }

        public async Task<List<Watchlist>> GetWatchlists()
        {
            this.EnsureIsAuthenticated();

            var xml = await this.http.GetXmlAsync("/apps/100/GetWatchlists?source=" + Uri.EscapeDataString(this.key));

            if (xml.Root.Element("result").Value != "OK")
            {
                throw new ApplicationException();
            }

            var list = new List<Watchlist>();

            foreach (var node in xml.Root.Element("watchlist-result").Elements("watchlist"))
            {
                using (var reader = node.CreateReader())
                {
                    list.Add((Watchlist)new XmlSerializer(typeof(Watchlist)).Deserialize(reader));
                }
            }

            return list;
        }

        public Task<Watchlist> CreateWatchlist(string name, params string[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            return this.CreateWatchlist(name, items.Select(x => new WatchlistItem { Symbol = x }).ToArray());
        }

        public async Task<Watchlist> CreateWatchlist(string name, params WatchlistItem[] items)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Trim() == string.Empty)
            {
                throw new ArgumentException(string.Format(Errors.CannotBeEmpty, "name"), "name");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Length == 0)
            {
                throw new ArgumentException(string.Format(Errors.CannotBeEmpty, "items"), "items");
            }

            this.EnsureIsAuthenticated();

            var xml = await this.http.GetXmlAsync("/apps/100/CreateWatchlist?source=" + Uri.EscapeDataString(this.key) +
                      "&watchlistname=" + Uri.EscapeDataString(name) +
                      "&symbollist=" + string.Join(",", items.Select(x => Uri.EscapeDataString(x.Symbol))));

            if (xml.Root.Element("result").Value != "OK")
            {
                throw new ApplicationException();
            }

            using (var reader = xml.Root.Element("created-watchlist").CreateReader())
            {
                return (Watchlist)new XmlSerializer(typeof(Watchlist)).Deserialize(reader);
            }
        }

        public async Task<bool> DeleteWatchlists(string watchlistID, string accountID = null)
        {
            if (watchlistID == null)
            {
                throw new ArgumentNullException("watchlistID");
            }

            if (watchlistID.Trim() == string.Empty)
            {
                throw new ArgumentException(string.Format(Errors.CannotBeEmpty, "watchlistID"), "watchlistID");
            }

            if (accountID != null && accountID.Trim() == string.Empty)
            {
                throw new ArgumentException(string.Format(Errors.CannotBeEmpty, "accountID"), "accountID");
            }

            this.EnsureIsAuthenticated();

            var url = "/apps/100/DeleteWatchlist?source=" +
                      Uri.EscapeDataString(this.key) + "&listid=" + Uri.EscapeDataString(watchlistID) +
                      (string.IsNullOrWhiteSpace(accountID) ? string.Empty : "&accountid=" + accountID);
            
            var xml = await this.http.GetXmlAsync(url);

            return xml.Root.Element("result").Value != "OK";
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

        private Object loginScreenLock = new Object();

        protected void EnsureIsAuthenticated()
        {
           
            if (!this.IsAuthenticated)
            {
                lock (loginScreenLock)
                {
                    if (!this.IsAuthenticated)
                    {
                        Thread thread = new Thread(LogIn);
                        thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                        thread.Start();
                        thread.Join(); //Wait for the thread to end   
                    }                              
                }                    
            }

            if (!this.IsAuthenticated)
            {
                throw new AccessViolationException();
            }
        }
    }
}
