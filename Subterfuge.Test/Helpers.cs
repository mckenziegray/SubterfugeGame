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

        public static void TestProtectAction(Agent protector, GameService game, bool protectorShouldDie)
        {
            Agent target = game.Agents[nameof(Hacker)];
            Agent attacker = game.Agents[nameof(Android)];

            protector.IsActing = true;
            protector.Target = target;
            attacker.IsActing = true;
            attacker.Target = target;

            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
            {
                game.Agents[agentType.Name].ActIfAble();

                if (agentType == typeof(Agent))
                {
                    Assert.IsTrue(target.IsProtected);
                    Assert.AreSame(target.Protector, protector);
                }
            }

            Assert.IsTrue(target.WasAttacked);
            Assert.IsFalse(target.WasKilled);
            Assert.IsTrue(target.IsAlive);
            Assert.IsFalse(target.IsProtected);

            Assert.IsFalse(protector.WasAttacked);
            Assert.AreEqual(protectorShouldDie, protector.WasKilled);
            Assert.AreEqual(!protectorShouldDie, protector.IsAlive);
            Assert.IsTrue(protectorShouldDie ? protector.Killer == attacker : protector.Killer is null);
        }
    }
}
