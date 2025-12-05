using System.Numerics;
using Content.Server.SS220.Rockets.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Physics.Components;

namespace Content.Server.SS220.Rockets.Systems;


// So it's proportional navigation in station
public sealed partial class HomingSystem : EntitySystem
{
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;

    private const int ProportionalNavigation = 5;
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<HomingComponent, TransformComponent, PhysicsComponent>();
        while (query.MoveNext(out var missile, out var homing, out var missileXform, out var physics))
        {
            if (homing.Target == null)
                continue;

            var target = homing.Target.Value;
            if (!TryComp(target, out TransformComponent? targetXform))
                continue;

            if (!TryComp<PhysicsComponent>(target, out var targetPhysics))
                continue;

            var missilePos = _transform.GetWorldPosition(missile);
            var targetPos = _transform.GetWorldPosition(target);

            var toTarget = targetPos - missilePos;
            var range = toTarget.Length();
            var lineOfSight = toTarget / range;

            var missileVelocity = physics.LinearVelocity;
            var targetVelocity = targetPhysics.LinearVelocity;
            var relativeVelocity = missileVelocity - targetVelocity;
            var closingVelocity = Vector2.Dot(relativeVelocity, lineOfSight);

            var angularRate = lineOfSight.X * relativeVelocity.Y - lineOfSight.Y * relativeVelocity.X;
            var magnitude = ProportionalNavigation * closingVelocity * angularRate;
            var direction = new Vector2(-lineOfSight.Y, lineOfSight.X);

            if (Vector2.Dot(direction, relativeVelocity) < 0)
                direction *= -1;

            var acceleration = direction * magnitude;
            var finalVelocity = missileVelocity += acceleration * frameTime;
            _physics.SetLinearVelocity(missile, finalVelocity);

            if (missileVelocity.Length() > 0.1f)
                _transform.SetLocalRotation(missile, Angle.FromWorldVec(finalVelocity));
        }
    }
}
