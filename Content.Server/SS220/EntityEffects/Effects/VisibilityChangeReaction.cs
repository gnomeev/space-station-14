using Content.Server.Stealth;
using Content.Shared.EntityEffects;
using Content.Shared.Stealth.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.SS220.EntityEffects.Effects;

public sealed partial class VisibilityChangeReaction : EntityEffect
{
    [DataField]
    public float RangePerUnit = 0.3f;

    [DataField]
    public float MaxRange = 20f;

    [DataField]
    public float VisibilityChange = 0.7f;

    public override void Effect(EntityEffectBaseArgs args)
    {
        var lookupSystem = args.EntityManager.System<EntityLookupSystem>();
        var stealthSystem = args.EntityManager.System<StealthSystem>();
        var transformSystem = args.EntityManager.System<TransformSystem>();

        var transform = args.EntityManager.GetComponent<TransformComponent>(args.TargetEntity);
        var range = RangePerUnit;

        if (args is EntityEffectReagentArgs reagentArgs)
        {
            range = MathF.Min((float) (reagentArgs.Quantity * RangePerUnit), MaxRange);
        }

        foreach (var ent in lookupSystem.GetEntitiesInRange(transformSystem.GetMapCoordinates(args.TargetEntity, xform: transform), range))
        {
            if (!args.EntityManager.HasComponent<StealthComponent>(ent))
                args.EntityManager.EnsureComponent<StealthComponent>(ent);

            if (args.EntityManager.TryGetComponent<StealthComponent>(ent, out var stealth))
            {
                var effectVisibility = stealthSystem.GetVisibility(ent, stealth) + VisibilityChange;
                stealthSystem.SetVisibility(ent, effectVisibility, stealth);
            }
        }
    }
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "ПОШЕЛ В ПИЗДУ";
    }

}
