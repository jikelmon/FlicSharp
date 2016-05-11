using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
    public class CommandBuilder
    {
    }

    public enum CreateConnectionChannelError
    {
        NoError,
        MaxPendingConnectionsReached
    }

    public enum ConnectionStatus
    {
        Disconnected,
        Connected,
        Ready
    }

    public enum DisconnectReason
    {
        Unspecified,
        ConnectionEstablishmentFailed,
        TimedOut,
        BondingKeysMismatch
    }

    public enum RemovedReason
    {
        RemovedByThisClient,
        ForceDisconnectedByThisClient,
        ForceDisconnectedByOtherClient,
        ButtonIsPrivate,
        VerifyTimeout,
        InternetBackendError,
        InvalidData
    }

    public enum ClickType
    {
        ButtonDown,
        ButtonUp,
        ButtonClick,
        ButtonSingleClick,
        ButtonDoubleClick,
        ButtonHold
    }

    public enum BdAddrType
    {
        PublicBdAddrType,
        RandomBdAddrType
    }

    public enum LatencyMode
    {
        Normal,
        Low,
        High
    }

    public enum BluetoothControllerState
    {
        Detached,
        Resetting,
        Attached
    }

    public enum Command
    {
        CmdGetInfo,
        CmdCreateScanner,
        CmdRemoveScanner,
        CmdCreateConnectionChannel,
        CmdRemoveConnectionChannel,
        CmdForceDisconnect,
        CmdChangeModeParameters,
        CmdPing
    }

    public enum Event
    {
        EvtAdvertisementPacket = 0,
        EvtCreateConnectionChannelResponse = 1,
        EvtConnectionStatusChanged = 2,
        EvtConnectionChannelRemoved = 3,
        EvtButtonUpOrDown = 4,
        EvtButtonClickOrHold = 5,
        EvtButtonSingleOrDoubleClick = 6,
        EvtButtonSingleOrDoubleClickOrHold = 7,
        EvtNewVerifiedButton = 8,
        EvtGetInfoResponse = 9,
        EvtNoSpaceForNewConnection = 10,
        EvtGotSpaceForNewConnection = 11,
        EvtBluetoothControllerStateChange = 12,
        EvtPingResponse = 13
    }
}
