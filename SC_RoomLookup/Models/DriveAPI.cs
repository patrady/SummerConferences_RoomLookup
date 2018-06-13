using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace SC_RoomLookup
{
    public class DriveAPI
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static string ApplicationName = "SCRoomLookup";
        private SheetsService Service { get; set; }
        private IList<IList<Object>> Values { get; set; }
        private string SpreadSheetID { get; set; } // Workbook ID
        private int SheetID { get; set; } // Sheet ID
        private Dictionary<string, int> PIK_Row { get; set; }
        private const int START_ROW = 5; // base zero
        private const int CHECK_IN_COL = 7; // base zero
        private const int CHECK_OUT_COL = 8; // base zero
        private static readonly string TABLE_NAME = "Roster";
        private static readonly string TABLE_RANGE = "F6:G";

        public DriveAPI()
        {
            this.PIK_Row = new Dictionary<string, int>();
            this.Login();
        }

        public void Login()
        {
            UserCredential credential;

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public void SetSpreadSheet(string sheet)
        {
            this.SpreadSheetID = sheet;
        }

        public void SetSheet(string sheet)
        {
            this.SheetID = int.Parse(sheet);
        }

        public void ReadSheet()
        {
            // Define request parameters.
            SpreadsheetsResource.ValuesResource.GetRequest request = Service.Spreadsheets.Values.Get(SpreadSheetID, TABLE_NAME + "!" + TABLE_RANGE);

            // Prints the names and majors of students in a sample spreadsheet:
            ValueRange response = request.Execute();
            Values = response.Values;
            int startRow = START_ROW;
            this.PIK_Row = new Dictionary<string, int>();
            if (Values != null && Values.Count > 0)
            {
                foreach (var row in Values)
                {
                    this.PIK_Row.Add(row[0] as string, startRow++);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }

        public void HighlightCell(string PIK, bool isCheckIn)
        {
            // Check that the PIK exists before attempting to get it
            if (this.PIK_Row.ContainsKey(PIK))
            {
                // Get the row of the PIK from the data structure and select which column should be used
                int row = this.PIK_Row[PIK];
                int col = isCheckIn ? CHECK_IN_COL : CHECK_OUT_COL;

                this.HighlightCellAsync(row, col);
            }
        }

        private async void HighlightCellAsync(int row, int col)
        {            
            Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetRequest requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetRequest();

            // Send a request that get the range, set the background color to highlight yellow, and set the date as today
            requestBody.Requests = new List<Google.Apis.Sheets.v4.Data.Request>()
            {
                new Request()
                {
                    RepeatCell = new RepeatCellRequest()
                    {
                        Range = new GridRange()
                        {
                            SheetId = this.SheetID,
                            StartRowIndex = row,
                            EndRowIndex = row + 1,
                            StartColumnIndex = col,
                            EndColumnIndex = col + 1
                        },
                        Cell = new CellData()
                        {
                            UserEnteredFormat = new CellFormat()
                            {
                                BackgroundColor = new Color()
                                {
                                    Red = 1,
                                    Green = 1,
                                    Blue = 0
                                },
                            },
                            UserEnteredValue = new ExtendedValue()
                            {
                                StringValue = DateTime.Today.ToString("MM/dd/yyyy")
                            }
                        },
                        Fields = "userEnteredValue,userEnteredFormat.backgroundColor"
                    },
                }
            };

            // Send the request
            SpreadsheetsResource.BatchUpdateRequest request = Service.Spreadsheets.BatchUpdate(requestBody, this.SpreadSheetID);
            Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetResponse response = await request.ExecuteAsync();
            Console.WriteLine(JsonConvert.SerializeObject(response));
        }
    }
}
