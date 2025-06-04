namespace Content.Shared.SS220.Stealth.TemporalStealth;

[RegisterComponent]
public sealed partial class TemporalStealthComponent : Component
{
    [DataField]
    public float Visibility;

    [DataField]
    public TimeSpan StealthTime;

    [DataField]
    public TimeSpan LastStealthTime;

    [DataField]
    public bool HasComp;
}
