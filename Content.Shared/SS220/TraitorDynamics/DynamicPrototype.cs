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
    public Dictionary<string, int> AntagLimits = new();

    [DataField]
    public int PlayersRequerment;

    [DataField]
    public ProtoId<DynamicNamePrototype> LoreNames;

    public LocId SelectedLoreName;
}
