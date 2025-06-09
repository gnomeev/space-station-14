using Content.Server.Antag;
using Content.Server.Antag.Components;
using Content.Shared.SS220.TraitorDynamics;
using Robust.Shared.Prototypes;

namespace Content.Server.SS220.TraitorDynamics;

/// <summary>
/// This handles...
/// </summary>
public sealed class TraitorDynamicsSystem : EntitySystem
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly SharedTraitorDynamicsSystem _dynamics = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private const string GameRule = "Traitor";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TraitorRuleAddedEvent>(OnTraitorRuleAdded);
    }

    private void OnTraitorRuleAdded(TraitorRuleAddedEvent ev)
    {
        var dynamic = _prototype.Index<DynamicPrototype>(_dynamics.GetRandomDynamic());

        var query = EntityQueryEnumerator<AntagSelectionComponent>(); //mb additionally check that AntagSelectionComponent is Traitor
        while (query.MoveNext(out var _, out var comp))
        {
            _antag.SetMaxAntags(comp, dynamic.LimitAntag);
        }
    }
}
