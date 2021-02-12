using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server = Exiled.Events.Handlers.Server;

namespace BetterClassDistribution
{
    public class BetterClassDistribution : Plugin<Config>
    {
        public EventHandlers Handler { get; private set; }
        public override string Name => nameof(BetterClassDistribution);
        public override string Author => "Written by TheLazyKitten";
        public override Version Version { get; } = new Version(1, 0, 0);
        public BetterClassDistribution() { }

        public override void OnEnabled()
        {
            Handler = new EventHandlers(this);
            Server.RoundStarted += Handler.OnRoundStart;
        }

        public override void OnDisabled()
        {
            Server.RoundStarted -= Handler.OnRoundStart;
            Handler = null;
        }
    }
}
