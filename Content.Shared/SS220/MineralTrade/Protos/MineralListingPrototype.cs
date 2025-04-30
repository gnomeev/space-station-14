using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.SS220.MineralTrade.Protos;

[NetSerializable, Serializable, Prototype("mineralListing")]
public sealed class MineralListingPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public LocId? Name;

    [DataField]
    public int Price = 0;

    [DataField]
    public EntProtoId? ListingEntityId = default!;

}
