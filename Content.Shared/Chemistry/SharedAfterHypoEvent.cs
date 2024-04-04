namespace Content.Shared.Chemistry
{
    [ByRefEvent]
    public readonly struct AfterHypoEvent
    {
        public readonly EntityUid User;

        public readonly EntityUid Target;

        public AfterHypoEvent(EntityUid user, EntityUid target)
        {
            User = user;
            Target = target;
        }
    }
}
