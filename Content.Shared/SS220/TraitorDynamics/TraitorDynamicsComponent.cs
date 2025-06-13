using Robust.Shared.Prototypes;

namespace Content.Shared.SS220.TraitorDynamics;

[RegisterComponent]
public sealed partial class TraitorDynamicsComponent :Component
{
    [DataField]
    public ProtoId<DynamicPrototype> CurrentDynamic;
}


public sealed class TraitorRuleAddedEvent : EntityEventArgs
{
    public EntityUid RuleEnt;

    public TraitorRuleAddedEvent(EntityUid ruleEnt)
    {
        RuleEnt = ruleEnt;
    }
}

public sealed class DynamicAddedEvent : EntityEventArgs
{
    public EntityUid Entity;
    public ProtoId<DynamicPrototype> Dynamic;

    public DynamicAddedEvent(EntityUid entity, ProtoId<DynamicPrototype> dynamic)
    {
        Entity = entity;
        Dynamic = dynamic;
    }
}
