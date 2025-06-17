using Content.Shared.GameTicking.Components;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.SS220.TraitorDynamics.Components;

namespace Content.Shared.SS220.TraitorDynamics;

/// <summary>
/// This handles...
/// </summary>
public abstract class SharedTraitorDynamicsSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    [ValidatePrototypeId<WeightedRandomPrototype>]
    private const string WeightsProto = "WeightedDynamicsList";

    public string GetRandomDynamic()
    {
        var validWeight = _prototype.Index<WeightedRandomPrototype>(WeightsProto);
        return validWeight.Pick(_random);
    }

    public void SetRandomDynamic(EntityUid ent, TraitorDynamicsComponent? comp = null)
    {
        var dynamic = GetRandomDynamic();
        SetDynamic(ent, dynamic, comp);
    }

    public void SetDynamic(EntityUid ent, string proto, TraitorDynamicsComponent? comp = null)
    {
        if (!Resolve(ent, ref comp))
            return;

        if (!_prototype.TryIndex<DynamicPrototype>(proto, out var dynamicProto))
            return;

        comp.CurrentDynamic = dynamicProto.ID;
        var ev = new DynamicAddedEvent(ent, dynamicProto.ID);
        RaiseLocalEvent(ev);

        if (dynamicProto.LoreNames == default || !_prototype.TryIndex(dynamicProto.LoreNames, out var namesProto))
            return;

        dynamicProto.SelectedLoreName = _random.Pick(namesProto.ListNames);
    }

    /// <summary>
    /// Tries to find the type of dynamic while in Traitor game rule
    /// </summary>
    /// <returns>installed dynamic</returns>
    public ProtoId<DynamicPrototype>? GetCurrentDynamic()
    {
        //gnv: каждого треитор рула свой динамик, стоило бы проверять динамик у конкретного рула
        var query = EntityQueryEnumerator<TraitorDynamicsComponent>();

        while (query.MoveNext(out var comp))
        {
            return comp.CurrentDynamic;
        }
        return default;
    }
}
