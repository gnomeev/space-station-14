using Robust.Shared.Prototypes;

namespace Content.Shared.SS220.TraitorDynamics;

[Prototype]
public sealed partial class DynamicPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public LocId Name;

    [DataField]
    public int LimitAntag;

    [DataField]
    public int LimitSleeperAntag;

    [DataField]
    public ProtoId<DynamicNamePrototype> LoreNames;

    public LocId SelectedLoreName;
}
