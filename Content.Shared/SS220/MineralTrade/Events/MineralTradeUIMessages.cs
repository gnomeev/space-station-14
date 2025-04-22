using Content.Shared.SS220.MineralTrade.Protos;
using Robust.Shared.Serialization;

namespace Content.Shared.SS220.MineralTrade.Events;

[Serializable, NetSerializable]
public sealed class MineralTradeState : BoundUserInterfaceState
{
    public List<MineralListingPrototype> Listings;
    public List<MineralListingPrototype> Checkout;
    public int Balance;

    public MineralTradeState(List<MineralListingPrototype> listings, List<MineralListingPrototype> checkout, int balance)
    {
        Listings = listings;
        Checkout = checkout;
        Balance = balance;
    }

}
[Serializable, NetSerializable]
public sealed class AddToCartMsg : BoundUserInterfaceMessage
{
    public string Id;

    public AddToCartMsg(string id)
    {
        Id = id;
    }
}
