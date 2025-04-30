using Content.Shared.SS220.MineralTrade.Protos;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.SS220.MineralTrade;

[RegisterComponent, NetworkedComponent]
public sealed partial class MineralTradeComponent : Component
{
    [DataField]
    public List<MineralListingPrototype> AvailableListings = new();

    /// <summary>
    /// for comic effect
    /// </summary>
    [DataField]
    public bool ShouldThrow = false;

    public Dictionary<MineralListingPrototype, int> Checkout = new();

    public int Balance;
}

[Serializable, NetSerializable]
public enum MineralTradeTerminalUiKey
{
    Key,
}
