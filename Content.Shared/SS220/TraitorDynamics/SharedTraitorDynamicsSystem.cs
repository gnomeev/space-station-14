using Content.Shared.GameTicking.Components;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.SS220.TraitorDynamics;

/// <summary>
/// This handles...
/// </summary>
public sealed class SharedTraitorDynamicsSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    [ValidatePrototypeId<WeightedRandomPrototype>]
    private const string WeightsProto = "WeightedDynamicsList";
    private const string GameRule = "Traitor";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GameRuleAddedEvent>(OnRuleAdded);
    }

    private void OnRuleAdded(ref GameRuleAddedEvent args)
    {
        if (args.RuleId!= GameRule)
            return;

        var ev = new TraitorRuleAddedEvent();
        RaiseLocalEvent(ev);
    }

    public string GetRandomDynamic()
    {
        var validWeight = _prototype.Index<WeightedRandomPrototype>(WeightsProto);
        return validWeight.Pick(_random);
    }
}
public sealed class TraitorRuleAddedEvent() : EntityEventArgs
{
}
