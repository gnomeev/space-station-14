using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Content.Shared.SS220.TraitorDynamics; //SS220 - show-in-uplink-type-dynamic

namespace Content.Shared.Store;

[Serializable, NetSerializable]
public enum StoreUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class StoreUpdateState : BoundUserInterfaceState
{
    public readonly HashSet<ListingDataWithCostModifiers> Listings;

    public readonly Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Balance;

    public readonly bool ShowFooter;

    //SS220 - show-in-uplink-type-dynamic-start
    public readonly ProtoId<DynamicPrototype>? Dynamic;
    //SS220 - show-in-uplink-type-dynamic-end

    public readonly bool AllowRefund;

    //SS220 - show-in-uplink-type-dynamic-start
    public StoreUpdateState(HashSet<ListingDataWithCostModifiers> listings, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> balance, bool showFooter, bool allowRefund, ProtoId<DynamicPrototype>? dynamic = default )
    {
        Listings = listings;
        Balance = balance;
        ShowFooter = showFooter;
        AllowRefund = allowRefund;
        Dynamic = dynamic;
    }
    //SS220 - show-in-uplink-type-dynamic-end
}

[Serializable, NetSerializable]
public sealed class StoreRequestUpdateInterfaceMessage : BoundUserInterfaceMessage
{

}

[Serializable, NetSerializable]
public sealed class StoreBuyListingMessage(ProtoId<ListingPrototype> listing) : BoundUserInterfaceMessage
{
    public ProtoId<ListingPrototype> Listing = listing;
}

[Serializable, NetSerializable]
public sealed class StoreRequestWithdrawMessage : BoundUserInterfaceMessage
{
    public string Currency;

    public int Amount;

    public StoreRequestWithdrawMessage(string currency, int amount)
    {
        Currency = currency;
        Amount = amount;
    }
}

/// <summary>
///     Used when the refund button is pressed
/// </summary>
[Serializable, NetSerializable]
public sealed class StoreRequestRefundMessage : BoundUserInterfaceMessage
{

}
