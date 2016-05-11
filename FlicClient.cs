using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class FlicClient
    {
        public delegate void FlicDaemonEvent(EvtGetInfoResponse egir);
        public delegate void ConnectedToDaemon();

        private Socket SOCKET { get; set; } //Communicationchannel from Client to Daemon
        public event FlicDaemonEvent Info;

        public List<FlicButton> BUTTONS { get; set; }   //A list holding the added buttons
        public bool CONNECTED { get; set; } //Indicates the connectionstatus to the daemon

        public FlicClient(string daemon_ip, int daemon_port = 5551)
        {
            CONNECTED = false;
            try
            {
                SOCKET = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SOCKET.Connect(IPAddress.Parse(daemon_ip), daemon_port);
                CONNECTED = true;
                Console.WriteLine(DateTime.Now.ToString() + ": Successfully connected to Daemon");
                BUTTONS = new List<FlicButton>();
                Thread bg_worker = new Thread(Listen);
                bg_worker.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": Cannot connect to Daemon. Is the daemon running?");
                throw ex;
            }
        }

        public void GetDaemonInfo()
        {
            byte[] buffer = new byte[3];
            //Header length
            buffer[0] = 0x01;
            buffer[1] = 0x00;
            //OP Code
            buffer[2] = 0x00;
            try
            {
                SOCKET.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Listen()
        {
            while (CONNECTED)
            {
                //Receive Header
                byte[] header_buffer = new byte[2];
                int bytes_received = SOCKET.Receive(header_buffer, header_buffer.Length, 0);                

                //Received Data
                byte[] data_buffer = new byte[BitConverter.ToUInt16(header_buffer, 0)];
                bytes_received = SOCKET.Receive(data_buffer, data_buffer.Length, 0);                

                switch (data_buffer[0])
                {
                    case (byte)(Event.EvtAdvertisementPacket): Process_EvtAdvertisementPacket(data_buffer); break;
                    case (byte)(Event.EvtCreateConnectionChannelResponse): Process_EvtCreateConnectionChannelResponse(data_buffer); break;
                    case (byte)(Event.EvtConnectionStatusChanged): Process_EvtConnectionStatusChanged(data_buffer); break;
                    case (byte)(Event.EvtConnectionChannelRemoved): Process_EvtConnectionChannelRemoved(data_buffer); break;
                    case (byte)(Event.EvtButtonUpOrDown): Process_EvtButtonUpOrDown(data_buffer); break;
                    case (byte)(Event.EvtButtonClickOrHold): Process_EvtButtonClickOrHold(data_buffer); break;
                    case (byte)(Event.EvtButtonSingleOrDoubleClick): Process_EvtButtonSingleOrDoubleClick(data_buffer); break;
                    case (byte)(Event.EvtButtonSingleOrDoubleClickOrHold): Process_EvtButtonSingleOrDoubleClickOrHold(data_buffer); break;
                    case (byte)(Event.EvtNewVerifiedButton): Process_EvtNewVerifiedButton(data_buffer); break;
                    case (byte)(Event.EvtGetInfoResponse): Process_EvtGetInfoResponse(data_buffer); break;
                    case (byte)(Event.EvtNoSpaceForNewConnection): Process_EvtNoSpaceForNewConnection(data_buffer); break;
                    case (byte)(Event.EvtGotSpaceForNewConnection): Process_EvtGotSpaceForNewConnection(data_buffer); break;
                    case (byte)(Event.EvtBluetoothControllerStateChange): Process_EvtBluetoothControllerStateChange(data_buffer); break;
                    case (byte)(Event.EvtPingResponse): Process_EvtPingResponse(data_buffer); break;
                    default: Console.WriteLine("Unknown"); break;
                }

                #region
                /*Console.WriteLine(DateTime.Now.ToString() + ": Antwort");
                for (int k = 0; k < bytes_received; k++)
                {
                    Console.WriteLine(recv_buffer[k]);
                }

                for (int i = 0; i < periods; i++)
                {
                    byte[] handle_buffer = new byte[4];
                    for (int j = 0; j < 4; j++)
                    {
                        handle_buffer[j] = recv_buffer[i * 13 + j + 3];
                    }
                    Array.Reverse(handle_buffer);
                    uint handle = BitConverter.ToUInt32(handle_buffer, 0);    //Button handle
                    byte op_code = recv_buffer[i * 13 + 2];
                    //OP Code
                    /*if (op_code == 4 || op_code == 5 || op_code == 6 || op_code == 7)
                    {
                        foreach (FlicButton fb in BUTTONS)
                        {
                            if (handle == fb.HANDLE)
                            {
                                fb.LAST_PRESSES.Add(op_code);
                                break;
                            }
                        }
                    }

                    switch (op_code)
                    {
                        case (byte)(Event.EvtAdvertisementPacket): Console.WriteLine("EvtAdvertisementPacket"); break;
                        case (byte)(Event.EvtCreateConnectionChannelResponse): Console.WriteLine("EvtCreateConnectionChannelResponse"); Console.WriteLine(Process_EvtCreateConnectionChannelResponse(recv_buffer)); break;
                        case (byte)(Event.EvtConnectionStatusChanged): Console.WriteLine("EvtConnectionStatusChanged"); Console.WriteLine(Process_EvtConnectionStatusChanged(recv_buffer)); break;
                        case (byte)(Event.EvtConnectionChannelRemoved): Console.WriteLine("EvtConnectionChannelRemoved"); Console.WriteLine(Process_EvtConnectionChannelRemoved(recv_buffer)); break;
                        case (byte)(Event.EvtButtonUpOrDown): Console.WriteLine("EvtButtonUpOrDown"); break;
                        case (byte)(Event.EvtButtonClickOrHold): Console.WriteLine("EvtButtonClickOrHold"); break;
                        case (byte)(Event.EvtButtonSingleOrDoubleClick): Console.WriteLine("EvtButtonSingleOrDoubleClick"); break;
                        case (byte)(Event.EvtButtonSingleOrDoubleClickOrHold): Console.WriteLine("EvtButtonSingleOrDoubleClickOrHold"); break;
                        case (byte)(Event.EvtNewVerifiedButton): Console.WriteLine("EvtNewVerifiedButton"); break;
                        case (byte)(Event.EvtGetInfoResponse): Console.WriteLine("EvtGetInfoResponse"); break;
                        case (byte)(Event.EvtNoSpaceForNewConnection): Console.WriteLine("EvtNoSpaceForNewConnection"); break;
                        case (byte)(Event.EvtGotSpaceForNewConnection): Console.WriteLine("EvtGotSpaceForNewConnection"); break;
                        case (byte)(Event.EvtBluetoothControllerStateChange): Console.WriteLine("EvtBluetoothControllerStateChange"); break;
                        case (byte)(Event.EvtPingResponse): Console.WriteLine("EvtPingResponse"); break;
                        default: Console.WriteLine("Unknown"); break;
                    }*/
                #endregion
            }
            #region old
            /*foreach (FlicButton fb in BUTTONS)
            {
                //Hold
                if (fb.LAST_PRESSES.Count >= 8)
                {
                    if (fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 8] == 4 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 7] == 4 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 6] == 5 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 5] == 4 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 4] == 4 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 3] == 5 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 2] == 6 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 1] == 7)
                    {
                        Hold(fb);
                        fb.LAST_PRESSES.Clear();
                    }else
                    {
                        fb.LAST_PRESSES.Clear();
                    }
                }
                //Single and Double Presses
                else if (fb.LAST_PRESSES.Count >= 5)
                {
                    if (fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 5] == 4 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 4] == 4 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 3] == 5 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 2] == 6 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 1] == 7)
                    {
                        SinglePress(fb);
                        fb.LAST_PRESSES.Clear();
                    }
                    if (fb.LAST_PRESSES.Count == 5 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 5] == 4 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 4] == 5 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 3] == 7 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 2] == 4 &&
                        fb.LAST_PRESSES[fb.LAST_PRESSES.Count - 1] == 6)
                    {
                        DoublePress(fb);
                        fb.LAST_PRESSES.Clear();
                    }
                }                    
            }*/
            #endregion
        }

        #region EvtParser
        //To Be Done OP = 0
        private void Process_EvtAdvertisementPacket(byte[] receive_buffer)
        {
            Console.WriteLine("EvtAdvertisementPacket - To be Done!");
        }

        //OP = 1
        private void Process_EvtCreateConnectionChannelResponse(byte[] receive_buffer)
        {
            //string info = "";

            ////Connection Channel Identifier
            //info += "Connection Channel Identifier: ";
            //byte[] channel_identifier = new byte[] { receive_buffer[1], receive_buffer[2], receive_buffer[3], receive_buffer[4] };
            //Array.Reverse(channel_identifier);
            //info += BitConverter.ToUInt32(channel_identifier, 0);
            //info += "\n";

            ////Create Connection Channel Error
            //info += "Create Connection Channel Error: ";
            //switch (receive_buffer[5])
            //{
            //    case 0: info += "No Error"; break;
            //    case 1: info += "Max Pending Connections Reached"; break;
            //    default: info += "Unknown"; break;
            //}
            //info += "\n";

            ////Connection Status
            //info += "Connection Status: ";
            //switch (receive_buffer[6])
            //{
            //    case 0: info += "Disconnected"; break;
            //    case 1: info += "Connected"; break;
            //    case 2: info += "Ready"; break;
            //    default: info += "Unknown"; break;
            //}

            EvtCreateConnectionChannelResponse eccr = new EvtCreateConnectionChannelResponse(receive_buffer);
            Console.WriteLine(eccr.ToString());
        }

        //OP = 2
        private void Process_EvtConnectionStatusChanged(byte[] receive_buffer)
        {
            EvtConnectionStatusChanged ecsc = new EvtConnectionStatusChanged(receive_buffer);
            Console.WriteLine(ecsc.ToString());
        }

        //OP = 3
        private void Process_EvtConnectionChannelRemoved(byte[] receive_buffer)
        {
            string info = "";

            //Connection Channel Identifier
            info += "Connection Channel Identifier: ";
            byte[] channel_identifier = new byte[] { receive_buffer[1], receive_buffer[2], receive_buffer[3], receive_buffer[4] };
            Array.Reverse(channel_identifier);
            info += BitConverter.ToUInt32(channel_identifier, 0);
            info += "\n";

            //Connection Status
            info += "Removed Reason: ";
            switch (receive_buffer[5])
            {
                case 0: info += "Removed By ThisClient"; break;
                case 1: info += "Force Disconnected By This Client"; break;
                case 2: info += "Force Disconnected By Other Client"; break;
                case 3: info += "Button Is Private"; break;
                case 4: info += "Verify Timeout"; break;
                case 5: info += "Internet Backend Error"; break;
                case 6: info += "Invalid Data"; break;
                default: info += "Unknown"; break;
            }

            Console.WriteLine(info);
        }

        //OP = 4
        private void Process_EvtButtonUpOrDown(byte[] receive_buffer)
        {
            EvtButtonUpOrDown ebuod = new EvtButtonUpOrDown(receive_buffer);
            foreach (FlicButton fb in BUTTONS)
            {
                if (fb.HANDLE == ebuod.CHANNEL_IDENTIFIER)
                {
                    fb.AddPressToHistory(ebuod.CLICK_TYPE);
                }
            }
            //Console.WriteLine(DateTime.Now.ToString() + ": EvtButtonUpOrDown - " + ebuod.ToString());            
        }

        //OP = 5
        private void Process_EvtButtonClickOrHold(byte[] receive_buffer)
        {
            EvtButtonClickOrHold ebcoh = new EvtButtonClickOrHold(receive_buffer);
            foreach (FlicButton fb in BUTTONS)
            {
                if (fb.HANDLE == ebcoh.CHANNEL_IDENTIFIER)
                {
                    fb.AddPressToHistory(ebcoh.CLICK_TYPE);
                }
            }
            //Console.WriteLine(DateTime.Now.ToString() + ": EvtButtonClickOrHold - " + ebcoh.ToString());
        }

        //OP = 6
        private void Process_EvtButtonSingleOrDoubleClick(byte[] receive_buffer)
        {
            EvtButtonSingleOrDoubleClick ebsodc = new EvtButtonSingleOrDoubleClick(receive_buffer);
            foreach (FlicButton fb in BUTTONS)
            {
                if (fb.HANDLE == ebsodc.CHANNEL_IDENTIFIER)
                {
                    fb.AddPressToHistory(ebsodc.CLICK_TYPE);
                }
            }
            //Console.WriteLine(DateTime.Now.ToString() + ": EvtButtonSingleOrDoubleClick - " + ebsodc);
        }

        //OP = 7
        private void Process_EvtButtonSingleOrDoubleClickOrHold(byte[] receive_buffer)
        {
            EvtButtonSingleOrDoubleClickOrHold ebsodcoh = new EvtButtonSingleOrDoubleClickOrHold(receive_buffer);
            foreach (FlicButton fb in BUTTONS)
            {
                if (fb.HANDLE == ebsodcoh.CHANNEL_IDENTIFIER)
                {
                    fb.AddPressToHistory(ebsodcoh.CLICK_TYPE);
                }
            }
            //Console.WriteLine(DateTime.Now.ToString() + ": EvtButtonSingleOrDoubleClickOrHold - " + ebsodcoh.ToString());
        }

        //To Be Done OP = 8
        private void Process_EvtNewVerifiedButton(byte[] receive_buffer)
        {
            Console.WriteLine("EvtNewVerifiedButton");
            for (int i = 0; i < receive_buffer.Length; i++)
            {
                Console.WriteLine("{0:X}", receive_buffer[i]);
            }
        }

        //OP = 9
        private void Process_EvtGetInfoResponse(byte[] receive_buffer)
        {
            EvtGetInfoResponse egir = new EvtGetInfoResponse(receive_buffer);
            Info(egir);
        }        

        //To Be Done OP = 10
        private void Process_EvtNoSpaceForNewConnection(byte[] receive_buffer)
        {
            Console.WriteLine("EvtNoSpaceForNewConnection");
            for (int i = 0; i < receive_buffer.Length; i++)
            {
                Console.WriteLine("{0:X}", receive_buffer[i]);
            }
        }

        //To Be Done OP = 11
        private void Process_EvtGotSpaceForNewConnection(byte[] receive_buffer)
        {
            Console.WriteLine("EvtGotSpaceForNewConnection");
            for (int i = 0; i < receive_buffer.Length; i++)
            {
                Console.WriteLine("{0:X}", receive_buffer[i]);
            }
        }

        //To Be Done OP = 12
        private void Process_EvtBluetoothControllerStateChange(byte[] receive_buffer)
        {
            Console.WriteLine("EvtBluetoothControllerStateChange");
            for (int i = 0; i < receive_buffer.Length; i++)
            {
                Console.WriteLine("{0:X}", receive_buffer[i]);
            }
        }

        //To Be Done OP = 13
        private void Process_EvtPingResponse(byte[] receive_buffer)
        {
            Console.WriteLine("EvtPingResponse");
            for (int i = 0; i < receive_buffer.Length; i++)
            {
                Console.WriteLine("{0:X}", receive_buffer[i]);
            }
        }
        #endregion

        //To Do
        public void RestoreConfiguration()
        {
            //Used to read the actual configuration and add flic buttons here
        }

        public void ConnectFlic(FlicButton flic_to_add, LatencyMode latency_mode, UInt16 auto_disconnect_time)
        {
            if (CONNECTED == false)
            {
                throw new Exception("Not connected to Daemon");
            }
            for (int i = 0; i < BUTTONS.Count; i++)
            {
                if (BUTTONS[i] == flic_to_add)
                {
                    throw new Exception("Duplicate found");
                }
            }

            byte[] buffer = new byte[16];

            //Header length
            buffer[0] = 0x0E;
            buffer[1] = 0x00;

            //OP Code
            buffer[2] = (byte)(Command.CmdCreateConnectionChannel);

            //Handle
            byte[] handle = BitConverter.GetBytes(flic_to_add.HANDLE);
            Array.Reverse(handle);  //Make it little endian
            for (int i = 0; i < handle.Length; i++)
            {
                buffer[3 + i] = handle[i];
            }

            //BT Address
            byte[] bt_address = flic_to_add.BT_ADDRESS.ADDRESS;
            Array.Reverse(bt_address);
            for (int i = 0; i < bt_address.Length; i++)
            {
                buffer[7 + i] = bt_address[i];
            }

            //Latency
            buffer[13] = (byte)(latency_mode);

            //Auto-Disconnect Time
            byte[] auto_disconnect_time_buffer = BitConverter.GetBytes(auto_disconnect_time);
            buffer[14] = auto_disconnect_time_buffer[0];
            buffer[15] = auto_disconnect_time_buffer[1];

            try
            {
                SOCKET.Send(buffer, buffer.Length, 0);
                BUTTONS.Add(flic_to_add);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetInfo()
        {
            string info = "";
            for (int i = 0; i < BUTTONS.Count; i++)
            {
                info += BUTTONS[i].ToString() + "\n";
            }
            return info;
        }
    }
}
