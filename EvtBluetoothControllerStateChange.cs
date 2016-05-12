using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtBluetoothControllerStateChange
    {
        public BluetoothControllerState BLUETOOTH_CONTROLLER_STATE { get; set; }

        public EvtBluetoothControllerStateChange(byte[] data_buffer)
        {
            switch (data_buffer[1])
            {
                case 0: BLUETOOTH_CONTROLLER_STATE = BluetoothControllerState.Detached; break;
                case 1: BLUETOOTH_CONTROLLER_STATE = BluetoothControllerState.Resetting; break;
                case 2: BLUETOOTH_CONTROLLER_STATE = BluetoothControllerState.Attached; break;
                default: throw new Exception("Unknown BluetoothControllerState value");
            }
        }

        public override string ToString()
        {
            return "Bluetooth Controller State: " + BLUETOOTH_CONTROLLER_STATE;
        }
    }
}
