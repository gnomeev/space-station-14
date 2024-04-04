using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server.Chemistry.Components
{
    [RegisterComponent]
    public sealed partial class AutoinjectorComponent : Component
    {
        [DataField("solution")]
        public string Solution = string.Empty;
    }
}
