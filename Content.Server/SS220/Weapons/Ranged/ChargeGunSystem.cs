// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Server.Popups;
using Content.Server.Hands.Systems;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.CombatMode;
using Content.Shared.SS220.Input;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Input.Binding;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Content.Shared.Hands.Components;

namespace Content.Server.SS220.Weapons.Ranged;

public sealed partial class ChargeGunSystem : EntitySystem
{
    [Dependency] private readonly GunSystem _gun = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly HandsSystem _hands = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChargeGunComponent, GunRefreshModifiersEvent>(OnRefreshModifiers);

        CommandBinds.Builder
        .Bind(KeyFunctions220.ChargeGun, InputCmdHandler.FromDelegate(HandleChargeGun))
        .Register<ChargeGunSystem>();
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

        args.SoundGunshot = modifiers.SoundGunshot ?? gunComponent.SoundGunshot;
        args.AngleIncrease = modifiers.AngleIncrease ?? gunComponent.AngleIncrease;
        args.AngleDecay = modifiers.AngleDecay ?? gunComponent.AngleDecay;
        args.MaxAngle = modifiers.MaxAngle ?? gunComponent.MaxAngle;
        args.MinAngle = modifiers.MinAngle ?? gunComponent.MinAngle;
        args.ShotsPerBurst = modifiers.ShotsPerBurst ?? gunComponent.ShotsPerBurst;
        args.FireRate = modifiers.FireRate ?? gunComponent.FireRate;
        args.ProjectileSpeed = modifiers.ProjectileSpeed ?? gunComponent.ProjectileSpeed;
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

            if (!chargeComp.Charging)
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

    private void ChargeGun(ChargeGunComponent chargeComp)
    {
        var curTime = _timing.CurTime;
        chargeComp.Charging = true;
        chargeComp.ChargeUntil = curTime + chargeComp.ChargeTime;
    }

    public bool TryChargeGun(ChargeGunComponent chargeComp)
    {
        return true;
    }
}
