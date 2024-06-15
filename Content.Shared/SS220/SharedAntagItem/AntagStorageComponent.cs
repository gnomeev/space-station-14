// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

using Content.Shared.Damage;

namespace Content.Shared.SS220.SharedAntagItem;

[RegisterComponent]
public sealed partial class AntagStorageComponent : Component
{
    [DataField]
    public bool Unequipble;

    [DataField]
    public bool ShouldDamageOnUseInteract;

    [DataField]
    public DamageSpecifier DamageOnInteract = new DamageSpecifier();

    [DataField]
    public string Slot = "back";

    [DataField]
    public string UnquipMesssage = "antag-storage-uniqup-fail-popup";

    [DataField]
    public string InteractMessage = "antag-storage-interact-fail-popup";
}
