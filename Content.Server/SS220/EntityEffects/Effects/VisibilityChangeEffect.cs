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
    public TimeSpan Duration = TimeSpan.FromSeconds(2);

    public override void Effect(EntityEffectBaseArgs args)
    {
        var stealthSystem = args.EntityManager.System<TemporalStealthSystem>();

        stealthSystem.ActivateTemporalStealth(args.TargetEntity, VisibilityChange, Duration);
    }
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "ПОШЕЛ НАХУЙ";
    }

}
