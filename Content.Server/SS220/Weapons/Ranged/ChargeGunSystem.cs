// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Timing;

namespace Content.Server.SS220.Weapons.Ranged;

public sealed partial class ChargeGunSystem : EntitySystem
{
    [Dependency] private readonly GunSystem _gun = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChargeGunComponent, AttemptShootEvent>(OnShootAttempt);
        SubscribeLocalEvent<ChargeGunComponent, GunRefreshModifiersEvent>(OnRefreshModifiers);
    }

    private void OnRefreshModifiers(Entity<ChargeGunComponent> ent, ref GunRefreshModifiersEvent args)
    {

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
            _gun.RefreshModifiers(ent);
        }
    }
}
