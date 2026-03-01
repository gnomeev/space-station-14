// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Server.Popups;
using Content.Server.Hands.Systems;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.CombatMode;
using Content.Shared.SS220.Input;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Input.Binding;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Content.Server.SS220.Weapons.Ranged.Events;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Systems;

namespace Content.Server.SS220.Weapons.Ranged;

public sealed partial class ChargeGunSystem : EntitySystem
{
    [Dependency] private readonly GunSystem _gun = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly HandsSystem _hands = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChargeGunComponent, GunRefreshModifiersEvent>(OnRefreshModifiers);
        SubscribeLocalEvent<ChargeGunComponent, SelfBeforeGunShotEvent>(OnBeforeShot);
        SubscribeLocalEvent<ChargeGunComponent, TakeAmmoEvent>(OnTakeAmmo);
        SubscribeLocalEvent<ChargeGunComponent, BeforeHitscanDamageEvent>(BeforeHitscanDamage);
        SubscribeLocalEvent<ChargeGunComponent, GunShotEvent>(OnAfterShot);

        CommandBinds.Builder
        .Bind(KeyFunctions220.ChargeGun, InputCmdHandler.FromDelegate(HandleChargeGun))
        .Register<ChargeGunSystem>();
    }

    private void OnAfterShot(Entity<ChargeGunComponent> ent, ref GunShotEvent args)
    {
        ent.Comp.CurChargesCount -= ent.Comp.ChargesPerShot;
    }

    private void BeforeHitscanDamage(Entity<ChargeGunComponent> ent, ref BeforeHitscanDamageEvent args)
    {
        if (args.Damage != null)
            args.Damage += ent.Comp.DamagePerCharge * ent.Comp.CurChargesCount;
    }

    private void OnTakeAmmo(Entity<ChargeGunComponent> ent, ref TakeAmmoEvent args)
    {
        if (TryComp<HitscanBatteryAmmoProviderComponent>(ent, out var batteryAmmoProvider))
        {
            var newCapacity = batteryAmmoProvider.Capacity - ent.Comp.AmmoPerCharge * ent.Comp.CurChargesCount;

            if (newCapacity <= 0)
                return;

            batteryAmmoProvider.Capacity = newCapacity;
        }
    }

    private void OnBeforeShot(Entity<ChargeGunComponent> ent, ref SelfBeforeGunShotEvent args)
    {
        foreach (var ammo in args.Ammo)
        {
            if (TryComp<ProjectileComponent>(ammo.Entity, out var projectile))
                projectile.Damage += ent.Comp.DamagePerCharge * ent.Comp.CurChargesCount;
        }
    }

    private void HandleChargeGun(ICommonSession? session)
    {
        if (session?.AttachedEntity is not { Valid: true } player)
            return;

        if (!TryComp<CombatModeComponent>(player, out var combatMode))
            return;

        if (!combatMode.IsInCombatMode)
            return;

        var heldEnt = _hands.GetActiveItem(player);

        if (heldEnt is null)
            return;

        if (TryComp<ChargeGunComponent>(heldEnt, out var chargeComp))
            ChargeGun(chargeComp);
    }
    private void OnRefreshModifiers(Entity<ChargeGunComponent> ent, ref GunRefreshModifiersEvent args)
    {
        if (ent.Comp.Modifiers is not { } modifiers)
            return;

        if (!TryComp<GunComponent>(ent.Owner, out var gunComponent))
            return;

        if (ent.Comp.CurChargesCount > 0)
        {
            args.SoundGunshot = modifiers.SoundGunshot ?? gunComponent.SoundGunshot;
            args.AngleIncrease = modifiers.AngleIncrease * ent.Comp.CurChargesCount ?? gunComponent.AngleIncrease;
            args.AngleDecay = modifiers.AngleDecay * ent.Comp.CurChargesCount ?? gunComponent.AngleDecay;
            args.MaxAngle = modifiers.MaxAngle * ent.Comp.CurChargesCount ?? gunComponent.MaxAngle;
            args.MinAngle = modifiers.MinAngle * ent.Comp.CurChargesCount ?? gunComponent.MinAngle;
            args.ShotsPerBurst = modifiers.ShotsPerBurst * ent.Comp.CurChargesCount ?? gunComponent.ShotsPerBurst;
            args.FireRate = modifiers.FireRate * ent.Comp.CurChargesCount ?? gunComponent.FireRate;
            args.ProjectileSpeed = modifiers.ProjectileSpeed * ent.Comp.CurChargesCount ?? gunComponent.ProjectileSpeed;
            return;
        }

        args.SoundGunshot = gunComponent.SoundGunshot;
        args.AngleIncrease = gunComponent.AngleIncrease;
        args.AngleDecay = gunComponent.AngleDecay;
        args.MaxAngle = gunComponent.MaxAngle;
        args.MinAngle = gunComponent.MinAngle;
        args.ShotsPerBurst = gunComponent.ShotsPerBurst;
        args.FireRate = gunComponent.FireRate;
        args.ProjectileSpeed = gunComponent.ProjectileSpeed;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<ChargeGunComponent>();
        var curTime = _timing.CurTime;

        while (query.MoveNext(out var ent, out var chargeComp))
        {
            if (!TryComp<GunComponent>(ent, out var gunComponent))
                continue;

            if (!chargeComp.Charging)
                continue;

            if (chargeComp.CurChargesCount >= chargeComp.MaxChargesCount)
            {
                chargeComp.Charging = false;
                UpdateGunModifiers(ent, chargeComp);
                continue;
            }

            if (chargeComp.ChargeUntil > curTime)
                continue;

            chargeComp.CurChargesCount++;
            chargeComp.ChargeUntil += chargeComp.ChargeTime;

            _popup.PopupEntity($"{chargeComp.CurChargesCount}/{chargeComp.MaxChargesCount} Charges!", ent,
            Shared.Popups.PopupType.SmallCaution);

            UpdateGunModifiers(ent, chargeComp);
        }
    }

    private void UpdateGunModifiers(Entity<GunComponent?> ent, ChargeGunComponent chargeComp)
    {
        _gun.RefreshModifiers(ent);
    }

    private void ChargeGun(ChargeGunComponent chargeComp)
    {
        var curTime = _timing.CurTime;
        chargeComp.Charging = true;
        chargeComp.ChargeUntil = curTime + chargeComp.ChargeTime;
    }

    public void ChangeChargingState(ChargeGunComponent chargeComp)
    {
        chargeComp.Charging = !chargeComp.Charging;
    }
}
