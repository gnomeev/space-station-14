namespace Content.Server.Chemistry.Components
{
    [RegisterComponent]
    public sealed partial class AutoinjectorComponent : Component
    {
        [DataField("solution")]
        public string Solution = string.Empty;
    }
}
