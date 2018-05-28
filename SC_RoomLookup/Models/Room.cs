using System;

namespace SC_RoomLookup
{
    public class Room
    {
        // PIK is an integer that is stored on the magnetic strip
        public string PIK { get; set; }

        // Room Number is the Room with the Suite Number (if applicable)
        public string RoomNumber { get; set; }

        // Key Code is used for Libscomb Hall
        public string KeyCode { get; set; }

        // Describes the status of the room - "To be used" or "Spare"
        public string RoomDescription { get; set; }

        public Room()
        {
            this.PIK = null;
            this.RoomNumber = null;
            this.RoomDescription = null;
            this.KeyCode = null;
        }

        public bool IsEmpty()
        {
            return PIK.Equals(-1) && string.IsNullOrEmpty(RoomNumber) && string.IsNullOrEmpty(KeyCode) && string.IsNullOrEmpty(RoomDescription);
        }
    }
}
