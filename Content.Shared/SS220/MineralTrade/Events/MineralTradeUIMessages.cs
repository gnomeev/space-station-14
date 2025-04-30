using Content.Shared.SS220.MineralTrade.Protos;
using Robust.Shared.Serialization;

namespace Content.Shared.SS220.MineralTrade.Events;

[Serializable, NetSerializable]
public sealed class MineralTradeState : BoundUserInterfaceState
{
    public List<MineralListingPrototype> Listings;
    public Dictionary<MineralListingPrototype, int> Checkout;
    public int Balance;

    public MineralTradeState(List<MineralListingPrototype> listings, Dictionary<MineralListingPrototype, int> checkout, int balance)
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
    public int Amount;

    public AddToCartMsg(string id, int amount)
    {
        Id = id;
        Amount = amount;
    }
}

[Serializable, NetSerializable]
public sealed class CheckoutMsg : BoundUserInterfaceMessage
{
    public Dictionary<MineralListingPrototype, int> Checkout;

    public CheckoutMsg(Dictionary<MineralListingPrototype, int> checkout)
    {
        Checkout = checkout;
    }
}
