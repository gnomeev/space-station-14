namespace  Content.Server.SS220.Weapons.Ranged.Events;
using Content.Shared.Damage;

[ByRefEvent]
public record struct BeforeHitscanDamageEvent(DamageSpecifier? Damage, EntityUid Target, EntityUid Gun);
