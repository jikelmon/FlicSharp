using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtButtonSingleOrDoubleClickOrHold
    {
        public uint CHANNEL_IDENTIFIER { get; set; }
        public ClickType CLICK_TYPE { get; set; }
        public bool QUEUED { get; set; }
        public int TIME_DIFFERENCE { get; set; }

        public EvtButtonSingleOrDoubleClickOrHold(byte[] data_buffer)
        {
            //Connection Channel Identifier
            byte[] channel_identifier = new byte[] { data_buffer[1], data_buffer[2], data_buffer[3], data_buffer[4] };
            Array.Reverse(channel_identifier);
            CHANNEL_IDENTIFIER = BitConverter.ToUInt32(channel_identifier, 0);

            //Click Type
            switch (data_buffer[5])
            {
                case 0: CLICK_TYPE = ClickType.ButtonDown; break;
                case 1: CLICK_TYPE = ClickType.ButtonUp; break;
                case 2: CLICK_TYPE = ClickType.ButtonClick; break;
                case 3: CLICK_TYPE = ClickType.ButtonSingleClick; break;
                case 4: CLICK_TYPE = ClickType.ButtonDoubleClick; break;
                case 5: CLICK_TYPE = ClickType.ButtonHold; break;
                default: throw new Exception("Unknown ClicKType value");
            }

            //Queued
            QUEUED = BitConverter.ToBoolean(data_buffer, 6);

            //Time Difference
            TIME_DIFFERENCE = BitConverter.ToInt32(data_buffer, 7);
        }

        public override string ToString()
        {
            string info = "";

            //Connection Channel Identifier
            info += "Connection Channel Identifier: ";
            info += CHANNEL_IDENTIFIER;
            info += "\n";

            //Click Type
            info += "Click Type: ";
            info += CLICK_TYPE;
            info += "\n";

            //Queued
            info += "Queued: ";
            info += QUEUED;
            info += "\n";

            //Time Difference
            info += "Time Differnece: ";
            info += TIME_DIFFERENCE;

            return info;
        }
    }
}
