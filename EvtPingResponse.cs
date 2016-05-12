using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class EvtPingResponse
    {
        public UInt32 PING_ID { get; set; }

        public EvtPingResponse(byte[] data_buffer)
        {
            //Scanner Identifier
            byte[] ping_identifier = new byte[] { data_buffer[1], data_buffer[2], data_buffer[3], data_buffer[4] };
            Array.Reverse(ping_identifier);
            PING_ID = BitConverter.ToUInt32(ping_identifier, 0);
        }

        public override string ToString()
        {
            return "Ping ID: " + PING_ID;
        }
    }
}
