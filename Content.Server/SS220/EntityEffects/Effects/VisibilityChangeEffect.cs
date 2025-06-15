using Content.Server.SS220.Stealth.TemporalStealth;
using Content.Shared.EntityEffects;
using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.SS220.EntityEffects.Effects;

public sealed partial class VisibilityChangeEffect : EntityEffect
{
    [DataField]
    public float VisibilityChange = 0.5f;

    [DataField]
    public TimeSpan VisibilityDuration = TimeSpan.FromSeconds(1);

    public override void Effect(EntityEffectBaseArgs args)
    {
        var stealthSystem = args.EntityManager.System<SharedStealthSystem>();
        var temporalSystem = args.EntityManager.System<TemporalStealthSystem>();

        if (args.EntityManager.TryGetComponent<StealthComponent>(args.TargetEntity, out var stealth))
        {
            var effectVisibility = stealthSystem.GetVisibility(args.TargetEntity, stealth) + VisibilityChange;
            temporalSystem.ActivateTemporalStealth(args.TargetEntity, effectVisibility, VisibilityDuration);
        }
    }
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "ПОШЕЛ НАХУЙ";
    }

}
