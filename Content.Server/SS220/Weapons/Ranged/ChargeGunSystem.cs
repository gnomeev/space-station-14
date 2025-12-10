// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Input;

namespace Content.Server.SS220.Weapons.Ranged;

public sealed partial class ChargeGunSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChargeGunComponent, AttemptShootEvent>(OnShootAttempt);
    }

    private void OnShootAttempt(Entity<ChargeGunComponent> ent, ref AttemptShootEvent args)
    {
        ent.Comp.Charging = true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<ChargeGunComponent>();

        while (query.MoveNext(out var ent, out var comp))
        {
            if (comp.Charging == false)
                return;


        }
    }
}
