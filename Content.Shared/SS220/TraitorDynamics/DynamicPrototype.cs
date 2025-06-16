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
    public List<LocId> ListLoreName = new();

    [DataField]
    public LocId EndRoundName;

    public LocId SelectedLoreName;
}
