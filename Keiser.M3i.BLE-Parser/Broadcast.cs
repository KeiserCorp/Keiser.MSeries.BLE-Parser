namespace Keiser.M3i.BLE_Parser
{
    public class Broadcast
    {
        public bool IsValid { get; set; }   // Indicates that broadcast was parsed successfully
        public string UUID { get; set; }    // Universally Unique Hex String
        public int ID { get; set; }         // Domain Unique Ordinal Number (Decimal)
        public int Cadence { get; set; }    // In RPM
        public int HeartRate { get; set; }  // In BPM
        public int Power { get; set; }      // In Watts
        public int BuildMajor { get; set; }
        public int BuildMinor { get; set; }
        public int Interval { get; set; }   // Raw Interval Value
        public bool IsRealTime              // Determines if current values are real time or averages
        {
            get
            {
                return Interval == 0 || (Interval > 128 && Interval < 255);
            }
        }
        public int IntervalValue            // Converted Interval Value (Matches Displayed Interval)
        {
            get
            {
                if (Interval == 0 || Interval == 255)
                    return 0;
                if (Interval > 128 && Interval < 255)
                    return Interval - 128;
                return Interval;
            }
        }

        public int Energy { get; set; }     // In KCal
        public int Trip { get; set; }       // In Miles
        public int Time { get; set; }       // In Seconds
        public int RSSI { get; set; }       // In dBm
        public int Gear { get; set; }       
    }
}
