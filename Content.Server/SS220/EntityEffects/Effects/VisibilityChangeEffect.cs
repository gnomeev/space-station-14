using Content.Shared.EntityEffects;
using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.SS220.EntityEffects.Effects;

public sealed partial class VisibilityChangeEffect : EntityEffect
{
    [DataField]
    public float VisibilityChange = 0.5f;

    public override void Effect(EntityEffectBaseArgs args)
    {
        var stealthSystem = args.EntityManager.System<SharedStealthSystem>();

        if (args.EntityManager.TryGetComponent<StealthComponent>(args.TargetEntity, out var stealth))
        {
            var effectVisibility = stealthSystem.GetVisibility(args.TargetEntity, stealth) + VisibilityChange;
            stealthSystem.SetVisibility(args.TargetEntity, effectVisibility, stealth);
        }
    }
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "ПОШЕЛ НАХУЙ";
    }

}
