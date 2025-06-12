using Content.Server.Stealth;
using Content.Shared.SS220.Stealth.TemporalStealth;
using Content.Shared.Stealth.Components;
using Robust.Shared.Timing;
namespace Content.Server.SS220.Stealth.TemporalStealth;

public sealed class TemporalStealthSystem : EntitySystem
{
    [Dependency] private readonly StealthSystem _stealth = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TemporalStealthComponent, ComponentStartup>(OnCompStartup);
        SubscribeLocalEvent<TemporalStealthComponent, ComponentShutdown>(OnCompShutdown);
    }

    private void OnCompShutdown(Entity<TemporalStealthComponent> ent, ref ComponentShutdown args)
    {
        if (!ent.Comp.HasComp)
        {
            RemCompDeferred<StealthComponent>(ent);
            return;
        }

        _stealth.SetEnabled(ent, false);
    }

    private void OnCompStartup(Entity<TemporalStealthComponent> ent, ref ComponentStartup args)
    {
        ent.Comp.LastStealthTime = _gameTiming.CurTime + ent.Comp.StealthTime;
        ent.Comp.HasComp = HasComp<StealthComponent>(ent);

        if (!ent.Comp.HasComp)
            EnsureComp<StealthComponent>(ent);

        _stealth.SetEnabled(ent, true);
        _stealth.SetVisibility(ent, ent.Comp.Visibility);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<TemporalStealthComponent>();
        var time = _gameTiming.CurTime;

        while (query.MoveNext(out var ent, out var temporal))
        {
            if (temporal.LastStealthTime < time)
                RemCompDeferred<TemporalStealthComponent>(ent);
        }
    }

    public void ActivateTemporalStealth(EntityUid uid, float visibility, TimeSpan duration)
    {
        var temporal = EnsureComp<TemporalStealthComponent>(uid);
        temporal.Visibility = visibility;
        temporal.StealthTime = duration;
        temporal.LastStealthTime = _gameTiming.CurTime + duration;

        Dirty(uid, temporal);
    }
}
