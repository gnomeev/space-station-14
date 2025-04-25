using Content.Client.SS220.MineralTrade.UI;
using Content.Shared.SS220.MineralTrade.Events;
using Robust.Client.UserInterface;

namespace Content.Client.SS220.MineralTrade.BUI;

public sealed class MineralTradeBoundUI : BoundUserInterface
{
    private MineralTradeTerminal? _window;
    private bool _isProcessing;

    public MineralTradeBoundUI(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MineralTradeTerminal>();
        _window.OpenCentered();
        _window.OnClose += Close;

        _window.AddToCart += (who) =>
        {
            if (_isProcessing)
                return;

            _isProcessing = true;
            SendMessage(new AddToCartMsg(who));
        };

        _window.CartCheckout += (who) =>
        {
            SendMessage(new CheckoutMsg(who));
        };
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _window?.Close();
    }


    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is MineralTradeState listingsMsg)
        {
            _window?.PopulateMaterialList(listingsMsg.Listings);
            _window?.PopulateCheckoutList(listingsMsg.Checkout);
            _window?.UpdateBank(listingsMsg.Balance);
        }
    }
}
