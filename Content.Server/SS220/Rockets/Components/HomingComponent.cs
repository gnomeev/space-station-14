// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

namespace Content.Server.SS220.Rockets.Components;

[RegisterComponent]
public sealed partial class HomingComponent : Component
{
    [DataField]
    public EntityUid? Target;
}

