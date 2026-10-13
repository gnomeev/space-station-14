namespace Content.Server.SS220.Akimbo;

using Content.Server.Actions;
using Content.Server.Hands.Systems;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.SS220.Akimbo;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;

public sealed class AkimboSystem : EntitySystem
{
    [Dependency] private readonly HandsSystem _hands = default!;
    [Dependency] private readonly GunSystem _gun = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AkimboComponent, GunShotEvent>(OnShot);
        SubscribeLocalEvent<AkimboComponent, GotEquippedHandEvent>(OnGotEquipped);
        SubscribeLocalEvent<AkimboComponent, GotUnequippedHandEvent>(OnGotUnequipped);
        SubscribeLocalEvent<AkimboOwnerComponent, ComponentInit>(OnCompInit);
        SubscribeLocalEvent<AkimboOwnerComponent, ComponentShutdown>(OnCompShutdown);
        SubscribeLocalEvent<AkimboOwnerComponent, AkimboToggleActionEvent>(OnAction);
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

    private void OnShot(Entity<AkimboComponent> ent, ref GunShotEvent args)
    {
        if (!TryComp<AkimboOwnerComponent>(args.User, out var akimboOwner))
            return;

        if (!akimboOwner.IsOn)
            return;

        if (!TryComp<GunComponent>(ent, out var gunComponent))
            return;

        var hands = _hands.EnumerateHeld(args.User);

        foreach (var held in hands)
        {
            if (held.Id == ent.Owner.Id)
                continue;

            if (!HasComp<AkimboComponent>(held))
                continue;

            if (!TryComp<GunComponent>(held, out var secondGunComponent))
                continue;

            if (secondGunComponent != null && args.toCoords != null)
            {
                _gun.AttemptShoot(args.User, (held, secondGunComponent), args.toCoords.Value, gunComponent.Target);
                Dirty(held, secondGunComponent);
            }
        }
    }
}
