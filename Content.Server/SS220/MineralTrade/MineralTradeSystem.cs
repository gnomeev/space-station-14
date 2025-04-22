using Content.Server.Cargo.Components;
using Content.Server.Cargo.Systems;
using Content.Server.Station.Systems;
using Content.Shared.SS220.MineralTrade;
using Content.Shared.SS220.MineralTrade.Events;
using Content.Shared.SS220.MineralTrade.Protos;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server.SS220.MineralTrade;

public sealed partial class MineralTradeSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly UserInterfaceSystem  _ui = default!;
    [Dependency] private readonly CargoSystem _cargo = default!;
    [Dependency] private readonly StationSystem _station = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MineralTradeComponent, ComponentInit>(OnCompInit);
        SubscribeLocalEvent<MineralTradeComponent, BoundUIOpenedEvent>(OnBUIOpen);
        SubscribeLocalEvent<MineralTradeComponent, AddToCartMsg>(OnAddToCart);
    }

    private void OnAddToCart(Entity<MineralTradeComponent> ent, ref AddToCartMsg args)
    {
        if (!_proto.TryIndex<MineralListingPrototype>(args.Id, out var proto))
            return;

        ent.Comp.Checkout.Add(proto);
        UpdateState(ent.Owner, ent.Comp);
    }

    private void OnBUIOpen(Entity<MineralTradeComponent> ent, ref BoundUIOpenedEvent args)
    {
        GetBank(ent);
        UpdateState(ent, ent.Comp);
    }

    private void OnCompInit(Entity<MineralTradeComponent> ent, ref ComponentInit args)
    {
        PopulateListings(ent);
    }

    public void PopulateListings(MineralTradeComponent trader)
    {
        trader.AvailableListings.Clear();

        foreach (var proto in _proto.EnumeratePrototypes<MineralListingPrototype>())
        {
            trader.AvailableListings.Add(proto);
        }
    }

    public void UpdateState(EntityUid uid, MineralTradeComponent component)
    {
        var state = new MineralTradeState(component.AvailableListings, component.Checkout, component.Balance);
        _ui.SetUiState(uid, MineralTradeTerminalUiKey.Key, state);
    }

    public void GetBank(Entity<MineralTradeComponent> ent)
    {
       var station = _station.GetOwningStation(ent);

       if (TryComp<StationBankAccountComponent>(station, out var bank))
       {
           ent.Comp.Balance = bank.Balance;
       }
    }
}
