using Robust.Shared.GameStates;

namespace Content.Shared.SS220.TraitorDynamics;

[RegisterComponent, NetworkedComponent]
public sealed partial class DynamicPricesComponent : Component
{
    [DataField]
    public byte CurrentDynamicState;

    public enum DynamicState : byte
    {
        Light,
        Medium,
        Hard,
    }
}
