
namespace RollingLEAPOptionsSimulator
{
    using Controls;
    using Extensions;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Resources;
    using RollingLEAPOptionsSimulator.Utility;
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
    using System.Web.Script.Serialization;
    using System.Xml.Linq;
    using System.Xml.Serialization;


    /// <summary>
    /// Represents a REST client for interacting with the TD Ameritrade Trading Platform.
    /// </summary>
    public class AmeritradeClient : IDisposable
    {
        private readonly HttpClient http;
        private AuthToken token;

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

            this.http = new HttpClient
            {
                BaseAddress = new Uri("https://api.tdameritrade.com")                 
            };           
        }

        ~AmeritradeClient()
        {
            this.Dispose(false);
        }

        public async Task<bool?> LogIn()
        {
            var appKey =  Settings.GetProtected(Settings.AppKeyKey);
            var token = Settings.GetProtected(Settings.RefreshTokenKey);
           

            if (!string.IsNullOrWhiteSpace(appKey) && !string.IsNullOrWhiteSpace(token))
            {
                return await LogIn(appKey, token);
            } 
            else
            {
                var login = new LoginScreen(this);
                return login.ShowDialog();
            }         
        }

        public async Task<bool> LogIn(string appKey, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException(string.Format(Errors.CannotBeNullOrWhitespace, "refreshToken"), "refreshToken");
            }

            try
            {
                http.DefaultRequestHeaders.Clear();

                var response = await http.PostAsync(
                   "/v1/oauth2/token",
                  new FormUrlEncodedContent(new[]
                  {
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("refresh_token", refreshToken),
                        new KeyValuePair<string, string>("client_id", appKey)
                  }));

                var text = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<AuthToken>(text);


                if (token != null)
                {
                    this.http.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);
                    return true;
                }

            } 
            catch (Exception ex)
            {

            }

          

            
            return false;
        }

        public class AuthToken
        {
            [JsonProperty("access_token")]
            public string  Token { get; set; }
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
            
            var url = "/v1/marketdata/quotes?" +             
                "&symbol=" + string.Join(",", symbols.Select(x => Uri.EscapeDataString(x.Trim())));
            string  text = await this.http.GetStringAsync(url);

            dynamic deserializedProduct = JsonConvert.DeserializeObject<object>(text);
        
            foreach (var ticker in symbols)
            {
                quotes.Add(JsonConvert.DeserializeObject<StockQuote>(deserializedProduct[ticker].ToString()));              
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

            var url = "/v1/marketdata/chains?" + 
                "symbol=" + symbol + "&includeQuotes=true";
            var text = await this.http.GetStringAsync(url);

            OptionQuote optionChain = JsonConvert.DeserializeObject<OptionQuote>(text);
            
            foreach (var optionDate in optionChain.callExpDateMap.Keys)
            {

                DateTime date = DateTime.ParseExact(optionDate.Substring(0,10), "yyyy-MM-dd", null);
               
                foreach (var strikeArr in optionChain.callExpDateMap[optionDate].Values)
                {
                   foreach (var strike in strikeArr)
                    {              
                        try
                        {                           
                            strike.ExpirationDate = date;
                            strike.Underlyer = symbol;
                            quotes.Add(strike);

                        } catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.InnerException?.Message);
                        }
                        
                       

                    }
                                  
                }
                  
            }           
            
            return quotes;

    
        }

        public class OptionQuote
        {
            public Dictionary<string, OptionDate> callExpDateMap { get; set; }
        }

        public class OptionDate : Dictionary<string, OptionStrike[]> { }

        
        
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
    }
}
