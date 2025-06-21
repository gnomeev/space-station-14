using Content.Shared.GameTicking.Components;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.SS220.TraitorDynamics;

/// <summary>
/// This handles...
/// </summary>
public abstract class SharedTraitorDynamicsSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    [ValidatePrototypeId<WeightedRandomPrototype>]
    private const string WeightsProto = "WeightedDynamicsList";

    protected ProtoId<DynamicPrototype>? CurrentDynamic = null; // prob we should have nullspace entity with comp

    public string GetRandomDynamic()
    {
        var validWeight = _prototype.Index<WeightedRandomPrototype>(WeightsProto);
        return validWeight.Pick(_random);
    }

    /// <summary>
    /// Tries to find the type of dynamic while in Traitor game rule
    /// </summary>
    /// <returns>installed dynamic</returns>
    public ProtoId<DynamicPrototype>? GetCurrentDynamic()
    {
        return CurrentDynamic;
    }

    public sealed class DynamicAddedEvent : EntityEventArgs
    {
        public ProtoId<DynamicPrototype> Dynamic;

        public DynamicAddedEvent(ProtoId<DynamicPrototype> dynamic)
        {
            Dynamic = dynamic;
        }
    }

    public sealed class DynamicSetAttempt : CancellableEntityEventArgs
    {
        public ProtoId<DynamicPrototype> Dynamic;

        public DynamicSetAttempt(ProtoId<DynamicPrototype> dynamic)
        {
            Dynamic = dynamic;
        }
    }
}
