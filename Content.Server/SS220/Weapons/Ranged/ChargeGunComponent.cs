// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
namespace Content.Server.SS220.Weapons.Ranged;

[RegisterComponent]
public sealed partial class ChargeGunComponent : Component
{
    [DataField]
    public float ChargeTime = 4f;

    [DataField]
    public bool Charging;
}
