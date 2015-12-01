namespace Keiser.M3i.BLE_Parser
{
    using System;

    public static class Parser
    {
        // Summary:
        //     Parses raw broadcast data into a Broadcast object
        //
        // Parameters:
        //   address:
        //     The address string returned by the Bluetooth LE scanning method.
        //
        //   advertisingData:
        //     The raw advertising data byte array returned by the Bluetooth LE scanning method. 
        //
        //   rssi:
        //     The raw RSSI value returned by the Bluetooth LE scanning method. 
        //
        // Returns:
        //     A Broadcast object with all properties assigned values based on 
        //     the received advertising data.
        //
        public static Broadcast Parse(string address, byte[] advertisingData, int rssi)
        {
            Broadcast broadcast = new Broadcast();
            // Sets broadcast to invalid until parsing is complete
            broadcast.IsValid = false;

            broadcast.UUID = address;
            broadcast.RSSI = rssi;

            // Checks that broadcast is not debug signal
            if (advertisingData.Length < 4 || advertisingData.Length > 19)
                return broadcast;

            // Sets parser index
            int index = 0;

            // Moves index past prefix bits (some platforms remove prefix bits from data)
            if (advertisingData[index] == 2 && advertisingData[index + 1] == 1)
                index += 2;

            // Assigns build values
            // ** Note: Build values are not converted to hex prior to broadcast
            //          so they arrive in a mutated form.
            broadcast.BuildMajor = BuildValueConvert(advertisingData[index++]);
            broadcast.BuildMinor = BuildValueConvert(advertisingData[index++]);

            // Determins which method to use for parsing based on build major
            // ** Note: Build major 6 currently the only build major.
            if (broadcast.BuildMajor == 6 && advertisingData.Length > (index + 13))
            {
                // Raw Interval Value
                broadcast.Interval = advertisingData[index];
                // Ordginal ID
                broadcast.ID = advertisingData[index + 1];
                // Cadence in RPM (broadcast with decimal precision)
                broadcast.Cadence = TwoByteConcat(advertisingData[index + 2], advertisingData[index + 3]) / 10;
                // Heart Rate in BPM (broadcast with decimal precision)
                broadcast.HeartRate = TwoByteConcat(advertisingData[index + 4], advertisingData[index + 5]) / 10;
                // Power in Watts
                broadcast.Power = TwoByteConcat(advertisingData[index + 6], advertisingData[index + 7]);
                // Energy as KCal ("energy burned")
                broadcast.Energy = TwoByteConcat(advertisingData[index + 8], advertisingData[index + 9]);
                // Time in Seconds (broadcast as minutes and seconds)
                broadcast.Time = advertisingData[index + 10] * 60;
                broadcast.Time += advertisingData[index + 11];
                // Trip in Miles (broadcast in miles or Km)
                broadcast.Trip = TwoByteConcat(advertisingData[index + 12], advertisingData[index + 13]);
                if ((broadcast.Trip & 32768) != 0)
                    broadcast.Trip = (int)(broadcast.Trip * 1.60934);
                // Check for additional parameters added to later builds (v.21+)
                if (broadcast.BuildMinor >= 21 && advertisingData.Length > (index + 14))
                {
                    // Raw Gear Value
                    broadcast.Gear = advertisingData[index + 14];
                }

                // Sets broadcast to valid 
                broadcast.IsValid = true;
            }

            return broadcast;
        }

        public static int TwoByteConcat(byte lower, byte higher)
        {
            return (higher << 8) | lower;
        }

        public static int BuildValueConvert(byte value)
        {
            int converted;
            Int32.TryParse(value.ToString("X"), out converted);
            return converted;
        }
    }
}
