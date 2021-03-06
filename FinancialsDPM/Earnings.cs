using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FinancialsDPM
{
    namespace FinancialsDPM
{



    namespace FinancialsDPM
    {
        /// <summary>
        /// Constructor is mainly for Null Exception Safety.
        /// Class is for data encapsulation.
        /// </summary>
        public class Earnings
        {
            public string Ticker;
            public string FiscalDateEnding;
            public string ReportedEPS;

            public Earnings(string ticker, string fiscalDateEnding, string reportedEps)
            {
                Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
                FiscalDateEnding = fiscalDateEnding ?? throw new ArgumentNullException(nameof(fiscalDateEnding));
                ReportedEPS = reportedEps ?? throw new ArgumentNullException(nameof(reportedEps));
            }

            public Earnings(string ticker)
            {
                Ticker = ticker;
            }
        }

        public static class GetEarningsData
        {
            private static string api_key = "47BGPYJPFN4CEC20";
            /// <summary>
            /// Requests data for each ticker in StockList.SList
            /// </summary>
            /// <param name="ilogger"></param>
            /// <param name="stoppingToken"></param>
            /// <param name="toBreak"></param>
            /// <returns> List of API responses </returns>
            public static async Task<List<string>> EarningsRequest(ILogger ilogger, CancellationToken stoppingToken, bool toBreak=false)
            {
                List<string> Earnings = new();
                using HttpClient httpClient = new();
                string responseBody = string.Empty;
                foreach (string stock in StockList.SList)
                {
                    try
                    {
                        responseBody = await httpClient.GetStringAsync(
                            $"https://www.alphavantage.co/query?function=EARNINGS&symbol={stock}&apikey={api_key}",
                            stoppingToken);
                        ilogger.LogInformation("Successfully received info at: {Time}", DateTimeOffset.UtcNow);
                        await Task.Delay(12000, stoppingToken);

                    }
                    catch (Exception e)
                    {
                        ilogger.LogInformation("Exception thrown : {Exception}", e.ToString());
                        await Task.Delay(12000, stoppingToken);

                    }

                    Earnings.Add(responseBody);

                    if (toBreak)
                    {
                        break;
                    }
                }

                return Earnings;
            }
            /// <summary>
            /// Processes the above gotten data.
            /// </summary>
            /// <param name="dList"></param>
            /// <returns>List of  Classes for each stock in StockList.SList</returns>
            /// <exception cref="InvalidOperationException"></exception>
            public static List<Earnings> ProcessEarningsData(List<string> dList)
            {
                List<Earnings> parsedData = new();
                foreach (string response in dList)
                {
                    var jsonData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(response) ??
                                   throw new InvalidOperationException();
                    var AnnualEarnings = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonData["annualEarnings"]);

                    string tempTicker = jsonData["symbol"].GetString();
                    string tempFiscalDateEnding = AnnualEarnings[0]["fiscalDateEnding"];
                    string tempReportedEPS = AnnualEarnings[0]["reportedEPS"];

                    
                    Earnings currentStock = new(tempTicker,
                                                    tempFiscalDateEnding,
                                                    tempReportedEPS);
                    parsedData.Add(currentStock);
                }

                return parsedData;
            }
        }
    }
}
}