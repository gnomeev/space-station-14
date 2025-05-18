
using Content.Server.Stealth;
using Content.Shared.Implants.Components;
using Content.Shared.SS220.Stealth.StealthImplant;
using Content.Shared.Stealth.Components;
using Robust.Shared.Timing;

namespace Content.Server.SS220.Stealth.StealthImplant;

public sealed class StealthImplantSystem : EntitySystem
{
    [Dependency] private readonly StealthSystem _stealth = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StealthImplantComponent, UseStealthImplantEvent>(OnImplantUse);
    }

    private void OnImplantUse(Entity<StealthImplantComponent> ent, ref UseStealthImplantEvent args)
    {
        if (!HasComp<StealthComponent>(args.Performer))
        {
            EnsureComp<StealthComponent>(args.Performer);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<StealthImplantComponent, SubdermalImplantComponent>();

        while (query.MoveNext(out _, out var stealth, out var implant))
        {
            var user = implant.user;
            if (user is not null && TryComp<StealthComponent>(user, out var stealthComponent) && stealthComponent.Enabled)
            {
                var time = _timing.CurTime + stealth.StealthTime;

                if (_timing.CurTime > time)
                {
                    _stealth.SetEnabled(user.Value, !stealthComponent.Enabled, stealthComponent);
                }
            }
        }
    }
}
