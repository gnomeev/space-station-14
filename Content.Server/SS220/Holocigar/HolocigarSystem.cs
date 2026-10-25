using Content.Server.SS220.IgnoreWield;
using Content.Shared.Inventory.Events;

namespace Content.Server.SS220.Holocigar;

public sealed class HolocigarSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HolocigarComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<HolocigarComponent, GotUnequippedEvent>(OnUnequipped);
    }

    private void OnUnequipped(Entity<HolocigarComponent> ent, ref GotUnequippedEvent args)
    {
        RemComp<IgnoreWieldComponent>(args.EquipTarget);
    }

    private void OnEquipped(Entity<HolocigarComponent> ent, ref GotEquippedEvent args)
    {
        EnsureComp<IgnoreWieldComponent>(args.EquipTarget);
    }


}
