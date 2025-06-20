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
    [Dependency] private readonly StoreDiscountSystem _discount = default!;

    [ValidatePrototypeId<StoreCategoryPrototype>]
    private const string DiscountedStoreCategoryPrototypeKey = "DiscountedItems";
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEndAppend);
        SubscribeLocalEvent<DynamicAddedEvent>(OnDynamicAdded);
        SubscribeLocalEvent<StoreInitializedEvent>(OnStoreInit);
        SubscribeLocalEvent<GameRuleAddedEvent>(OnGameRuleAdded);
    }

    private void OnGameRuleAdded(ref GameRuleAddedEvent ev)
    {
        if (TryComp<TraitorDynamicsComponent>(ev.RuleEntity, out var comp))
            SetRandomDynamic(ev.RuleEntity, comp);
    }

    private void OnStoreInit(ref StoreInitializedEvent ev)
    {
        var query = EntityQueryEnumerator<TraitorDynamicsComponent>();

        while (query.MoveNext(out var traitorComponent))
        {
            if (traitorComponent.CurrentDynamic == null)
                continue;

            ApplyDynamicPrice(ev, traitorComponent.CurrentDynamic.Value);
        }
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
        var dynamic = GetCurrentDynamic();

        if (!_prototype.TryIndex(dynamic, out var dynamicProto))
            return;

        ev.AddLine($"{Loc.GetString("dynamic-show-end-round")} {Loc.GetString(dynamicProto.Name)}");
    }

    private void ApplyDynamicPrice(StoreInitializedEvent ev,  ProtoId<DynamicPrototype> currentDynamic)
    {
        var itemDiscounts = _discount.GetItemsDiscount(ev.Store, ev.Listings);

        foreach (var listing in ev.Listings)
        {
            if (!listing.DynamicsPrices.TryGetValue(currentDynamic, out var dynamicPrice))
                continue;

            listing.RemoveCostModifier(DiscountedStoreCategoryPrototypeKey);
            listing.SetNewCost(dynamicPrice);

            var finalPrice = ApplyDiscountsToPrice(dynamicPrice, listing, itemDiscounts);
            listing.SetExactPrice(DiscountedStoreCategoryPrototypeKey, finalPrice);
        }
    }

    private Dictionary<ProtoId<CurrencyPrototype>,FixedPoint2> ApplyDiscountsToPrice(
        Dictionary<ProtoId<CurrencyPrototype>,FixedPoint2> basePrice,
        ListingDataWithCostModifiers listing,
        Dictionary<string,Dictionary<ProtoId<CurrencyPrototype>,FixedPoint2>> itemDiscounts)
    {
        if (!itemDiscounts.TryGetValue(listing.ID, out var currencyDiscounts))
            return basePrice;

        var finalPrice = new Dictionary<ProtoId<CurrencyPrototype>,FixedPoint2>(basePrice);

        foreach (var (currency, discountPercent) in currencyDiscounts)
        {
            if (!listing.OriginalCost.ContainsKey(currency))
                continue;

            if (!finalPrice.TryGetValue(currency, out var currentPrice))
                continue;

            var discountMultiplier = FixedPoint2.New(1) - (discountPercent / FixedPoint2.New(100));
            finalPrice[currency] = currentPrice * discountMultiplier;
        }

        return finalPrice;
    }
}
