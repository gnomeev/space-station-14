using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared.SS220.Stealth.StealthImplant;

[RegisterComponent, NetworkedComponent]
public sealed partial class StealthImplantComponent : Component
{
    [DataField]
    public TimeSpan StealthTime = TimeSpan.FromSeconds(8);

    [DataField]
    public TimeSpan LastStealthTime;
}

public sealed partial class UseStealthImplantEvent : InstantActionEvent;
