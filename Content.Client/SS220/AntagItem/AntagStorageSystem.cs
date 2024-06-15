// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.SS220.SharedAntagItem;

namespace Content.Client.SS220.AntagItem;

public sealed partial class AntagStorageSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<AntagStorageComponent, BeingUnequippedAttemptEvent>(OnUnequipAttempt);
    }

    private void OnUnequipAttempt(Entity<AntagStorageComponent> entity, ref BeingUnequippedAttemptEvent ev)
    {
        if (entity.Comp.Unequipble) // Shoud be in Shared .IsClient?
            ev.Cancel();
    }

}
