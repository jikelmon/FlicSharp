using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtGetInfoResponse
    {
        public BluetoothControllerState BLUETOOTH_CONTROLLER_STATE { get; set; }
        public BluetoothAddress HCI_CONTROLLER_BLUETOOTH_ADDRESS { get; set; }
        public BdAddrType BD_ADDR_TYPE { get; set; }
        public byte MAX_PENDING_CONNECTIONS { get; set; }
        public Int16 MAX_CONCURRENTLY_CONNECTED_BUTTONS { get; set; }
        public byte CURRENT_PENDING_CONNECTIONS { get; set; }
        public bool CURRENTLY_NO_SPACE_FOR_NEW_CONNECTIONS { get; set; }
        public UInt16 NUMBER_OF_VERIFIED_BUTTONS { get; set; }
        public List<BluetoothAddress> VERIFIED_FLIC_BUTTONS { get; set; }

        public EvtGetInfoResponse(byte[] data_buffer)
        {
            //Bluetooth Controller State           
            switch (data_buffer[1])
            {
                case 0: BLUETOOTH_CONTROLLER_STATE = BluetoothControllerState.Detached; break;
                case 1: BLUETOOTH_CONTROLLER_STATE = BluetoothControllerState.Resetting; break;
                case 2: BLUETOOTH_CONTROLLER_STATE = BluetoothControllerState.Attached; break;
                default: throw new Exception("Unknown BluetoothControllerState value");
            }

            //HCI Bluetooth Device Address            
            string mac_address = "";
            for (int i = 7; i > 1; i--)
            {
                mac_address += String.Format("{0:X}", data_buffer[i]);
                if (i != 2)
                {
                    mac_address += ":";
                }
            }
            HCI_CONTROLLER_BLUETOOTH_ADDRESS = BluetoothAddress.Parse(mac_address);

            //BD Address Type            
            switch (data_buffer[8])
            {
                case 0: BD_ADDR_TYPE = BdAddrType.PublicBdAddrType; break;
                case 1: BD_ADDR_TYPE = BdAddrType.RandomBdAddrType; break;
                default: throw new Exception("Unknown BdAddrType value");
            }

            //Max pending connections            
            MAX_PENDING_CONNECTIONS = data_buffer[9];

            //Max Concurrently Connected Buttons            
            MAX_CONCURRENTLY_CONNECTED_BUTTONS = BitConverter.ToInt16(data_buffer, 10);

            //Current Pending Connections            
            CURRENT_PENDING_CONNECTIONS = data_buffer[12];

            //Currently No Space For New Connection            
            CURRENTLY_NO_SPACE_FOR_NEW_CONNECTIONS = BitConverter.ToBoolean(data_buffer, 13);

            //Number Of Verified Buttons            
            NUMBER_OF_VERIFIED_BUTTONS = BitConverter.ToUInt16(data_buffer, 14);

            //Verified Buttons
            if (NUMBER_OF_VERIFIED_BUTTONS == 0)
            {
                VERIFIED_FLIC_BUTTONS = null;
            }
            else
            {
                VERIFIED_FLIC_BUTTONS = new List<BluetoothAddress>();
                for (int i = 0; i < NUMBER_OF_VERIFIED_BUTTONS; i++)
                {
                    mac_address = "";
                    byte[] mac_address_buffer = new byte[6];
                    int k = 0;
                    for (int j = i * 6 + 21; j > i * 6 + 15; j--)
                    {
                        mac_address_buffer[k++] = data_buffer[j];                        
                    }
                    VERIFIED_FLIC_BUTTONS.Add(new BluetoothAddress(mac_address_buffer));
                }
            }            
        }

        public override string ToString()
        {
            string info = "";

            //Bluetooth Controller State
            info = "Bluetooth Controller State: ";
            info += BLUETOOTH_CONTROLLER_STATE;
            info += "\n";

            //HCI Bluetooth Device Address
            info += "MAC Address: ";
            info += HCI_CONTROLLER_BLUETOOTH_ADDRESS;
            info += "\n";

            //BD Address Type
            info += "Bluetooth Address Type: ";
            info += BD_ADDR_TYPE;
            info += "\n";

            //Max pending connections
            info += "Max Pending Connections: ";
            info += MAX_PENDING_CONNECTIONS;
            info += "\n";

            //Max Concurrently Connected Buttons
            info += "Max Concurrently Connected Buttons: ";
            info += MAX_CONCURRENTLY_CONNECTED_BUTTONS;
            info += "\n";

            //Current Pending Connections
            info += "Current Pending Connections: ";
            info += CURRENT_PENDING_CONNECTIONS;
            info += "\n";

            //Currently No Space For New Connection
            info += "Currently No Space For New Connection: ";
            info += CURRENTLY_NO_SPACE_FOR_NEW_CONNECTIONS;
            info += "\n";

            //Number Of Verified Buttons
            info += "Number Of Verified Buttons: ";
            info += NUMBER_OF_VERIFIED_BUTTONS;
            info += "\n";

            //Verified Buttons
            info += "Verified Buttons:\n";
            if (NUMBER_OF_VERIFIED_BUTTONS == 0)
            {
                info += "\tNo Verfified Buttons";
            }
            else
            {
                foreach (BluetoothAddress ba in VERIFIED_FLIC_BUTTONS)
                {
                    info += "\t" + ba.ToString() + "\n";
                }
            }

            return info;            
        }
    }
}
