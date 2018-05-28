using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace SC_RoomLookup
{
    public class Model
    {
        public Model() { }

        public Header Header;

        public Dictionary<string, List<Room>> Read()
        {
            Dictionary<string, string> Files = new Dictionary<string, string>()
            {
                {Hall.A_Hall.ToString(), "CSV/AH.csv"},
                {Hall.KC_Hall.ToString(), "CSV/KC.csv"},
                {Hall.Libscomb_Hall.ToString(), "CSV/LH.csv"},
                {Hall.Vandergriff_Hall.ToString(), "CSV/VH.csv"}
            };

            Dictionary<string, List<Room>> Data = new Dictionary<string, List<Room>>();
            for (int i = 0; i < Files.Count; i++)
                Data.Add(Files.ElementAt(i).Key, this.ReadFile(Files.ElementAt(i).Value));

            return Data;
        }

        private Header ReadHeaders(string fileName)
        {
            // Open and Read the File
            string text = File.ReadAllText(fileName);

            // Find the number of headers
            string[] headers = text.Split(Environment.NewLine.ToCharArray())[0].Split(',');

            // Create the Header Object
            return new Header(headers);
        }

        private List<Room> ReadFile(string fileName)
        {
            List<Room> returnList = new List<Room>();
            string[] columns;

            this.Header = this.ReadHeaders(fileName);

            // Read the file
            string[] lines = File.ReadAllLines(fileName);

            for (int i = 1; i < lines.Length; i++)
            {
                // Split the line into its columns
                columns = lines[i].Split(',');

                Room temp = new Room();

                for (int j = 0; j < columns.Length; j++)
                {
                    // Convert the data based on what it is
                    switch (Header.Headers[j].Title)
                    {
                        case HeaderValues.PIK:
                            temp.PIK = columns[j];
                            break;
                        case HeaderValues.Room:
                            temp.RoomNumber = columns[j];
                            break;
                        case HeaderValues.RoomDesc:
                            temp.RoomDescription = columns[j];
                            break;
                        case HeaderValues.KeyCode:
                            temp.KeyCode = columns[j];
                            break;
                        default:
                            break;
                    }
                }

                if (!temp.IsEmpty())
                    returnList.Add(temp);
            }

            // Sort the File by PIK
            returnList = this.SortPIK(returnList);

            return returnList;
        }

        private List<Room> SortPIK(List<Room> input)
        {
            return input.OrderBy(o => o.PIK).ToList();
        }
    }
}
