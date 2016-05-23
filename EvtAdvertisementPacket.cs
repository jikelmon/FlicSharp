using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtAdvertisementPacket
    {
        public UInt32 SCAN_ID { get; set; }
        public BluetoothAddress FLIC_BT_ADDRESS { get; set; }
        public byte FLIC_NAME_LENGTH { get; set; }
        public string FLIC_NAME { get; set; }
        public sbyte RSSI { get; set; }
        public bool IS_PRIVATE { get; set; }
        public bool ALREADY_VERIFIED { get; set; }

        public EvtAdvertisementPacket(byte[] data_buffer)
        {
            //Scanner Identifier
            byte[] scanner_identifier = new byte[] { data_buffer[1], data_buffer[2], data_buffer[3], data_buffer[4] };
            Array.Reverse(scanner_identifier);
            SCAN_ID = BitConverter.ToUInt32(scanner_identifier, 0);

            //Flic Bluetooth Address            
            string mac_address = "";
            for (int i = 10; i > 4; i--)
            {
                mac_address += String.Format("{0:X2}", data_buffer[i]);
                if (i != 5)
                {
                    mac_address += ":";
                }
            }
            FLIC_BT_ADDRESS = BluetoothAddress.Parse(mac_address);

            //Flic name length
            FLIC_NAME_LENGTH = data_buffer[11];

            //Flic Name
            FLIC_NAME = Encoding.UTF8.GetString(data_buffer, 12, FLIC_NAME_LENGTH);

            //RSSI
            RSSI = (sbyte)(data_buffer[28]);

            //Is private
            IS_PRIVATE = BitConverter.ToBoolean(data_buffer, 29);

            //already verified
            ALREADY_VERIFIED = BitConverter.ToBoolean(data_buffer, 30);
        }

        public override string ToString()
        {
            string info = "";

            //Scanner identifier
            info += "Scanner Identifier: ";
            info += SCAN_ID;
            info += "\n";

            //Scanner identifier
            info += "Flic BT Address ";
            info += FLIC_BT_ADDRESS.ToString();
            info += "\n";

            //Name
            info += "Flic Name: ";
            info += FLIC_NAME;
            info += "\n";

            //RSSI
            info += "RSSI: ";
            info += RSSI;
            info += "\n";

            //Is private
            info += "Is Private: ";
            info += IS_PRIVATE;
            info += "\n";

            //Is private
            info += "Already Verified: ";
            info += ALREADY_VERIFIED;
            info += "\n";

            return info;
        }
    }
}
