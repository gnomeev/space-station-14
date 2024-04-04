using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Tag;
using Content.Server.Popups;
using Content.Shared.Chemistry;

namespace Content.Server.Chemistry.EntitySystems
{
    public sealed partial class AutoinjectorSystem : EntitySystem
    {
        [Dependency] private readonly SolutionContainerSystem _solutionContainerSystem = default!;
        public override void Initialize()
        {
            SubscribeLocalEvent<AutoinjectorComponent, AfterHypoEvent>(OnCheck);
        }

        private void OnCheck(EntityUid uid, AutoinjectorComponent component, AfterHypoEvent args)
        {
            if (!_solutionContainerSystem.TryGetSolution(uid, component.Solution, out _, out var solutions))
                return;

            if (solutions.Volume <= 0)
            {
                RemComp<SolutionContainerManagerComponent>(uid);
                RemComp<HyposprayComponent>(uid);
            }

        }
    }
}

