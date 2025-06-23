using Content.Server.SS220.Stealth.TemporalStealth;
using Content.Shared.SS220.Stealth.StealthImplant;

namespace Content.Server.SS220.Stealth.StealthImplant;

public sealed class StealthImplantSystem : EntitySystem
{
    [Dependency] private readonly TemporalStealthSystem _temporal = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StealthImplantComponent, UseStealthImplantEvent>(OnImplantUse);
    }

    private void OnImplantUse(Entity<StealthImplantComponent> ent, ref UseStealthImplantEvent args)
    {
        if (args.Handled)
            return;

        _temporal.ActivateTemporalStealth(args.Performer, ent.Comp.Visibility, ent.Comp.StealthTime);
        args.Handled = true;
    }
}
