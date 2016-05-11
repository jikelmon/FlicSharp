using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class FlicButton
    {
        public delegate void FlicButtonAction(FlicButton flic_button);

        public string NAME { get; set; }   //Name of the Flic button
        public uint HANDLE { get; set; } //Handle number for managing multiple buttons
        public BluetoothAddress BT_ADDRESS { get; set; }
        public List<ClickType> PRESS_HISTORY { get; set; }

        public event FlicButtonAction SinglePressed;
        public event FlicButtonAction DoublePressed;
        public event FlicButtonAction Hold;

        public FlicButton(string flic_name, uint flic_handle, string bluetooth_address)
        {
            PRESS_HISTORY = new List<ClickType>();
            NAME = flic_name;
            HANDLE = flic_handle;
            BT_ADDRESS = BluetoothAddress.Parse(bluetooth_address);
        }

        public void AddPressToHistory(ClickType click_type)
        {
            if ((PRESS_HISTORY.Count == 0 && click_type == ClickType.ButtonSingleClick) ||
                (PRESS_HISTORY.Count == 0 && click_type == ClickType.ButtonUp))
            {
                return;
            }
            PRESS_HISTORY.Add(click_type);

            //SingleClick
            try
            {                
                if (PRESS_HISTORY[PRESS_HISTORY.Count - 5] == ClickType.ButtonDown &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 4] == ClickType.ButtonUp &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 3] == ClickType.ButtonClick &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 2] == ClickType.ButtonSingleClick &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 1] == ClickType.ButtonSingleClick)
                {
                    SinglePressed(this);
                    PRESS_HISTORY.Clear();
                }
            }
            catch
            {
                //Do nothing
            }

            //Hold
            try
            {                
                if (PRESS_HISTORY[PRESS_HISTORY.Count - 3] == ClickType.ButtonDown &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 2] == ClickType.ButtonHold &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 1] == ClickType.ButtonHold)
                {
                    Hold(this);
                    PRESS_HISTORY.Clear();
                }
            }
            catch
            {
                //Do nothing
            }

            //DoublePress
            try
            {
                if (PRESS_HISTORY[PRESS_HISTORY.Count - 8] == ClickType.ButtonDown &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 7] == ClickType.ButtonUp &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 6] == ClickType.ButtonClick &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 5] == ClickType.ButtonDown &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 4] == ClickType.ButtonUp &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 3] == ClickType.ButtonClick &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 2] == ClickType.ButtonDoubleClick &&
                    PRESS_HISTORY[PRESS_HISTORY.Count - 1] == ClickType.ButtonDoubleClick)
                {
                    DoublePressed(this);
                    PRESS_HISTORY.Clear();
                }
            }
            catch
            {
                //Do nothing
            }
        }

        public override string ToString()
        {
            return "Name: " + NAME + " - Handle: " + HANDLE + " - BT-Address: " + BT_ADDRESS.ToString();
        }
    }
}
