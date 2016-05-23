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
        //Delegates
        public delegate void FlicAdvertisementPacket(EvtAdvertisementPacket eap);
        public delegate void FlicCreateConnectionChannelResponse(EvtCreateConnectionChannelResponse ecccr);
        public delegate void FlicConnectionStatusChanged(EvtConnectionStatusChanged ecsc);
        public delegate void FlicConnectionChannelRemoved(EvtConnectionChannelRemoved eccr);
        public delegate void FlicButtonUpOrDown(EvtButtonUpOrDown ebuod);
        public delegate void FlicButtonClickOrHold(EvtButtonClickOrHold ebcoh);
        public delegate void FlicButtonSingleOrDoubleClick(EvtButtonSingleOrDoubleClick ebsodc);
        public delegate void FlicButtonSingleOrDoubleClickOrHold(EvtButtonSingleOrDoubleClickOrHold ebsodcoh);
        public delegate void FlicNewVerifiedButton(EvtNewVerifiedButton envb);        
        public delegate void FlicGetInfoResponse(EvtGetInfoResponse egir);
        public delegate void FlicNoSpaceForNewConnection(EvtNoSpaceForNewConnection ensfnc);
        public delegate void FlicGotSpaceForNewConnection(EvtGotSpaceForNewConnection egsfnc);
        public delegate void FlicBluetoothControllerStateChange(EvtBluetoothControllerStateChange ebcsc);
        public delegate void FlicPingResponse(EvtPingResponse evr);

        //Events
        public event FlicAdvertisementPacket AdvertisementPacket;
        public event FlicCreateConnectionChannelResponse CreateConnectionChannelResponse;
        public event FlicConnectionStatusChanged ConnectionStatusChanged;
        public event FlicConnectionChannelRemoved ConnectionChannelRemoved;
        public event FlicButtonUpOrDown ButtonUpOrDown;
        public event FlicButtonClickOrHold ButtonClickOrHold;
        public event FlicButtonSingleOrDoubleClick ButtonSingleOrDoubleClick;
        public event FlicButtonSingleOrDoubleClickOrHold ButtonSingleOrDoubleClickOrHold;
        public event FlicNewVerifiedButton NewVerifiedButton;
        public event FlicGetInfoResponse GetInfoResponse;
        public event FlicNoSpaceForNewConnection NoSpaceForNewConnection;
        public event FlicGotSpaceForNewConnection GotSpaveForNewConnection;
        public event FlicBluetoothControllerStateChange BluetoothControllerStateChange;
        public event FlicPingResponse PingResponse;

        private Socket SOCKET { get; set; } //Communicationchannel from Client to Daemon        

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
        
        public void DisconnectFromDaemon()
        {
            try
            {
                CONNECTED = false;
                SOCKET.Shutdown(SocketShutdown.Both);                
            }
            catch (Exception)
            {
                throw new Exception("Error shutting down the socket connection to daemon!");
            }
        }

        #region Commands
        // OP = 0
        public void GetInfo()
        {            
            try
            {
                byte[] buffer = new byte[3];
                //Header length
                buffer[0] = 0x01;
                buffer[1] = 0x00;
                //OP Code
                buffer[2] = (byte)(Command.CmdGetInfo);
                //Send data
                SOCKET.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // OP = 1
        public void CreateScanner(UInt32 scanner_id)
        {
            try
            {
                byte[] buffer = new byte[7];
                //Header length
                buffer[0] = 0x05;
                buffer[1] = 0x00;
                //OP Code
                buffer[2] = (byte)(Command.CmdCreateScanner);
                //Ping ID
                byte[] scanner_id_buffer = BitConverter.GetBytes(scanner_id);
                Array.Reverse(scanner_id_buffer);
                buffer[3] = scanner_id_buffer[0];
                buffer[4] = scanner_id_buffer[1];
                buffer[5] = scanner_id_buffer[2];
                buffer[6] = scanner_id_buffer[3];
                //Send data
                SOCKET.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // OP = 2
        public void RemoveScanner(UInt32 scanner_id)
        {
            try
            {
                byte[] buffer = new byte[7];
                //Header length
                buffer[0] = 0x05;
                buffer[1] = 0x00;
                //OP Code
                buffer[2] = (byte)(Command.CmdRemoveScanner);
                //Ping ID
                byte[] ping_id_buffer = BitConverter.GetBytes(scanner_id);
                Array.Reverse(ping_id_buffer);  //Make it little endian
                buffer[3] = ping_id_buffer[0];
                buffer[4] = ping_id_buffer[1];
                buffer[5] = ping_id_buffer[2];
                buffer[6] = ping_id_buffer[3];
                //Send data
                SOCKET.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // OP = 3
        public void CreateConnectionChannel(FlicButton flic_to_add, LatencyMode latency_mode, UInt16 auto_disconnect_time)
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
            byte[] handle_buffer = BitConverter.GetBytes(flic_to_add.HANDLE);
            Array.Reverse(handle_buffer);  //Make it little endian
            buffer[3] = handle_buffer[0];
            buffer[4] = handle_buffer[1];
            buffer[5] = handle_buffer[2];
            buffer[6] = handle_buffer[3];

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

        // OP = 4
        public void RemoveConnectionChannel(FlicButton flic_to_remove)
        {
            try
            {
                byte[] buffer = new byte[7];
                //Header length
                buffer[0] = 0x05;
                buffer[1] = 0x00;
                //OP Code
                buffer[2] = (byte)(Command.CmdRemoveConnectionChannel);
                //Handle
                byte[] handle_buffer = BitConverter.GetBytes(flic_to_remove.HANDLE);
                Array.Reverse(handle_buffer);  //Make it little endian
                buffer[3] = handle_buffer[0];
                buffer[4] = handle_buffer[1];
                buffer[5] = handle_buffer[2];
                buffer[6] = handle_buffer[3];
                //Send data
                SOCKET.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // OP = 5
        public void ForceDisconnect(FlicButton flic_to_remove)
        {
            try
            {
                byte[] buffer = new byte[9];
                //Header length
                buffer[0] = 0x09;
                buffer[1] = 0x00;
                //OP Code
                buffer[2] = (byte)(Command.CmdForceDisconnect);
                //Bluetooth Address                
                buffer[3] = flic_to_remove.BT_ADDRESS.ADDRESS[0];
                buffer[4] = flic_to_remove.BT_ADDRESS.ADDRESS[1];
                buffer[5] = flic_to_remove.BT_ADDRESS.ADDRESS[2];
                buffer[6] = flic_to_remove.BT_ADDRESS.ADDRESS[3];
                buffer[7] = flic_to_remove.BT_ADDRESS.ADDRESS[4];
                buffer[8] = flic_to_remove.BT_ADDRESS.ADDRESS[5];
                //Send data
                SOCKET.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // OP = 6
        public void ChangeModeParameters(FlicButton flic_to_remove, LatencyMode latency_mode, Int16 auto_disconnect_time)
        {
            try
            {
                byte[] buffer = new byte[10];
                //Header length
                buffer[0] = 0x0A;
                buffer[1] = 0x00;
                //OP Code
                buffer[2] = (byte)(Command.CmdChangeModeParameters);
                //Handle              
                byte[] handle_buffer = BitConverter.GetBytes(flic_to_remove.HANDLE);
                Array.Reverse(handle_buffer);  //Make it little endian
                buffer[3] = handle_buffer[0];
                buffer[4] = handle_buffer[1];
                buffer[5] = handle_buffer[2];
                buffer[6] = handle_buffer[3];
                //Latency mode
                buffer[7] = (byte)(latency_mode);
                //Auto disconnect time
                byte[] auto_disconnect_time_buffer = BitConverter.GetBytes(auto_disconnect_time);
                Array.Reverse(auto_disconnect_time_buffer);  //Make it little endian
                buffer[3] = auto_disconnect_time_buffer[0];
                buffer[4] = auto_disconnect_time_buffer[1];
                //Send data
                SOCKET.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // OP = 7
        public void Ping(UInt32 ping_id)
        {
            try
            {
                byte[] buffer = new byte[7];
                //Header length
                buffer[0] = 0x05;
                buffer[1] = 0x00;
                //OP Code
                buffer[2] = (byte)(Command.CmdPing);
                //Ping ID
                byte[] ping_id_buffer = BitConverter.GetBytes(ping_id);
                Array.Reverse(ping_id_buffer);
                buffer[3] = ping_id_buffer[0];
                buffer[4] = ping_id_buffer[1];
                buffer[5] = ping_id_buffer[2];
                buffer[6] = ping_id_buffer[3];
                //Send data
                SOCKET.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

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
            }
        }

        #region EvtParser
        //To Be Done OP = 0
        private void Process_EvtAdvertisementPacket(byte[] receive_buffer)
        {
            EvtAdvertisementPacket eav = new EvtAdvertisementPacket(receive_buffer);
            AdvertisementPacket?.Invoke(eav);
        }

        //OP = 1
        private void Process_EvtCreateConnectionChannelResponse(byte[] receive_buffer)
        {
            EvtCreateConnectionChannelResponse eccr = new EvtCreateConnectionChannelResponse(receive_buffer);
            CreateConnectionChannelResponse?.Invoke(eccr);
        }

        //OP = 2
        private void Process_EvtConnectionStatusChanged(byte[] receive_buffer)
        {
            EvtConnectionStatusChanged ecsc = new EvtConnectionStatusChanged(receive_buffer);
            ConnectionStatusChanged?.Invoke(ecsc);
        }

        //OP = 3
        private void Process_EvtConnectionChannelRemoved(byte[] receive_buffer)
        {
            EvtConnectionChannelRemoved eccr = new EvtConnectionChannelRemoved(receive_buffer);
            ConnectionChannelRemoved?.Invoke(eccr);            
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
            ButtonUpOrDown?.Invoke(ebuod);
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
            ButtonClickOrHold?.Invoke(ebcoh);
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
            ButtonSingleOrDoubleClick?.Invoke(ebsodc);
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
            ButtonSingleOrDoubleClickOrHold?.Invoke(ebsodcoh);
        }

        //To Be Done OP = 8
        private void Process_EvtNewVerifiedButton(byte[] receive_buffer)
        {
            EvtNewVerifiedButton envb = new EvtNewVerifiedButton(receive_buffer);
            NewVerifiedButton?.Invoke(envb);            
        }

        //OP = 9
        private void Process_EvtGetInfoResponse(byte[] receive_buffer)
        {
            EvtGetInfoResponse egir = new EvtGetInfoResponse(receive_buffer);
            GetInfoResponse?.Invoke(egir);
        }        

        //To Be Done OP = 10
        private void Process_EvtNoSpaceForNewConnection(byte[] receive_buffer)
        {
            EvtNoSpaceForNewConnection ensfnc = new EvtNoSpaceForNewConnection(receive_buffer);
            NoSpaceForNewConnection?.Invoke(ensfnc);
        }

        //To Be Done OP = 11
        private void Process_EvtGotSpaceForNewConnection(byte[] receive_buffer)
        {
            EvtGotSpaceForNewConnection egsfnc = new EvtGotSpaceForNewConnection(receive_buffer);
            GotSpaveForNewConnection?.Invoke(egsfnc);
        }

        //To Be Done OP = 12
        private void Process_EvtBluetoothControllerStateChange(byte[] receive_buffer)
        {
            EvtBluetoothControllerStateChange ebcsc = new EvtBluetoothControllerStateChange(receive_buffer);
            BluetoothControllerStateChange?.Invoke(ebcsc);
        }

        //To Be Done OP = 13
        private void Process_EvtPingResponse(byte[] receive_buffer)
        {
            EvtPingResponse epr = new EvtPingResponse(receive_buffer);
            PingResponse?.Invoke(epr);
        }
        #endregion

        //To Do
        public void RestoreConfiguration()
        {
            throw new NotImplementedException();
        }        

        public override string ToString()
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
