using Robust.Shared.GameStates;

namespace Content.Shared.SS220.Stealth.TemporalStealth;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TemporalStealthComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Visibility;

    [DataField, AutoNetworkedField]
    public TimeSpan StealthTime;

    [DataField, AutoNetworkedField]
    public TimeSpan LastStealthTime;

    public bool? OriginalStealthEnabled;
    public float? OriginalVisibility;
}

public sealed partial class TemporalStealthAddedEvent() : EntityEventArgs
{
}
