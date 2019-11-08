
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ZedGraph;
using System;
using System.Runtime.InteropServices;
using RollingLEAPOptionsSimulator.Models;
using RollingLEAPOptionsSimulator.Utility;

namespace RollingLEAPOptionsSimulator
{
    public partial class Form1 : Form
    {
        public AmeritradeClient TDAmeritradeClient;
        private Excel.Workbook _workbook;
        private Excel.Application _xlApp;
        private Excel._Worksheet _mainWorksheet;
        private Excel._Worksheet _pnlSheet;



        private string FilePathKey = "FilePath";
        string path;

        private Dictionary<string, List<OptionQuote>> options;
        private List<StockQuote> quotes;

        public Form1()
        {
            TDAmeritradeClient = new AmeritradeClient();
            InitializeComponent();
            info("Starting application...");
            path = Settings.GetProtected(FilePathKey);
            fileLabel.Text = path;

        }

        void ThisWorkbook_BeforeClose(ref bool Cancel)
        {
            this.Invoke((MethodInvoker)delegate
            {
                Close();
            });
        }

        public void info(string v)
        {
            error(v, null);
        }

        public void error(string text, Exception ex)
        {
            string timestamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss - ");
            string threadId = "Thread " + Thread.CurrentThread.ManagedThreadId + ": ";

            if (InvokeRequired)
            {
                text = timestamp + threadId + text;
                this.Invoke(new Action<string, Exception>(error), new object[] { text, ex });
                return;
            }
            else if (!text.Contains("Thread"))
            {
                text = timestamp + threadId + text;
            }

            if (ex != null)
            {
                text += "\r\n" + ex.Message;
                text += "\r\n" + ex.StackTrace;
            }

            output.AppendText(text + "\r\n");
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        public override void Refresh()
        {

            Settings.SetProtected(FilePathKey, path);

            TDAmeritradeClient.KeepAlive();

            if (!TDAmeritradeClient.IsAuthenticated && (!TDAmeritradeClient.LogIn() ?? true))
            {
                error("Not logged in!", null);
                return;
            }

            Task<List<object>> stocksTasks = null;
            List<Task<List<object>>> optionsTask = null;

            try
            {
                stocksTasks = RefreshStocks();
            }
            catch (Exception ex)
            {
                error("Unable to refresh stock quotes", ex);
                return;
            }

            try
            {
                optionsTask = RefreshOptions();

            }
            catch (Exception ex)
            {
                error("Unable to refresh options", ex);
                return;
            }

            GetExcel().Visible = true;


            try
            {
                HandleStockQuote(stocksTasks);
            }
            catch (Exception ex)
            {
                error("Unable to refresh stock quotes", ex);                
            }


            foreach (Task<List<object>> task in optionsTask)
            {
                try
                {
                    HandleOptionChain(task);
                }
                catch (Exception ex)
                {
                    error("Unable to refresh opton quote", ex);
                }
            }
        }


        private Task<List<object>> RefreshStocks()
        {
            List<String> symbols = new List<string>();

            lock (excelLock)
            {
                info("");
                info("Locked Excel. Getting stock quotes symbols.");
                GetMainWorkSheet().Range["N2:P8"].Font.Color = ColorTranslator.ToOle(Color.Red);
                for (int row = 2; row < 9; row++)
                {
                    string symbol = (string)(GetMainWorkSheet().Cells[row, 3] as Excel.Range).Value;
                    if (!string.IsNullOrEmpty(symbol))
                    {
                        symbols.Add(symbol);
                    }
                }
            }
            info("Unlocked Excel. Sending stock quotes request");
            return TDAmeritradeClient.GetQuotes(symbols.ToArray());                     
        }




        private Excel.Application GetExcel()
        {
            if (_xlApp == null)
            {
                _xlApp = new Excel.Application();
            }
            return _xlApp;
        }

        private Excel.Workbook GetWorkBook()
        {
            if (_workbook == null)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    _workbook = GetExcel().Workbooks.Open(path);
                    _workbook.BeforeClose += ThisWorkbook_BeforeClose;
                }
                else
                {
                    throw new Exception("Excel file path is missing.");
                }
            }
            return _workbook;
        }

        private Excel._Worksheet GetMainWorkSheet()
        {
            if (_mainWorksheet == null)
            {
                _mainWorksheet = (Excel._Worksheet)GetWorkBook().Sheets["Main"];
            }
            return _mainWorksheet;
        }

        private Excel._Worksheet GetPnLWorksheetSheet()
        {
            if (_pnlSheet == null)
            {
                _pnlSheet = (Excel._Worksheet)GetWorkBook().Sheets["PNL"];
            }
            return _pnlSheet;
        }


        private List<Task<List<object>>> RefreshOptions()
        {
            List<string> symbols = new List<string>();
            List<Task<List<object>>> tasks = new List<Task<List<object>>>();

            lock (excelLock)
            {                 
                info("Locked Excel. Getting option symbols.");
                GetMainWorkSheet().Range["C2:C8"].Font.Color = ColorTranslator.ToOle(Color.Red);

                for (int row = 2; row < 9; row++)
                {
                    string symbol = (string)(GetMainWorkSheet().Cells[row, 3] as Excel.Range).Value;
                    if (!string.IsNullOrEmpty(symbol))
                    {
                        symbols.Add(symbol);
                    }
                }
            }
            info("Unlocked Excel. Sending option qoute requests.");

            foreach (string symbol in symbols)
            {
                tasks.Add(TDAmeritradeClient.GetOptionChain(symbol));
            }
            info("Done sending option quote requests.");

            return tasks;

        }



        private void Cleanup()
        {
            TDAmeritradeClient.LogOut();

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            if (_mainWorksheet != null)
            {
                Marshal.ReleaseComObject(_mainWorksheet);
            }


            //close and release
            //  xlWorkbook.Save();
            if (_workbook != null)
            {
                Marshal.ReleaseComObject(_workbook);
            }


            //quit and release
            if (_xlApp != null)
            {
                _xlApp.Quit();
                Marshal.ReleaseComObject(_xlApp);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cleanup();
        }

        private object excelLock = new object();

        async void HandleOptionChain(Task<List<object>> task)
        {
            List<object> options = await task;

            if (options.Count > 0)
            {
                lock (excelLock)
                {
                    string symbol = (options[0] as OptionStrike)?.Call?.UnderlyingSymbol;
                    info("Locked Excel. Handling options quote symbol " + symbol);
                    try
                    {

                        Excel._Worksheet symbolSheet = (Excel._Worksheet)GetWorkBook().Sheets[symbol];
                        Excel.Range xlRange = symbolSheet.UsedRange;
                        xlRange.ClearContents();
                        object[,] data = new object[options.Count + 1, 18];

                        int row = 0;

                        data[row, 0] = "Symbol";
                        data[row, 1] = "Type";
                        data[row, 3] = "ExpirationDate";
                        data[row, 5] = "StrikePrice";
                        data[row, 9] = "Bid";
                        data[row, 10] = "Ask";                     
                        data[row, 12] = "Delta";
                        data[row, 13] = "Gamma";
                        data[row, 14] = "Theta";
                        data[row, 15] = "Vega";
                        data[row, 16] = "Rho";
                        data[row, 17] = "ImpliedVolatitily";

                        row++;

                        foreach (OptionStrike optionStrike in options)
                        {

                            Call call = optionStrike.Call;

                            if (call == null)
                            {
                                continue;
                            }

                            data[row, 0] = call.Symbol;
                            data[row, 1] = call.GetType().Name;
                            data[row, 3] = optionStrike.ExpirationDate.ToString("yyyy-MM-dd");
                            data[row, 5] = optionStrike.StrikePrice;
                            data[row, 9] = call.Bid;
                            data[row, 10] = call.Ask;
                            //  data[row, 11] = option.ExpirationType;
                            data[row, 12] = call.Delta;
                            data[row, 13] = call.Gamma;
                            data[row, 14] = call.Theta;
                            data[row, 15] = call.Vega;
                            data[row, 16] = call.Rho;
                            data[row, 17] = call.ImpliedVolatitily;                           
                            row++;
                        }
                        xlRange = GetExcel().Range[symbolSheet.Cells[1, 1], symbolSheet.Cells[data.GetLength(0), data.GetLength(1)]];
                        xlRange.Value = data;
                        (GetMainWorkSheet().Cells[25, "C"] as Excel.Range).Value = DateTime.Today;
                        (GetMainWorkSheet().Cells[GetSymbolRow(symbol), "C"] as Excel.Range).Font.Color = ColorTranslator.ToOle(Color.Black);
                    }
                    catch (Exception ex)
                    {
                        error("Unable to handle option chain", ex);
                    }

                }
                info("Unlocked Excel. Done handling options quote");

            }
        }

        private int GetSymbolRow(string symbol)
        {
            for (int row = 2; row < 9; row++)
            {
                var range = (GetMainWorkSheet().Cells[row, 3] as Excel.Range);
                string cell = (string)range.Value;
                if (!string.IsNullOrEmpty(cell) && cell.Equals(symbol))
                {
                    return row;
                }
            }
            return 0;
        }

        public async void HandleStockQuote(Task<List<object>> task)
        {
            List<object> quotes = await task;

            lock (excelLock)
            {
                info("Locked Excel. Handling stock quotes.");

                foreach (StockQuote quote in quotes)
                {
                    try
                    {
                        object[] data = new object[3];
                        data[0] = (quote.Bid + quote.Ask) / 2;
                        data[1] = (quote.Bid + quote.Ask) / 2 - quote.Close;
                        data[2] = quote.Close;
                        int row = GetSymbolRow(quote.Symbol);
                        Excel.Range xlRange = GetExcel().Range[GetMainWorkSheet().Cells[row, 14], GetMainWorkSheet().Cells[row, 16]];
                        xlRange.Value = data;
                        (GetMainWorkSheet().Cells[row, 14] as Excel.Range).Font.Color = ColorTranslator.ToOle(Color.Black);
                        (GetMainWorkSheet().Cells[row, 15] as Excel.Range).Font.Color = ColorTranslator.ToOle(Color.Black);
                        (GetMainWorkSheet().Cells[row, 16] as Excel.Range).Font.Color = ColorTranslator.ToOle(Color.Black);


                    }
                    catch (Exception ex)
                    {
                        error("Unable to handle stock quote for symbol " + quote.Symbol, ex);
                    }
                }
            }
            info("Unlocked Excel. Done handling stock quotes");
        }



        private void button3_Click(object sender, EventArgs e)
        {

            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                path = file.FileName;
                fileLabel.Text = path;
            }
        }

        private void fileLabel_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Refresh();
            }

        }
    }
}
