using Content.Shared.SS220.MineralTrade.Protos;
using Robust.Shared.Containers;
using Robust.Shared.Serialization;

namespace Content.Shared.SS220.MineralTrade;

[RegisterComponent]
public sealed partial class MineralTradeComponent : Component
{
    [DataField]
    public List<MineralListingPrototype> AvailableListings = new();

    /// <summary>
    /// for comic effect
    /// </summary>
    [DataField]
    public bool ShouldThrow;

    public List<MineralListingPrototype> Checkout = new();

    public int Balance;
}

[Serializable, NetSerializable]
public enum MineralTradeTerminalUiKey
{
    Key,
}
