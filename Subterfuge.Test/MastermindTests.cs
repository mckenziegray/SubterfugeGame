using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class MastermindTests
    {
        protected GameService Game { get; set; }
        protected Mastermind Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Mastermind)Game.Agents[nameof(Mastermind)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Mastermind", Agent.Name);
        }

        [Test]
        public void TestAllegiance()
        {
            Assert.AreEqual(Allegiance.Enemy, Agent.Allegiance);
        }

        [Test]
        public void TestRequiresTarget()
        {
            Assert.AreEqual(true, Agent.RequiresTarget);
        }

        [Test]
        public void TestSelectTarget()
        {
            Type[] invalidTargets = GameService.AGENT_TYPES_ORDERED.Where(t => Game.Agents[t.Name].Allegiance != Allegiance.Ally).ToArray();
            int numTargets = 10000;
            List<Type> targets = new List<Type>(numTargets);

            // Drudge is still alive, so no target should be selected
            for (int i = 0; i < numTargets; i++)
            {
                Agent.SelectTarget(Game.Agents);
                Assert.IsNull(Agent.Target);
            }

            // Once Drudge is dead, targets can be selected
            Game.Agents[nameof(Drudge)].Execute();
            for (int i = 0; i < numTargets; i++)
            {
                Agent.SelectTarget(Game.Agents);
                if (Agent.Target is not null)
                {
                    Assert.IsFalse(invalidTargets.Contains(Agent.Target.GetType()));
                    targets.Add(Agent.Target.GetType());
                }
                Agent.Reset();
            }

            // With enough trials, every valid target should have been selected at least once
            Assert.AreEqual(Game.Agents.OrderedList.Where(a => !invalidTargets.Contains(a.GetType())).Count(), targets.Distinct().Count());
        }

        [Test]
        public void TestAct()
        {
            Agent target = Game.Agents[nameof(Hacker)];

            Helpers.TestKillAction(Agent, Game);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[Agent.GetType().Name];
            target = Game.Agents[nameof(Hacker)];

            // If targeted by a blocker while the Drudge is still alive, nothing special should happen
            Agent blocker1 = Game.Agents[nameof(Swallow)];
            blocker1.IsActing = true;
            blocker1.Target = Agent;
            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
                Game.Agents[agentType.Name].ActIfAble();
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsNull(Agent.Target);
            Assert.IsFalse(blocker1.WasAttacked);
            Assert.IsFalse(blocker1.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(blocker1.IsAlive);
            Agent.Reset();
            target.Reset();
            blocker1.Reset();

            Agent blocker2 = Game.Agents[nameof(Marshal)];
            blocker2.IsActing = true;
            blocker2.Target = Agent;
            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
                Game.Agents[agentType.Name].ActIfAble();
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsNull(Agent.Target);
            Assert.IsFalse(blocker2.WasAttacked);
            Assert.IsFalse(blocker2.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(blocker2.IsAlive);
            Agent.Reset();
            target.Reset();
            blocker2.Reset();

            // If targeted by a blocker while the Drudge is dead, the Mastermind should change targets and kill the blocker (without visiting either one)
            Game.Agents[nameof(Drudge)].Execute();
            Agent blocker3 = Game.Agents[nameof(Swallow)];
            Agent.IsActing = true;
            Agent.Target = target;
            blocker3.IsActing = true;
            blocker3.Target = Agent;
            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
                Game.Agents[agentType.Name].ActIfAble();
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsTrue(Agent.Target == blocker3);
            Assert.IsTrue(blocker3.WasAttacked);
            Assert.IsFalse(blocker3.Visitors.Contains(Agent.Codename));
            Assert.IsFalse(blocker3.IsAlive);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[Agent.GetType().Name];
            target = Game.Agents[nameof(Hacker)];

            Game.Agents[nameof(Drudge)].Execute();
            Agent blocker4 = Game.Agents[nameof(Marshal)];
            Agent.IsActing = true;
            Agent.Target = target;
            blocker4.IsActing = true;
            blocker4.Target = Agent;
            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
                Game.Agents[agentType.Name].ActIfAble();
            Assert.IsTrue(Agent.Target == blocker4);
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsTrue(Agent.Target == blocker4);
            Assert.IsTrue(blocker4.WasAttacked);
            Assert.IsFalse(blocker4.Visitors.Contains(Agent.Codename));
            Assert.IsFalse(blocker4.IsAlive);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[Agent.GetType().Name];
            target = Game.Agents[nameof(Hacker)];

            // The same should be true even if the Mastermind didn't intend to attack
            Game.Agents[nameof(Drudge)].Execute();
            Agent blocker5 = Game.Agents[nameof(Swallow)];
            Agent.IsActing = true;
            Agent.Target = target;
            blocker5.IsActing = true;
            blocker5.Target = Agent;
            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
                Game.Agents[agentType.Name].ActIfAble();
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsTrue(Agent.Target == blocker5);
            Assert.IsTrue(blocker5.WasAttacked);
            Assert.IsFalse(blocker5.Visitors.Contains(Agent.Codename));
            Assert.IsFalse(blocker5.IsAlive);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[Agent.GetType().Name];
            target = Game.Agents[nameof(Hacker)];

            Game.Agents[nameof(Drudge)].Execute();
            Agent blocker6 = Game.Agents[nameof(Marshal)];
            Agent.IsActing = true;
            Agent.Target = target;
            blocker6.IsActing = true;
            blocker6.Target = Agent;
            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
                Game.Agents[agentType.Name].ActIfAble();
            Assert.IsTrue(Agent.Target == blocker6);
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsTrue(Agent.Target == blocker6);
            Assert.IsTrue(blocker6.WasAttacked);
            Assert.IsFalse(blocker6.Visitors.Contains(Agent.Codename));
            Assert.IsFalse(blocker6.IsAlive);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[Agent.GetType().Name];
            target = Game.Agents[nameof(Hacker)];
        }
    }
}
