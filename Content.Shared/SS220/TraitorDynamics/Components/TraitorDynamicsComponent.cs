using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.SS220.TraitorDynamics.Components;

[RegisterComponent, Access(typeof(SharedTraitorDynamicsSystem))]
public sealed partial class TraitorDynamicsComponent :Component
{
    [DataField]
    public ProtoId<DynamicPrototype>? CurrentDynamic;
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
