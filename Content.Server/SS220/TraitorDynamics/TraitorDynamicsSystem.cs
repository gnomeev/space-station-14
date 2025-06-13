using Content.Server.Antag;
using Content.Server.Antag.Components;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Components;
using Content.Shared.SS220.TraitorDynamics;
using Robust.Shared.Prototypes;

namespace Content.Server.SS220.TraitorDynamics;

/// <summary>
/// This handles...
/// </summary>
public sealed class TraitorDynamicsSystem : SharedTraitorDynamicsSystem
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TraitorRuleAddedEvent>(OnTraitorRuleAdded);
        SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEndAppend);
        SubscribeLocalEvent<DynamicAddedEvent>(OnDynamicAdded);
    }

    private void OnDynamicAdded(DynamicAddedEvent ev)
    {
        if (!TryComp<AntagSelectionComponent>(ev.Entity, out var selectionComp))
            return;

        var dynamic = _prototype.Index(ev.Dynamic);
        _antag.SetMaxAntags(selectionComp, dynamic.LimitAntag);
    }

    private void OnRoundEndAppend(RoundEndTextAppendEvent ev)
    {
        var query = EntityQueryEnumerator<TraitorDynamicsComponent>();

        while (query.MoveNext(out _, out var dynamicComp))
        {
            ev.AddLine($"На смене был динамик агентов: {dynamicComp.CurrentDynamic}"); // TODO: add loc
        }
    }

    private void OnTraitorRuleAdded(TraitorRuleAddedEvent ev)
    {
        if (!TryComp<TraitorRuleComponent>(ev.RuleEnt, out var traitor))
            return;

        if (!TryComp<TraitorDynamicsComponent>(ev.RuleEnt, out var dynamicComp))
            return;

        var dynamicStr = GetRandomDynamic();
        SetDynamic(ev.RuleEnt, dynamicStr, dynamicComp);
    }
}
