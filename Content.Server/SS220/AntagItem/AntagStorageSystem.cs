// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Server.Popups;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs;
using Content.Shared.SS220.SharedAntagItem;
using Robust.Shared.Containers;
using Content.Shared.SS220.CultYogg;
using Content.Shared.Item;

namespace Content.Server.SS220.AntagItem;

public sealed partial class AntagStorageSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
        SubscribeLocalEvent<AntagStorageComponent, ContainerIsRemovingAttemptEvent>(OnRemoveAttempt);
        SubscribeLocalEvent<AntagStorageComponent, ContainerIsInsertingAttemptEvent>(OnInsertAttempt);
        SubscribeLocalEvent<AntagStorageComponent, BeingUnequippedAttemptEvent>(OnUnequipAttempt);
        SubscribeLocalEvent<AntagStorageComponent, PickupVerbAttempt>(OnPickupVerbAttempt);
    }

    private void OnInsertAttempt(Entity<AntagStorageComponent> entity, ref ContainerIsInsertingAttemptEvent ev)
    {
        if (!IsValidAntagStorageUser(entity))
        {
            if (entity.Comp.ShouldDamageOnUseInteract)
                _damageable.TryChangeDamage(Transform(entity).ParentUid, entity.Comp.DamageOnInteract, true);

            _popup.PopupEntity(Loc.GetString(entity.Comp.InteractMessage), entity, Shared.Popups.PopupType.SmallCaution);
            ev.Cancel();
        }
    }

    private void OnRemoveAttempt(Entity<AntagStorageComponent> entity, ref ContainerIsRemovingAttemptEvent ev)
    {
        if (!IsValidAntagStorageUser(entity))
        {
            if (entity.Comp.ShouldDamageOnUseInteract)
                _damageable.TryChangeDamage(Transform(entity).ParentUid, entity.Comp.DamageOnInteract, true);

            _popup.PopupEntity(Loc.GetString(entity.Comp.InteractMessage), entity, Shared.Popups.PopupType.SmallCaution);
            ev.Cancel();
        }
    }

    private void OnPickupVerbAttempt(Entity<AntagStorageComponent> entity, ref PickupVerbAttempt ev)
    {
        if (!entity.Comp.Unequipble)
            return;

        if (_inventory.TryGetSlotEntity(ev.User, entity.Comp.Slot, out var _))
            ev.Cancel();
    }

    private void OnUnequipAttempt(Entity<AntagStorageComponent> entity, ref BeingUnequippedAttemptEvent ev)
    {
        if (!entity.Comp.Unequipble)
            return;

        _popup.PopupEntity(Loc.GetString(entity.Comp.UnquipMesssage), entity, ev.UnEquipTarget);
        ev.Cancel();
    }

    private void OnMobStateChanged(MobStateChangedEvent ev)
    {
        if (!_inventory.TryGetSlotEntity(ev.Target, "back", out var backpack) || !TryComp<AntagStorageComponent>(backpack, out var comp))
            return;

        if (ev.NewMobState == MobState.Dead || ev.OldMobState == MobState.Dead) // it's just works
        {
            _inventory.TryUnequip(ev.Target, comp.Slot, true, true, false);
        }
    }

    public bool IsValidAntagStorageUser(EntityUid storage)
    {
        if (!_inventory.TryGetContainingSlot(storage, out var _))
            return false;

        if (!HasComp<CultYoggComponent>(Transform(storage).ParentUid)) // ToDo: shoud be prototype check?
            return false;

        return true;
    }

}
