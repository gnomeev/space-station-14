using System.Linq;
using Content.Server.Cargo.Systems;
using Content.Server.Popups;
using Content.Server.Station.Systems;
using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.SS220.MineralTrade;
using Content.Shared.SS220.MineralTrade.Events;
using Content.Shared.SS220.MineralTrade.Protos;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.SS220.MineralTrade;

public sealed partial class MineralTradeSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly UserInterfaceSystem  _ui = default!;
    [Dependency] private readonly CargoSystem _cargo = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MineralTradeComponent, ComponentInit>(OnCompInit);
        SubscribeLocalEvent<MineralTradeComponent, BoundUIOpenedEvent>(OnBUIOpen);
        SubscribeLocalEvent<MineralTradeComponent, AddToCartMsg>(OnAddToCart);
        SubscribeLocalEvent<MineralTradeComponent, CheckoutMsg>(OnCheckout);
    }

    private void OnCheckout(Entity<MineralTradeComponent> ent, ref CheckoutMsg args)
    {
        var station = _station.GetOwningStation(ent);

        if (station is null || !TryComp<StationBankAccountComponent>(station, out var bank))
            return;

        var finalPrice = args.Checkout.Sum(x => x.Key.Price * x.Value);

        if (finalPrice > ent.Comp.Balance)
        {
            _popup.PopupEntity("Недостаточно средств", ent.Owner);
            return;
        }

        foreach (var (proto, amount) in args.Checkout)
        {
            if(!_proto.HasIndex(proto.ID))
                continue;

            for (var i = 0; i < amount; i++)
            {
                Spawn(proto.ID, Transform(ent).Coordinates);
            }
        }

        _cargo.UpdateBankAccount((station.Value,bank), finalPrice, ent.Comp.Account);
        ent.Comp.Balance -= finalPrice;
        UpdateState(ent, ent.Comp);
    }

    private void OnAddToCart(Entity<MineralTradeComponent> ent, ref AddToCartMsg args)
    {
        if (!_proto.TryIndex<MineralListingPrototype>(args.Id, out var proto))
            return;

        if (!ent.Comp.Checkout.TryAdd(proto, args.Amount))
            ent.Comp.Checkout[proto] = 1;

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

       if (!TryComp<StationBankAccountComponent>(station, out var bank))
            return;

       foreach (var (account, value) in bank.Accounts)
       {
           if (account.Id == ent.Comp.Account.Id)
               ent.Comp.Balance = value;
       }
    }
}
