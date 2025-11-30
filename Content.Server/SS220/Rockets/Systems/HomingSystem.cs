using Content.Server.SS220.Rockets.Components;
using Robust.Server.GameObjects;

namespace Content.Server.SS220.Rockets.Systems;

public sealed partial class HomingSystem : EntitySystem
{
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<HomingComponent, TransformComponent>();
        while (query.MoveNext(out var missile, out var homing, out var transform))
        {
            if (homing.Target == null || !TryComp<TransformComponent>(homing.Target.Value, out var targetXform))
                continue;

            var direction = (_transform.GetWorldPosition(homing.Target.Value) - _transform.GetWorldPosition(missile)).Normalized();
            var currentRot = _transform.GetWorldRotation(missile);
            var targetRot = Angle.FromWorldVec(direction);

        }
    }
}
