using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtCreateConnectionChannelResponse
    {
        public uint CHANNEL_IDENTIFIER { get; set; }
        public CreateConnectionChannelError CREATE_CONNECTION_CHANNEL_ERROR { get; set; }
        public ConnectionStatus CONNECTION_STATUS { get; set; }

        public EvtCreateConnectionChannelResponse(byte[] data_buffer)
        {
            //Connection Channel Identifier
            byte[] channel_identifier = new byte[] { data_buffer[1], data_buffer[2], data_buffer[3], data_buffer[4] };
            Array.Reverse(channel_identifier);
            CHANNEL_IDENTIFIER = BitConverter.ToUInt32(channel_identifier, 0);

            //Create Connection Channel Error
            switch (data_buffer[5])
            {
                case 0: CREATE_CONNECTION_CHANNEL_ERROR = CreateConnectionChannelError.NoError; break;
                case 1: CREATE_CONNECTION_CHANNEL_ERROR = CreateConnectionChannelError.MaxPendingConnectionsReached; break;
                default: throw new Exception("Unknown CreateConnectionChannelError value");
            }

            //Connection Status
            switch (data_buffer[6])
            {
                case 0: CONNECTION_STATUS = ConnectionStatus.Disconnected; break;
                case 1: CONNECTION_STATUS = ConnectionStatus.Connected; break;
                case 2: CONNECTION_STATUS = ConnectionStatus.Ready; break;
                default: throw new Exception("Unknown ConnectionStatus value");
            }
        }

        public override string ToString()
        {
            string info = "";

            //Connection Channel Identifier
            info += "Connection Channel Identifier: ";
            info += CHANNEL_IDENTIFIER;
            info += "\n";

            //Create Connection Channel Error
            info += "Create Connection Channel Error: ";
            info += CREATE_CONNECTION_CHANNEL_ERROR;
            info += "\n";

            //Connection Status
            info += "Connection Status: ";
            info += CONNECTION_STATUS;

            return info;
        }
    }
}
