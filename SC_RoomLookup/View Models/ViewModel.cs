using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SC_RoomLookup
{
    public class ViewModel : INotifyPropertyChanged
    {
        public Model Model { get; set; }

        public ViewModel()
        {
            Model = new Model();
            this.IndexedData = Model.Read();
            this.Results = new ObservableCollection<Room>();
            this.ChangeHall(Hall.A_Hall);

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
        }
        private void SearchByPIK(string PIK)
        {
            this.Results.Clear();
            foreach (Room r in this.HallData.FindAll(r => r.PIK.Contains(PIK)))
                this.Results.Add(r);
        }
        private void SearchByRoomNum(string RoomNum)
        {
            this.Results.Clear();
            foreach (Room r in this.HallData.FindAll(r => r.RoomNumber.Contains(RoomNum)))
                this.Results.Add(r);
        }
        private void SearchByKeyCode(string keyCode)
        {
            this.Results.Clear();
            foreach (Room r in this.HallData.FindAll(r => r.KeyCode.Contains(keyCode)))
                this.Results.Add(r);
        }
        private void UploadFile()
        {
            OpenFileDialog fileBrowser = new OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".csv",
                Filter = "Comma Delimited (*.csv)|"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = fileBrowser.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = fileBrowser.FileName;
                Console.WriteLine(filename);
            }
        }
        private void ChangeHall(Hall newHall)
        {
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

        // Private variables
        private string searchInput;
        private Hall selectedHall;
        private SearchBy_Settings selectedsetting;
        private FilterBy_Settings filtersetting;

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

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SearchBy_Settings
    {
        [Description("PIK")]
        PIK,
        [Description("Room Number")]
        RoomNumber,
        [Description("Key Code")]
        KeyCode
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FilterBy_Settings
    {
        [Description("Conference Room")]
        Conference,
        [Description("Spare Room")]
        Spare,
        [Description("All")]
        All
    }

    public enum Hall { A_Hall, KC_Hall, Libscomb_Hall, Vandergriff_Hall }

}
