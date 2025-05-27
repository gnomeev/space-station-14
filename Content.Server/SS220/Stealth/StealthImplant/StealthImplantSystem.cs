
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
            EnsureComp<StealthComponent>(args.Performer);

        args.Handled = true;
        _stealth.SetEnabled(args.Performer, true);
        _stealth.SetVisibility(args.Performer, ent.Comp.Visibility);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<StealthImplantComponent, SubdermalImplantComponent>();

        while (query.MoveNext(out _, out var stealthImplant, out var implant))
        {
            if (implant.user is not {} user)
                continue;

            if (!TryComp<StealthComponent>(user, out var stealthComponent) || !stealthComponent.Enabled)
                continue;

            var curTime = _timing.CurTime;

            if (stealthImplant.LastStealthTime <= TimeSpan.Zero)
                stealthImplant.LastStealthTime = curTime + stealthImplant.StealthTime;

            if (curTime > stealthImplant.LastStealthTime)
            {
                _stealth.SetEnabled(user, false, stealthComponent);
                stealthImplant.LastStealthTime = TimeSpan.Zero;
            }
        }
    }
}
