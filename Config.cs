using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;

namespace BetterClassDistribution
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("The minimum ratio for D-Class spawns")]
        public int DClassMin { get; set; } = 31;
        [Description("The maximum ratio for D-Class spawns")]
        public int DClassMax { get; set; } = 51;
        [Description("The minimum ratio for SCP spawns")]
        public int SCPMin { get; set; } = 7;
        [Description("The maximum ratio for SCP spawns")]
        public int SCPMax { get; set; } = 27;
        [Description("The minimum ratio for Guard spawns")]
        public int GuardMin { get; set; } = 6;
        [Description("The maximum ratio for Guard spawns")]
        public int GuardMax { get; set; } = 26;
        [Description("The minimum ratio for Scientist spawns")]
        public int ScientistMin { get; set; } = 10;
        [Description("The maximum ratio for Scientist spawns")]
        public int ScientistMax { get; set; } = 30;
    }
}
