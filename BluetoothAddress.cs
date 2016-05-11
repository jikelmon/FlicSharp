using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class BluetoothAddress
    {
        public byte[] ADDRESS { get; set; }

        public BluetoothAddress(byte[] bt_address)
        {
            if (bt_address.Length != 6)
            {
                throw new Exception("Wrong format of bluetooth address");
            }
            ADDRESS = bt_address;
        }

        public override string ToString()
        {
            string info = "";
            for (int i = 0; i < ADDRESS.Length; i++)
            {
                info += String.Format("{0:X2}", ADDRESS[i]);
                if (i != ADDRESS.Length - 1)
                {
                    info += ":";
                }
            }
            return info;
        }

        public static BluetoothAddress Parse(string bluetooth_address)
        {
            //00:11:22:33:44:55
            if (bluetooth_address.Contains(":"))
            {
                if (bluetooth_address.Length < 11 || bluetooth_address.Length > 17)
                {
                    throw new Exception("Wrong format of bluetooth address");
                }
                else
                {
                    return new BluetoothAddress(new byte[] { Convert.ToByte(bluetooth_address.Substring(0, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(3, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(6, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(9, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(12, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(15, 2), 16)});
                }
            }
            //001122334455
            else
            {
                if (bluetooth_address.Length != 12)
                {
                    throw new Exception("Wrong format of bluetooth address");
                }
                else
                {
                    return new BluetoothAddress(new byte[] { Convert.ToByte(bluetooth_address.Substring(0, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(2, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(4, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(6, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(8, 2), 16),
                                        Convert.ToByte(bluetooth_address.Substring(10, 2), 16)});
                }
            }
        }

        public static bool operator ==(BluetoothAddress a, BluetoothAddress b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            for (int i = 0; i < a.ADDRESS.Length; i++)
            {
                if (a.ADDRESS[i] != b.ADDRESS[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(BluetoothAddress a, BluetoothAddress b)
        {
            return !(a == b);
        }
    }
}
