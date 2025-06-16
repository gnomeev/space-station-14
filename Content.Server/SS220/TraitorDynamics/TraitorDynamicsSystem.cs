using Content.Server.Antag;
using Content.Server.Antag.Components;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.StoreDiscount.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking.Components;
using Content.Shared.SS220.TraitorDynamics;
using Content.Shared.SS220.TraitorDynamics.Components;
using Content.Shared.Store;
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
        SubscribeLocalEvent<TraitorSleeperAddedEvent>(OnTraitorSleeperAdded);
        SubscribeLocalEvent<StoreInitializedEvent>(OnStoreInit);
    }

    private void OnStoreInit(ref StoreInitializedEvent ev)
    {
        var query = EntityQueryEnumerator<TraitorDynamicsComponent>();

        while (query.MoveNext(out var comp))
        {
            if (comp.CurrentDynamic == null)
                continue;

            foreach (var listing in ev.Listings)
            {
                if (!listing.DynamicsPrices.TryGetValue(comp.CurrentDynamic.Value, out var price))
                    return;

                listing.AddCostModifier(comp.CurrentDynamic, price);
            }
        }
    }

    private void OnDynamicAdded(DynamicAddedEvent ev)
    {
        if (!TryComp<AntagSelectionComponent>(ev.Entity, out var selectionComp))
            return;

        var dynamic = _prototype.Index(ev.Dynamic);
        _antag.SetMaxAntags(selectionComp, dynamic.LimitAntag);
    }

    private void OnTraitorSleeperAdded(TraitorSleeperAddedEvent ev)
    {
        var dynamic = GetCurrentDynamic();

        if (!_prototype.TryIndex(dynamic, out var dynamicProto))
            return;

        if (!TryComp<AntagSelectionComponent>(ev.RuleEnt, out var selectionComp))
            return;

        _antag.SetMaxAntags(selectionComp, dynamicProto.LimitSleeperAntag);
    }

    private void OnRoundEndAppend(RoundEndTextAppendEvent ev)
    {
        var dynamic = GetCurrentDynamic();

        if (!_prototype.TryIndex(dynamic, out var dynamicProto))
            return;

        ev.AddLine($"{Loc.GetString("dynamic-show-end-round")} {Loc.GetString(dynamicProto.Name)}");
    }

    private void OnTraitorRuleAdded(TraitorRuleAddedEvent ev)
    {
        if (!TryComp<TraitorRuleComponent>(ev.RuleEnt, out var _))
            return;

        if (!TryComp<TraitorDynamicsComponent>(ev.RuleEnt, out var dynamicComp))
            return;

        var dynamicStr = GetRandomDynamic();
        SetDynamic(ev.RuleEnt, dynamicStr, dynamicComp);
    }
}
