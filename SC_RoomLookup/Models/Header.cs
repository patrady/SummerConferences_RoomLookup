using System.Collections.Generic;

namespace SC_RoomLookup
{
    public class Header
    {
        public List<Header_Item> Headers { get; set; }
        public int NumColumns { get; set; }

        public Header(string[] _ColumnTitles)
        {
            // Initialize the class properties
            this.NumColumns = _ColumnTitles.Length;
            this.Headers = new List<Header_Item>();

            // Add the header titles to this main header and detect what type they are
            int i = 0;
            foreach (string columnTitle in _ColumnTitles)
                this.Headers.Add(new Header_Item(i++, this.DetectHeaderTypes(columnTitle)));
        }

        private void SetHeader(int index, HeaderValues title)
        {
            // Set a headers title if the index is valid
            if (index < this.Headers.Count)
            {
                this.Headers[index].Title = title;
            }
        }

        private HeaderValues DetectHeaderTypes(string HeaderTitle)
        {
            // Detect what type the column title is 
            if (HeaderTitle.ToLower().Contains("pik"))
                return HeaderValues.PIK;
            else if (HeaderTitle.ToLower().Contains("hall"))
                return HeaderValues.RoomDesc;
            else if (HeaderTitle.ToLower().Contains("room"))
                return HeaderValues.Room;
            else if (HeaderTitle.ToLower().Contains("key"))
                return HeaderValues.KeyCode;

            return HeaderValues.Blank;
        }
    }
}
