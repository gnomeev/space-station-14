namespace Content.Shared.SS220.Akimbo;

using Content.Shared.Actions;
using Content.Shared.CombatMode;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.SS220.Akimbo;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;

public sealed class AkimboSystem : EntitySystem
{
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedCombatModeSystem _combatMode = default!;
    public override void Initialize()
    {
        base.Initialize();
        // SubscribeLocalEvent<AkimboComponent, GunShotEvent>(OnShot);
        SubscribeLocalEvent<AkimboComponent, GotEquippedHandEvent>(OnGotEquipped);
        SubscribeLocalEvent<AkimboComponent, GotUnequippedHandEvent>(OnGotUnequipped);
        SubscribeLocalEvent<AkimboOwnerComponent, ComponentInit>(OnCompInit);
        SubscribeLocalEvent<AkimboOwnerComponent, ComponentShutdown>(OnCompShutdown);
        SubscribeLocalEvent<AkimboOwnerComponent, AkimboToggleActionEvent>(OnAction);
        SubscribeAllEvent<RequestShootEvent>(OnRequestShoot);
    }

    private void OnRequestShoot(RequestShootEvent ev, EntitySessionEventArgs args)
    {
        var user = args.SenderSession.AttachedEntity;
        var toCoords = GetCoordinates(ev.Coordinates);
        var target = GetEntity(ev.Target);

        if (user == null || !_combatMode.IsInCombatMode(user))
            return;

        if (!TryComp<AkimboOwnerComponent>(user, out var akimboOwner))
            return;

        if (!akimboOwner.IsOn)
            return;

        var hands = _hands.EnumerateHeld(user.Value);

        foreach (var held in hands)
        {
            if (!HasComp<AkimboComponent>(held))
                continue;

            if (!TryComp<GunComponent>(held, out var gunComponent))
                continue;

            if (gunComponent != null)
            {
                _gun.AttemptShoot(user.Value, (held, gunComponent), toCoords, target);
                Dirty(held, gunComponent);
            }
        }
    }

    private void OnCompShutdown(Entity<AkimboOwnerComponent> ent, ref ComponentShutdown args)
    {
        _actions.RemoveAction(ent.Owner, ent.Comp.AkimboToggleActionEntity);
    }

    private void OnCompInit(Entity<AkimboOwnerComponent> ent, ref ComponentInit args)
    {
        _actions.AddAction(ent.Owner, ref ent.Comp.AkimboToggleActionEntity, ent.Comp.AkimboToggleAction);
    }

    private void OnGotUnequipped(Entity<AkimboComponent> ent, ref GotUnequippedHandEvent args)
    {
        var hands = _hands.EnumerateHeld(args.User);

        foreach (var held in hands)
        {
            if (held.Id == ent.Owner.Id)
                continue;

            if (HasComp<AkimboComponent>(held))
                return;
        }

        RemComp<AkimboOwnerComponent>(args.User);
    }

    private void OnGotEquipped(Entity<AkimboComponent> ent, ref GotEquippedHandEvent args)
    {
        EnsureComp<AkimboOwnerComponent>(args.User);
    }

    private void OnAction(Entity<AkimboOwnerComponent> ent, ref AkimboToggleActionEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;
        ent.Comp.IsOn = !ent.Comp.IsOn;

        if (ent.Comp.AkimboToggleActionEntity != null)
            _actions.SetToggled(ent.Comp.AkimboToggleActionEntity, ent.Comp.IsOn);
    }
}
