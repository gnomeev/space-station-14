using Content.Server.Stealth;
using Content.Shared.SS220.Stealth.TemporalStealth;
using Content.Shared.StatusEffect;
using Content.Shared.Stealth.Components;
using Robust.Shared.Timing;
namespace Content.Server.SS220.Stealth.TemporalStealth;

public sealed class TemporalStealthSystem : EntitySystem
{
    [Dependency] private readonly StealthSystem _stealth = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly StatusEffectsSystem _status = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TemporalStealthComponent, ComponentInit>(OnCompInit);
    }

    private void OnCompInit(Entity<TemporalStealthComponent> ent, ref ComponentInit args)
    {
        ent.Comp.LastStealthTime = _gameTiming.CurTime + ent.Comp.StealthTime;

        if (HasComp<StealthComponent>(ent))
            ent.Comp.HasComp = true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<TemporalStealthComponent>();

        while (query.MoveNext(out var ent, out var temporal))
        {
            if (temporal.LastStealthTime > _gameTiming.CurTime)
            {

            }

            _stealth.SetEnabled(ent, false);

            if (!temporal.HasComp)
                RemCompDeferred<StealthComponent>(ent);

            RemCompDeferred<TemporalStealthComponent>(ent);
        }
    }

    public void SetTemporalStealth(EntityUid target, TemporalStealthComponent? comp = null)
    {
        if(!Resolve(target, ref comp, logMissing: true))
            return;
    }
}
