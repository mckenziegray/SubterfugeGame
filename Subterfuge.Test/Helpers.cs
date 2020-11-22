using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;

namespace Subterfuge.Test
{
    public static class Helpers
    {
        public static void TestKillAction(Agent killer, GameService game)
        {
            Agent target = game.Agents[nameof(Hacker)];
            Agent protector = game.Agents[nameof(Medic)];

            // A protected target should be visited and attacked but not killed
            killer.IsActing = true;
            killer.Target = target;
            killer.Target.Protect(protector);
            killer.ActIfAble();
            Assert.IsTrue(killer.Target.WasAttacked);
            Assert.IsTrue(killer.Target.Visitors.Contains(killer.Codename));
            Assert.IsTrue(killer.Target.IsAlive);
            killer.Target.Reset();
            killer.Reset();

            // An unprotected target should be visited and killed
            killer.IsActing = true;
            killer.Target = target;
            killer.ActIfAble();
            Assert.IsTrue(killer.Target.WasAttacked);
            Assert.IsTrue(killer.Target.Visitors.Contains(killer.Codename));
            Assert.IsFalse(killer.Target.IsAlive);
        }
    }
}
