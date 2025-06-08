using Content.Server.GameTicking.Rules.Components;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Content.Shared.SS220.TraitorDynamics;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.SS220.TraitorDynamics;

public sealed partial class DynamicPricesSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    [ValidatePrototypeId<WeightedRandomPrototype>]
    private const string WeightsProto = "WeightedDynamicsList";
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TraitorRuleAddedEvent>(OnRuleAdded);
    }

    private void OnRuleAdded(TraitorRuleAddedEvent args)
    {
        var dynamic = _prototype.Index<DynamicPrototype>(GetRandomDynamic());
        // something..

    }

    public string GetRandomDynamic()
    {
       var validWeight = _prototype.Index<WeightedRandomPrototype>(WeightsProto);
       return validWeight.Pick(_random);
    }
}
