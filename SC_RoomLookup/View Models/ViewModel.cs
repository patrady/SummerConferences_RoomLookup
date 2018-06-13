using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace SC_RoomLookup
{
    public class ViewModel : INotifyPropertyChanged
    {
        public Model Model { get; set; }
        public DriveAPI GoogleSheets { get; set; }
        private string FileName { get; set; }

        public ViewModel()
        {
            Model = new Model();
            this.GoogleSheets = new DriveAPI();
            this.IndexedData = Model.Read();
            this.Results = new ObservableCollection<Room>();
            this.ChangeHall(Hall.A_Hall);
            this.FileName = "log_" + DateTime.Now.ToString("MM-dd-yyyy hh_mm_ss") + ".txt";

            try {
                this.GoogleSheets.SetSpreadSheet("1qM2QNKumoae0UbksARlBVjSmVHdbTEWG5VU1C5oB00o");
            } catch (Exception e)
            {
                MessageBox.Show("Please ensure that you are connected to the internet\nRestart the app after you are connected!" , "Google Sheets Error");
                Application.Current.Shutdown();
            }
        }

        // Public Attributes
        public Hall Selected_Hall
        {
            get { return selectedHall; }
            set
            {
                if (value != selectedHall)
                {
                    selectedHall = value;
                    OnPropertyChanged("Selected_Hall");
                }
            }
        }
        public SearchBy_Settings Selected_Setting
        {
            get { return selectedsetting; }
            set
            {
                if (value != selectedsetting)
                {
                    selectedsetting = value;
                    OnPropertyChanged("Selected_Setting");
                }
            }
        }
        public FilterBy_Settings Filtered_Setting
        {
            get { return filtersetting; }
            set
            {
                if (value != filtersetting)
                {
                    filtersetting = value;
                    OnPropertyChanged("Filtered_Setting");
                }
            }
        }
        public Check_Options Check_Setting
        {
            get { return checkOptions; }
            set
            {
                if (value != checkOptions)
                {
                    checkOptions = value;
                    OnPropertyChanged("Check_Setting");
                    IsUpdatingGoogleSheets();
                }
            }
        }
        public ObservableCollection<Room> Results { get; set; }
        public string Search_Input
        {
            get { return searchInput; }
            set
            {
                if (value != searchInput)
                {
                    searchInput = value;
                    OnPropertyChanged("Search_Input");
                }
            }
        }
        public Dictionary<string,List<Room>> IndexedData { get; set; }
        public List<Room> HallData { get; set; }
        public string SpreadSheetURL
        {
            get { return spreadsheetURL; }
            set
            {
                if (value != spreadsheetURL)
                {
                    //https://docs.google.com/spreadsheets/d/{SpreadSheetID}/edit#gid={SheetID}
                    // Check if the URL is valid
                    if (this.ValidateURL(value))
                    {
                        spreadsheetURL = value;

                        // Update the connection to the Google Spreadsheet
                        this.GoogleSheets.SetSpreadSheet(spreadsheetURL.Split('/')[5]);
                        this.GoogleSheets.SetSheet(spreadsheetURL.Split('/').Last().Replace("edit#gid=", ""));
                        this.GoogleSheets.ReadSheet();

                        // Display to the user if they are connected to the Google Sheet and it will be updated
                        this.IsUpdatingGoogleSheets();
                    } 
                    else 
                    {
                        MessageBox.Show("The URL is not valid! Make sure that it follows the format: \n" +
                            "https://docs.google.com/spreadsheets/d/{SpreadSheetID}/edit#gid={SheetID} \n" +
                            "where {SpreadSheetID} is combination of letters\n" +
                            "and {SheetID} is a number");
                    }
                }
                else
                    value = null;

                OnPropertyChanged("SpreadSheetURL");
            }
        }
        public bool WillUpdateGoogleSheets
        {
            get { return willUpdateGoogleSheets; }
            set
            {
                if (value != willUpdateGoogleSheets)
                {
                    willUpdateGoogleSheets = value;
                    OnPropertyChanged("WillUpdateGoogleSheets");
                }
            }
        }


        // Public Commands
        private RelayCommand _uploadFile;
        public ICommand UploadFileCommand { get => _uploadFile ?? (_uploadFile = new RelayCommand(param => this.UploadFile())); }

        private RelayCommand<Hall> _changeHall;
        public ICommand ChangeHallCommand { get => _changeHall ?? (_changeHall = new RelayCommand<Hall>(i => this.ChangeHall(i))); }

        private RelayCommand _search;
        public ICommand SearchCommand { get => _search ?? (_search = new RelayCommand(i => this.Search())); }

        // Methods
        private void Search()
        {
            // Search by the selected setting
            string input = this.Search_Input;
            switch (Selected_Setting)
            {
                case SearchBy_Settings.KeyCode:
                    SearchByKeyCode(input);
                    break;

                case SearchBy_Settings.PIK:
                    int output;
                    if (!int.TryParse(input, out output))
                    {
                        MessageBox.Show("Please enter the last four digits of the PIK Number.", "Invalid PIK", MessageBoxButton.OK);
                        return;
                    }
                    SearchByPIK(input);
                    break;

                case SearchBy_Settings.RoomNumber:
                    SearchByRoomNum(input);
                    break;
            }
            Filter();
            PrintToFile(input);

            // If the Search only yeilds one result, then get the full PIK of that input and update the google sheet
            if (this.Results.Count.Equals(1))
                UpdateGoogleSheets(this.GetFullPIK(input));
        }
        private void SearchByPIK(string PIK)
        {
            // Check if the PIK exists and add it to the results
            try
            {
                this.Results.Clear();
                foreach (Room r in this.HallData.FindAll(r => r.PIK.Contains(PIK)))
                    this.Results.Add(r);
            }
            catch { }

        }
        private void SearchByRoomNum(string RoomNum)
        {
            // Check if the room number exists and add it to the results (case insensitive)
            try
            {
                this.Results.Clear();
                RoomNum = RoomNum.ToLower();
                foreach (Room r in this.HallData.FindAll(r => r.RoomNumber.ToLower().Contains(RoomNum)))
                    this.Results.Add(r);
            }
            catch { }

        }
        private void SearchByKeyCode(string keyCode)
        {
            // Check if the key code exists and add it to the results (case insensitive)
            try
            {
                this.Results.Clear();
                keyCode = keyCode.ToLower();
                foreach (Room r in this.HallData.FindAll(r => r.KeyCode.ToLower().Contains(keyCode)))
                    this.Results.Add(r);
            } catch { }

        }
        private void ChangeHall(Hall newHall)
        {
            // Update the new hall and load the data for that hall
            this.Selected_Hall = newHall;
            this.HallData = this.IndexedData[Selected_Hall.ToString()];
            this.Results.Clear();
        }
        private void Filter()
        {
            // Do not filter if no filtered settings are selected
            if (this.Filtered_Setting == FilterBy_Settings.All)
                return;

            // Make a duplicate of the results
            List<Room> temp = new List<Room>(this.Results);

            // Filter the rooms
            if (this.Filtered_Setting == FilterBy_Settings.Conference)
                temp.RemoveAll(r => r.RoomDescription.Contains("SPARE"));
            else if (this.Filtered_Setting == FilterBy_Settings.Spare)
                temp = new List<Room>(temp.FindAll(r => r.RoomDescription.Contains("SPARE")));

            // Add all of the rooms to the data
            this.Results.Clear();
            foreach (Room r in temp)
                this.Results.Add(r);
        }
        private void UpdateGoogleSheets(string PIK)
        {
            try
            {
                // Highlight the cell in the Google sheets based off it is check-in or check-out
                switch (this.Check_Setting)
                {
                    case Check_Options.Check_In:
                        this.GoogleSheets.HighlightCell(PIK, true);
                        break;
                    case Check_Options.Check_Out:
                        this.GoogleSheets.HighlightCell(PIK, false);
                        break;
                }
            } catch (Exception e)
            {
                // Check if the internet is down since an error was thrown
                if (!IsConnectedToInternet())
                    MessageBox.Show("The Google Sheet cannot be updated until connected to the internet", "Connection Issue", MessageBoxButton.OK);
                else
                    MessageBox.Show("Unable to update the Google Sheet!\nError: " + e, "Google Sheets Error", MessageBoxButton.OK);
            }
            
        }
        private void PrintToFile(string search)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Logs");

            // Create the Directory if it does not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Write the search to the log
            using (StreamWriter file = new StreamWriter(Path.Combine(path, this.FileName), true))
            {
                file.WriteLine(String.Format("Hall: {0}, Search: {1}, Type: {2}, Filter: {3}, GoogleSheets: {4}", this.Selected_Hall.ToString(), 
                                                                                                                 search, 
                                                                                                                 this.Selected_Setting.ToString(), 
                                                                                                                 this.Filtered_Setting.ToString(), 
                                                                                                                 this.Check_Setting.ToString()));
            }
        }
        private bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool ValidateURL(string url)
        {
            // Sample URL: https://docs.google.com/spreadsheets/d/1qM2QNKumoae0UbksARlBVjSmVHdbTEWG5VU1C5oB00/edit#gid=47395400
            long tempInt;
            try
            {
                if (!url.Contains("docs.google.com/spreadsheets/d/"))
                    return false;
                if (url.Split('/').Count() != 7)
                    return false;
                if (!long.TryParse(url.Split('/').Last().Replace("edit#gid=", ""), out tempInt))
                    return false;
            } catch { return false; }

            return true;
            
        }
        private string GetFullPIK(string input)
        {
            // Returns the first PIK match for a partial PIK
            input = input.ToLower();
            switch (this.Selected_Setting)
            {
                case SearchBy_Settings.PIK:
                    foreach (Room r in this.HallData.FindAll(r => r.PIK.Contains(input)))
                        return r.PIK;
                    break;
                case SearchBy_Settings.KeyCode:
                    foreach (Room r in this.HallData.FindAll(r => r.KeyCode.ToLower().Contains(input)))
                        return r.PIK;
                    break;
                case SearchBy_Settings.RoomNumber:
                    foreach (Room r in this.HallData.FindAll(r => r.RoomNumber.ToLower().Contains(input)))
                        return r.PIK;
                    break;
            }
            return null;
        }
        private void IsUpdatingGoogleSheets()
        {
            this.WillUpdateGoogleSheets = !string.IsNullOrEmpty(this.SpreadSheetURL) && this.Check_Setting != Check_Options.None;
        }

        // Private variables
        private string searchInput, spreadsheetURL;
        private Hall selectedHall;
        private SearchBy_Settings selectedsetting;
        private FilterBy_Settings filtersetting;
        private Check_Options checkOptions;
        public bool willUpdateGoogleSheets;

        //INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
