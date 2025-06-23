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
        SubscribeLocalEvent<TemporalStealthComponent, TemporalStealthAddedEvent>(OnStealthAdded);
        SubscribeLocalEvent<TemporalStealthComponent, ComponentShutdown>(OnCompShutdown);
    }

    private void OnCompShutdown(Entity<TemporalStealthComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp is { OriginalStealthEnabled: not null, OriginalVisibility: not null })
        {
            _stealth.SetEnabled(ent, ent.Comp.OriginalStealthEnabled.Value);
            _stealth.SetVisibility(ent, ent.Comp.OriginalVisibility.Value);
        }
        else
        {
            RemComp<StealthComponent>(ent);
        }
    }

    private void OnStealthAdded(Entity<TemporalStealthComponent> ent, ref TemporalStealthAddedEvent args)
    {
        ent.Comp.OriginalStealthEnabled = TryComp<StealthComponent>(ent, out var comp) ? comp.Enabled : null;

        if (ent.Comp.OriginalStealthEnabled.HasValue)
            ent.Comp.OriginalVisibility = _stealth.GetVisibility(ent, comp);

        var stealth = EnsureComp<StealthComponent>(ent);

        _stealth.SetEnabled(ent, true, stealth);
        _stealth.ModifyVisibility(ent, ent.Comp.Visibility, stealth);
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

    public void ActivateTemporalStealth(EntityUid target, float visibility, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            return;

        if (TryComp<TemporalStealthComponent>(target, out var has))
            has.StealthTime += duration;

        var temporal = EnsureComp<TemporalStealthComponent>(target);
        temporal.Visibility = visibility;
        temporal.StealthTime = duration;
        temporal.LastStealthTime = _gameTiming.CurTime + duration;
        Dirty(target, temporal);

        var ev = new TemporalStealthAddedEvent();
        RaiseLocalEvent(target, ev);
    }
}
