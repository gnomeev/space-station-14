// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

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
    public int ChargesPerShot;

    [DataField]
    public DamageSpecifier DamagePerCharge = new();

    [DataField]
    public ChargeGunModifiers Modifiers = new();
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class ChargeGunModifiers
{
    [DataField]
    public SoundSpecifier? SoundGunshot;

    [DataField]
    public float? CameraRecoilScala;

    [DataField]
    public Angle? AngleIncrease;

    [DataField]
    public Angle? AngleDecay;

    [DataField]
    public Angle? MaxAngle;

    [DataField]
    public Angle? MinAngle;

    [DataField]
    public int? ShotsPerBurst;

    [DataField]
    public float? FireRate;

    [DataField]
    public float? ProjectileSpeed;
}
