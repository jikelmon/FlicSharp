using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtNewVerifiedButton
    {
        public BluetoothAddress FLIC_BT_ADDRESS { get; set; }

        public EvtNewVerifiedButton(byte[] data_buffer)
        {
            //Bluetooth Address
            byte[] mac_address_buffer = new byte[6];
            for (int i = 5; i >= 0; i--)
            {
                mac_address_buffer[i] = data_buffer[i + 1];
            }
            FLIC_BT_ADDRESS = new BluetoothAddress(mac_address_buffer);
        }

        public override string ToString()
        {
            return "New Verified Button: " + FLIC_BT_ADDRESS.ToString();
        }
    }
}
