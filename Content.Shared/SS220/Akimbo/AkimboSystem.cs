namespace Content.Shared.SS220.Akimbo;

using Content.Shared.CombatMode;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;

public sealed class AkimboSystem : EntitySystem
{
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;
    [Dependency] private readonly SharedCombatModeSystem _combatMode = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeAllEvent<RequestShootEvent>(OnRequestShoot);
        SubscribeLocalEvent<AkimboComponent, GetVerbsEvent<Verb>>(OnGetVerb);
    }

    private void OnGetVerb(Entity<AkimboComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var verb = new Verb
        {
            Text = Loc.GetString("Переключить стрельбу с двух рук"),
            Act = () =>
            {
                ent.Comp.IsOn = !ent.Comp.IsOn;
            },
        };

        args.Verbs.Add(verb);
    }

    private void OnRequestShoot(RequestShootEvent ev, EntitySessionEventArgs args)
    {
        var user = args.SenderSession.AttachedEntity;

        if (user == null || !_combatMode.IsInCombatMode(user))
            return;

        var hands = _hands.EnumerateHeld(user.Value);
        List<Entity<GunComponent>> gunList = new();

        foreach (var held in hands)
        {
            if (!TryComp<AkimboComponent>(held, out var akimboComponent))
                continue;

            if (!TryComp<GunComponent>(held, out var gunComponent))
                continue;

            if (gunComponent != null && akimboComponent.IsOn)
                gunList.Add((held, gunComponent));
        }

        foreach (var gun in gunList)
        {
            _gun.AttemptShoot(user.Value, gun, GetCoordinates(ev.Coordinates), GetEntity(ev.Target));
        }
    }
}
