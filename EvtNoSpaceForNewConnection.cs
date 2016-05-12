using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtNoSpaceForNewConnection
    {
        public byte MAX_CONCURRENTLY_CONNECTED_BUTTONS { get; set; }

        public EvtNoSpaceForNewConnection(byte[] data_buffer)
        {
            MAX_CONCURRENTLY_CONNECTED_BUTTONS = data_buffer[1];
        }

        public override string ToString()
        {
            return "Max Concurrently Connected Buttons: " + MAX_CONCURRENTLY_CONNECTED_BUTTONS;
        }
    }
}
