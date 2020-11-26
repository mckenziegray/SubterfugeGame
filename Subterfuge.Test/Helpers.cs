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
            Assert.IsTrue(killer.Target.IsActive);
            killer.Target.Reset();
            killer.Reset();

            // An unprotected target should be visited and killed
            killer.IsActing = true;
            killer.Target = target;
            killer.ActIfAble();
            Assert.IsTrue(killer.Target.WasAttacked);
            Assert.IsTrue(killer.Target.Visitors.Contains(killer.Codename));
            Assert.IsFalse(killer.Target.IsActive);
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

                if (agentType == protector.GetType())
                {
                    Assert.IsTrue(target.IsProtected);
                    Assert.AreSame(target.Protector, protector);
                }
            }

            Assert.IsTrue(target.WasAttacked);
            Assert.IsFalse(target.WasKilled);
            Assert.IsTrue(target.IsActive);
            Assert.AreEqual(!protectorShouldDie, target.IsProtected);
            Assert.IsTrue(target.Visitors.Contains(protector.Codename));

            Assert.IsFalse(protector.WasAttacked);
            Assert.AreEqual(protectorShouldDie, protector.WasKilled);
            Assert.AreEqual(!protectorShouldDie, protector.IsActive);
            Assert.IsTrue(protectorShouldDie ? protector.Killer == attacker : protector.Killer is null);
        }

        public static void TestBlockAction(Agent blocker, GameService game)
        {
            NonPlayerAgent target = (NonPlayerAgent)game.Agents[nameof(Drudge)];
            NonPlayerAgent targetTarget = (NonPlayerAgent)game.Agents[nameof(Android)];

            blocker.IsActing = true;
            blocker.Target = target;
            target.IsActing = true;
            target.SelectTarget(game.Agents);

            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
            {
                game.Agents[agentType.Name].ActIfAble();
            }

            Assert.IsTrue(target.IsBlocked);
            Assert.AreSame(blocker, target.Blocker);
            Assert.IsFalse(targetTarget.WasAttacked);
            Assert.IsTrue(target.Visitors.Contains(blocker.Codename));
        }

        //public static void TestBlockerKillInteraction(Agent blocker, Agent killer, GameService game)
        //{
        //    TestBlockerKillInteraction(blocker, killer, game, true);
        //}

        //public static void TestBlockerKillInteraction(Agent blocker, Agent killer, GameService game, bool shouldKill)
        //{
        //    TestBlockerKillInteraction(blocker, killer, game, shouldKill, true);
        //    game.Reset();
        //    blocker = game.Agents[blocker.GetType().Name];
        //    killer = game.Agents[killer.GetType().Name];

        //    TestBlockerKillInteraction(blocker, killer, game, shouldKill, false);
        //}

        public static void TestBlockerKillInteraction(Agent blocker, Agent killer, GameService game, bool shouldKill, bool killerIntendsToAct)
        {
            Agent target = game.Agents[nameof(Hacker)];

            blocker.IsActing = true;
            blocker.Target = killer;
            if (killerIntendsToAct)
            {
                killer.IsActing = true;
                killer.Target = target;
            }

            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
            {
                game.Agents[agentType.Name].ActIfAble();
            }

            Assert.True(killer.Visitors.Contains(blocker.Codename));
            if (shouldKill)
            {
                Assert.IsFalse(killer.IsBlocked);
                Assert.IsNull(killer.Blocker);
            }
            else
            {
                Assert.IsTrue(killer.IsBlocked);
                Assert.AreSame(blocker, killer.Blocker);
            }

            Assert.IsFalse(target.Visitors.Contains(killer.Codename));
            Assert.IsFalse(target.WasAttacked);
            Assert.IsTrue(target.IsActive);

            Assert.IsFalse(blocker.Visitors.Contains(killer.Codename));
            if (shouldKill)
            {
                Assert.AreSame(blocker, killer.Target);
                Assert.IsFalse(blocker.IsActive);
                Assert.IsTrue(blocker.WasKilled);
                Assert.AreSame(killer, blocker.Killer);
            }
            else
            {
                Assert.IsTrue(blocker.IsActive);
                Assert.IsFalse(blocker.WasKilled);
                Assert.IsNull(blocker.Killer);
            }
        }
    }
}
