using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtConnectionStatusChanged
    {
        public uint CHANNEL_IDENTIFIER { get; set; }
        public ConnectionStatus CONNECTION_STATUS { get; set; }
        public DisconnectReason DISCONNECT_REASON { get; set; }

        public EvtConnectionStatusChanged(byte[] data_buffer)
        {
            //Connection Channel Identifier
            byte[] channel_identifier = new byte[] { data_buffer[1], data_buffer[2], data_buffer[3], data_buffer[4] };
            Array.Reverse(channel_identifier);
            CHANNEL_IDENTIFIER = BitConverter.ToUInt32(channel_identifier, 0);

            //Connection Status            
            switch (data_buffer[5])
            {
                case 0: CONNECTION_STATUS = ConnectionStatus.Disconnected; break;
                case 1: CONNECTION_STATUS = ConnectionStatus.Connected; break;
                case 2: CONNECTION_STATUS = ConnectionStatus.Ready; break;
                default: throw new Exception("Unknown ConnectionStatus value");
            }

            //Disconnect Reason
            switch (data_buffer[6])
            {
                case 0: DISCONNECT_REASON = DisconnectReason.Unspecified; break;
                case 1: DISCONNECT_REASON = DisconnectReason.ConnectionEstablishmentFailed; break;
                case 2: DISCONNECT_REASON = DisconnectReason.TimedOut; break;
                case 3: DISCONNECT_REASON = DisconnectReason.BondingKeysMismatch; break;
                default: throw new Exception("Unknown DisconnectReason value");
            }
        }

        public override string ToString()
        {
            string info = "";

            //Connection Channel Identifier
            info += "Connection Channel Identifier: ";
            info += CHANNEL_IDENTIFIER;
            info += "\n";

            //Connection Status
            info += "Connection Status: ";
            info += CONNECTION_STATUS;
            info += "\n";

            //Disconnect Reason
            info += "Disconnect Reason: ";
            info += DISCONNECT_REASON;
            info += "\n";

            return info;
        }
    }
}
