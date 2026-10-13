namespace Content.Server.SS220.Akimbo;

[RegisterComponent]
public sealed partial class AkimboOwnerComponent : Component
{
    [DataField]
    public bool IsOn;
    [DataField]
    public string AkimboToggleAction = "ActionAkimboToggle";

    [DataField]
    public EntityUid? AkimboToggleActionEntity;
}
