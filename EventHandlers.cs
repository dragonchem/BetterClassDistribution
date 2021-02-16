using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterClassDistribution
{
    public class EventHandlers
    {
        public static BetterClassDistribution plugin;
        private Dictionary<string, RoleType> LastRoles = new Dictionary<string, RoleType>();
        private Random rnd = new Random();
        private List<RoleType> lsSCPRoles = new List<RoleType>();
        public EventHandlers(BetterClassDistribution pl)
        {
            plugin = pl;
        }

        public void OnRoundStart()
        {
            Dictionary<string, RoleType> PreviousRoundRoles = LastRoles;
            LastRoles = new Dictionary<string, RoleType>();
            lsSCPRoles = new List<RoleType>();

            float DRand = rnd.Next(plugin.Config.DClassMin, plugin.Config.DClassMax);
            float SCPRand = rnd.Next(plugin.Config.SCPMin, plugin.Config.SCPMax);
            float GuardRand = rnd.Next(plugin.Config.GuardMin, plugin.Config.GuardMax);
            float ScientistRand = rnd.Next(plugin.Config.ScientistMin, plugin.Config.ScientistMax);

            float RandomTotal = DRand + SCPRand + ScientistRand + GuardRand;
            float DChance = DRand / RandomTotal;
            float SCPChance = SCPRand / RandomTotal;
            float ScientistChance = ScientistRand / RandomTotal;
            float GuardChance = GuardRand / RandomTotal;

            float RemovePercentile = 1f / Player.List.Count();

            // Randomizer
            foreach (Player ply in Player.List)
            {
                RoleType prevRole = RoleType.Tutorial;
                PreviousRoundRoles.TryGetValue(ply.UserId, out prevRole);
                double plyChance = rnd.NextDouble() * RandomTotal / 100;
                Log.Info($"PlayerChance: ${plyChance}, RandomTotal: {RandomTotal}, DChance: {DChance}, SCPChance: {SCPChance}, GuardChance: {GuardChance}, ScientistChance: {ScientistChance}");
                if (plyChance < DChance && DChance > 0f && prevRole != RoleType.ClassD)
                {
                    ply.SetRole(RoleType.ClassD);
                    LastRoles.Remove(ply.UserId);
                    LastRoles.Add(ply.UserId, RoleType.ClassD);
                    DChance = DChance - RemovePercentile;
                }
                else if ((plyChance - DChance) < SCPChance && SCPChance > 0f && prevRole != RoleType.Scp049)
                {
                    RandomSCP(ply);
                    LastRoles.Remove(ply.UserId);
                    LastRoles.Add(ply.UserId, RoleType.Scp049);
                    SCPChance = SCPChance - RemovePercentile;
                }
                else if ((plyChance - DChance - SCPChance) < GuardChance && GuardChance > 0f && prevRole != RoleType.FacilityGuard)
                {
                    ply.SetRole(RoleType.FacilityGuard);
                    LastRoles.Remove(ply.UserId);
                    LastRoles.Add(ply.UserId, RoleType.FacilityGuard);
                    GuardChance = GuardChance - RemovePercentile;
                }
                else if (prevRole != RoleType.Scientist)
                {
                    ply.SetRole(RoleType.Scientist);
                    LastRoles.Remove(ply.UserId);
                    LastRoles.Add(ply.UserId, RoleType.Scientist);
                    ScientistChance = ScientistChance - RemovePercentile;
                }
                else
                {
                    ply.SetRole(RoleType.ClassD);
                    LastRoles.Remove(ply.UserId);
                    LastRoles.Add(ply.UserId, RoleType.ClassD);
                    DChance = DChance - RemovePercentile;
                }
                RandomTotal = RandomTotal - (RemovePercentile * 100);
            }


            // Prioritizer
            IEnumerable<Player> lsDClass = Player.List.Where(x => x.Role == RoleType.ClassD);
            IEnumerable<Player> lsSCP = Player.List.Where(x => x.Team == Team.SCP);
            IEnumerable<Player> lsGuard = Player.List.Where(x => x.Role == RoleType.FacilityGuard);
            IEnumerable<Player> lsScientist = Player.List.Where(x => x.Role == RoleType.Scientist);

            if (lsDClass.Count() == 0)
            {
                if (lsScientist.Count() > lsGuard.Count())
                {
                    Player player = lsScientist.ElementAt(rnd.Next(1, lsScientist.Count()) - 1);
                    player.SetRole(RoleType.ClassD);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.ClassD);
                }
                else if (lsGuard.Count() > lsScientist.Count() && lsGuard.Count() > 2)
                {
                    Player player = lsGuard.ElementAt(rnd.Next(1, lsGuard.Count()) - 1);
                    player.SetRole(RoleType.ClassD);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.ClassD);
                }
                else if (lsSCP.Count() > 1)
                {
                    Player player = lsSCP.ElementAt(rnd.Next(1, lsSCP.Count()) - 1);
                    player.SetRole(RoleType.ClassD);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.ClassD);
                }
                else
                {
                    Log.Error("No suitable targets found to be turned into D-class");
                }
            }

            lsDClass = Player.List.Where(x => x.Role == RoleType.ClassD);
            lsSCP = Player.List.Where(x => x.Team == Team.SCP);
            lsGuard = Player.List.Where(x => x.Role == RoleType.FacilityGuard);
            lsScientist = Player.List.Where(x => x.Role == RoleType.Scientist);

            if (lsSCP.Count() == 0)
            {
                if (lsDClass.Count() > 1)
                {
                    Player player = lsDClass.ElementAt(rnd.Next(1, lsDClass.Count()) - 1);
                    RandomSCP(player);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.Scp049);
                }
                else if (lsScientist.Count() > 0)
                {
                    Player player = lsScientist.ElementAt(rnd.Next(1, lsScientist.Count()) - 1);
                    RandomSCP(player);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.Scp049);
                }
                else if (lsGuard.Count() > 0)
                {
                    Player player = lsGuard.ElementAt(rnd.Next(1, lsGuard.Count()) - 1);
                    RandomSCP(player);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.Scp049);
                }
                else
                {
                    Log.Error("No suitable targets found to be turned into SCP");
                }
            }

            lsDClass = Player.List.Where(x => x.Role == RoleType.ClassD);
            lsSCP = Player.List.Where(x => x.Team == Team.SCP);
            lsGuard = Player.List.Where(x => x.Role == RoleType.FacilityGuard);
            lsScientist = Player.List.Where(x => x.Role == RoleType.Scientist);

            if (lsGuard.Count() == 0)
            {
                if (lsDClass.Count() > 1)
                {
                    Player player = lsDClass.ElementAt(rnd.Next(1, lsDClass.Count()) - 1);
                    player.SetRole(RoleType.FacilityGuard);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.FacilityGuard);
                }
                else if (lsScientist.Count() > 0)
                {
                    Player player = lsScientist.ElementAt(rnd.Next(1, lsScientist.Count()) - 1);
                    player.SetRole(RoleType.FacilityGuard);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.FacilityGuard);
                }
                else if (lsSCP.Count() > 1)
                {
                    Player player = lsSCP.ElementAt(rnd.Next(1, lsSCP.Count()) - 1);
                    player.SetRole(RoleType.FacilityGuard);
                    LastRoles.Remove(player.UserId);
                    LastRoles.Add(player.UserId, RoleType.FacilityGuard);
                }
                else
                {
                    Log.Error("No suitable targets found to be turned into Guard");
                }
            }
        }

        private void RandomSCP(Player ply, int lastscp = 0)
        {
            int scp = 0;
            if (lastscp > 0)
            {
                scp = lastscp + 1;
                if (scp == 7)
                {
                    scp = 1;
                }
            }
            else
            {
                if (lsSCPRoles.Count() > 0)
                {
                    scp = new Random().Next(1, 6);
                }
                else
                {
                    scp = new Random().Next(1, 7);
                }
            }
            switch (scp)
            {
                case 1:
                    if (lsSCPRoles.Where(x => x == RoleType.Scp173).Count() > 0)
                    {
                        RandomSCP(ply, scp);
                    }
                    else
                    {
                        ply.SetRole(RoleType.Scp173);
                        lsSCPRoles.Add(RoleType.Scp173);
                    }
                    break;
                case 2:
                    if (lsSCPRoles.Where(x => x == RoleType.Scp049).Count() > 0)
                    {
                        RandomSCP(ply, scp);
                    }
                    else
                    {
                        ply.SetRole(RoleType.Scp049);
                        lsSCPRoles.Add(RoleType.Scp049);
                    }
                    break;
                case 3:
                    if (lsSCPRoles.Where(x => x == RoleType.Scp096).Count() > 0)
                    {
                        RandomSCP(ply, scp);
                    }
                    else
                    {
                        ply.SetRole(RoleType.Scp096);
                        lsSCPRoles.Add(RoleType.Scp096);
                    }
                    break;
                case 4:
                    if (lsSCPRoles.Where(x => x == RoleType.Scp106).Count() > 0)
                    {
                        RandomSCP(ply, scp);
                    }
                    else
                    {
                        ply.SetRole(RoleType.Scp106);
                        lsSCPRoles.Add(RoleType.Scp106);
                    }
                    break;
                case 5:
                    if (lsSCPRoles.Where(x => x == RoleType.Scp93953).Count() > 0)
                    {
                        ply.SetRole(RoleType.Scp93953);
                        lsSCPRoles.Add(RoleType.Scp93953);
                    }
                    else if (lsSCPRoles.Where(x => x == RoleType.Scp93989).Count() > 0)
                    {
                        RandomSCP(ply, scp);
                    }
                    else
                    {
                        ply.SetRole(RoleType.Scp93989);
                        lsSCPRoles.Add(RoleType.Scp93989);
                    }
                    break;
                case 6:
                    if (lsSCPRoles.Where(x => x == RoleType.Scp079).Count() > 0)
                    {
                        RandomSCP(ply, scp);
                    }
                    else
                    {
                        ply.SetRole(RoleType.Scp079);
                        lsSCPRoles.Add(RoleType.Scp079);
                    }
                    break;
                case 7:
                    Log.Error("Fout");
                    break;
            }
        }
    }
}
