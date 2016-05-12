using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlicSharp
{
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
        EvtAdvertisementPacket,
        EvtCreateConnectionChannelResponse,
        EvtConnectionStatusChanged,
        EvtConnectionChannelRemoved,
        EvtButtonUpOrDown,
        EvtButtonClickOrHold,
        EvtButtonSingleOrDoubleClick,
        EvtButtonSingleOrDoubleClickOrHold,
        EvtNewVerifiedButton,
        EvtGetInfoResponse,
        EvtNoSpaceForNewConnection,
        EvtGotSpaceForNewConnection,
        EvtBluetoothControllerStateChange,
        EvtPingResponse
    }
}
