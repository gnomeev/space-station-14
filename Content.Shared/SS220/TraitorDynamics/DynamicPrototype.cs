using Content.Shared.Random;
using Content.Shared.Store;
using Robust.Shared.Prototypes;

namespace Content.Shared.SS220.TraitorDynamics;

[Prototype]
public sealed partial class DynamicPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public float DynamicPriceMultiplier = 2.0f;

    [DataField]
    public int LimitAntag;

    [DataField]
    public int LimitSleeperAntag;

    [DataField]
    public LocId LoreNameDynamic;

    [DataField]
    public LocId EndRoundNameDynamic;
}
