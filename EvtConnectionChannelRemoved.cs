using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtConnectionChannelRemoved
    {
        public UInt32 CONNECTION_IDENTIFIER { get; set; }
        public RemovedReason REMOVED_REASON { get; set; }        

        public EvtConnectionChannelRemoved(byte[] data_buffer)
        {
            //Connection Channel Identifier
            byte[] connection_identifier = new byte[] { data_buffer[1], data_buffer[2], data_buffer[3], data_buffer[4] };
            Array.Reverse(connection_identifier);
            CONNECTION_IDENTIFIER = BitConverter.ToUInt32(connection_identifier, 0);

            //Connection Status
            switch (data_buffer[5])
            {
                case 0: REMOVED_REASON = RemovedReason.RemovedByThisClient; break;
                case 1: REMOVED_REASON = RemovedReason.ForceDisconnectedByThisClient; break;
                case 2: REMOVED_REASON = RemovedReason.ForceDisconnectedByOtherClient; break;
                case 3: REMOVED_REASON = RemovedReason.ButtonIsPrivate; break;
                case 4: REMOVED_REASON = RemovedReason.VerifyTimeout; break;
                case 5: REMOVED_REASON = RemovedReason.InternetBackendError; break;
                case 6: REMOVED_REASON = RemovedReason.InvalidData; break;
                default: throw new Exception("Unknown RemnovedReason value");
            }            
        }

        public override string ToString()
        {
            string info = "";

            //Connection Channel Identifier
            info += "Connection Channel Identifier: ";
            info += CONNECTION_IDENTIFIER;
            info += "\n";

            //Removed reason
            info += "Removed Reason: ";
            info += REMOVED_REASON;
            info += "\n";

            return info;
        }
    }
}
