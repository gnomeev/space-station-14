// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Server.Popups;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.SS220.Weapons.Ranged;

public sealed partial class ChargeGunSystem : EntitySystem
{
    [Dependency] private readonly GunSystem _gun = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly PrototypeManager _proto = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChargeGunComponent, AttemptShootEvent>(OnShootAttempt);
        SubscribeLocalEvent<ChargeGunComponent, GunRefreshModifiersEvent>(OnRefreshModifiers);
    }

    private void OnRefreshModifiers(Entity<ChargeGunComponent> ent, ref GunRefreshModifiersEvent args)
    {
        if (ent.Comp.Modifiers is not { } modifiers)
            return;

        if (!TryComp<GunComponent>(ent.Owner, out var gunComponent))
            return;

        args.SoundGunshot = modifiers.SoundGunshot ?? gunComponent.SoundGunshot;
        args.AngleIncrease = modifiers.AngleIncrease ?? gunComponent.AngleIncrease;
        args.AngleDecay = modifiers.AngleDecay ?? gunComponent.AngleDecay;
        args.MaxAngle = modifiers.MaxAngle ?? gunComponent.MaxAngle;
        args.MinAngle = modifiers.MinAngle ?? gunComponent.MinAngle;
        args.ShotsPerBurst = modifiers.ShotsPerBurst ?? gunComponent.ShotsPerBurst;
        args.FireRate = modifiers.FireRate ?? gunComponent.FireRate;
        args.ProjectileSpeed = modifiers.ProjectileSpeed ?? gunComponent.ProjectileSpeed;
    }

    private void OnShootAttempt(Entity<ChargeGunComponent> ent, ref AttemptShootEvent args)
    {
        var curTime = _timing.CurTime;
        ent.Comp.Charging = true;
        ent.Comp.ChargeUntil = curTime + ent.Comp.ChargeTime;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<ChargeGunComponent, BasicEntityAmmoProviderComponent>();
        var curTime = _timing.CurTime;

        while (query.MoveNext(out var ent, out var chargeComp, out var ammoComp))
        {
            if (!TryComp<GunComponent>(ent, out var gunComponent))
                continue;

            if (chargeComp.Charging == false)
                continue;

            if (chargeComp.CurChargesCount >= chargeComp.MaxChargesCount)
                continue;

            if (ammoComp.Count <= 0)
                continue;

            if (chargeComp.ChargeUntil > curTime)
                continue;

            var newAmmo = ammoComp.Count - chargeComp.AmmoPerCharge;

            if (newAmmo < 0)
                continue;

            chargeComp.CurChargesCount++;

            _popup.PopupEntity($"{chargeComp.CurChargesCount}/{chargeComp.MaxChargesCount} Charges!", ent,
            Shared.Popups.PopupType.SmallCaution);

            UpdateGunModifiers(ent, chargeComp);
        }
    }

    private void UpdateGunModifiers(Entity<GunComponent?> ent, ChargeGunComponent chargeComp)
    {
        _gun.RefreshModifiers(ent);
        if (!TryComp<HitscanBatteryAmmoProviderComponent>(ent, out var batteryAmmo))
            return;

        var h = _proto.Index<HitscanPrototype>(batteryAmmo.Prototype).Damage;

        if (h != null)
            h += chargeComp.DamagePerCharge * chargeComp.CurChargesCount;
    }
}
