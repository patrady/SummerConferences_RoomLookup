namespace SC_RoomLookup
{
    public class Header_Item
    {
        public int HeaderNum { get; set; }
        public HeaderValues Title { get; set; }

        public Header_Item(int _HeaderNum, HeaderValues _Title)
        {
            this.HeaderNum = _HeaderNum;
            this.Title = _Title;
        }
    }
}
