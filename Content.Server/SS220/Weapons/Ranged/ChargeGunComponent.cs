// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

using Content.Shared.Damage;

namespace Content.Server.SS220.Weapons.Ranged;

[RegisterComponent]
public sealed partial class ChargeGunComponent : Component
{
    [DataField]
    public TimeSpan ChargeTime = TimeSpan.FromSeconds(5);

    [DataField]
    public TimeSpan ChargeUntil;

    [DataField]
    public int AmmoPerCharge;

    [DataField]
    public bool Charging;

    [DataField]
    public int MaxChargesCount;

    [DataField]
    public int CurChargesCount;

    [DataField]
    public DamageSpecifier DamagePerCharge = new();
}
