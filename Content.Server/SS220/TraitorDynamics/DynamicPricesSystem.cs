using Content.Shared.SS220.TraitorDynamics;
using Robust.Shared.Prototypes;

namespace Content.Server.SS220.TraitorDynamics;

public sealed partial class DynamicPricesSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly SharedTraitorDynamicsSystem _dynamics = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TraitorRuleAddedEvent>(OnTraitorRuleAdded);
    }

    private void OnTraitorRuleAdded(TraitorRuleAddedEvent args)
    {
         var dynamic = _prototype.Index<DynamicPrototype>(_dynamics.GetRandomDynamic());
        //something...
    }
}
