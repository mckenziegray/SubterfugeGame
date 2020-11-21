using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;
using Subterfuge.Exceptions;

namespace Subterfuge.Test
{
    public class AgentTests
    {
        [Test]
        public void TestConstructor()
        {
            List<Gender> genders = new List<Gender>(GameService.AGENT_TYPES_ORDERED.Length);

            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
            {
                Agent agent = agentType.Instantiate<Agent>();

                Assert.IsTrue(agent.IsAlive);
                Assert.IsFalse(agent.IsActing);
                Assert.IsFalse(agent.IsBlocked);
                Assert.IsFalse(agent.IsProtected);
                Assert.IsFalse(agent.WasFramed);
                Assert.IsFalse(agent.WasAttacked);
                Assert.IsFalse(agent.WasKilled);
                Assert.IsFalse(string.IsNullOrWhiteSpace(agent.Name));
                Assert.IsFalse(string.IsNullOrWhiteSpace(agent.Codename));
                Assert.IsNull(agent.Target);
                Assert.IsNull(agent.Blocker);
                Assert.IsNull(agent.Protector);
                Assert.IsNull(agent.Killer);
                Assert.IsNotNull(agent.Visitors);
                Assert.AreEqual(0, agent.Visitors.Count);

                genders.Add(agent.Gender);
            }

            // Make sure there's a mix of genders
            // Assuming it's 50/50, there's only a 0.003% chance that all the agents have the same gender
            Assert.AreNotEqual(1, genders.Distinct().Count());
        }

        [Test]
        public void TestCanAct()
        {
            Agent agent1 = new Hacker();
            Agent agent2 = new Saboteur();

            // They should not be able to act initially because they don't have targets
            Assert.False(agent1.CanAct);
            Assert.False(agent2.CanAct);

            // They should be able to act once they have targets
            agent1.Target = agent2;
            agent2.Target = agent1;
            Assert.True(agent1.CanAct);
            Assert.True(agent2.CanAct);

            // A blocked agent should't be able to act
            agent1.Block(agent2);
            Assert.IsFalse(agent1.CanAct);

            // A dead agent should't be able to act
            agent2.Execute();
            Assert.IsFalse(agent2.CanAct);

            // If the blocker dies, the blocked agent should no longer be blocked
            Assert.IsTrue(agent1.CanAct);
        }

        [Test]
        public void TestReset()
        {
            Agent agent = new Hacker();
            Agent protector = new Medic();
            Agent blocker = new Saboteur();
            Agent framer = new Fabricator();
            Agent killer = new Android();

            string name = agent.Name;
            string codename = agent.Codename;
            Gender gender = agent.Gender;
            agent.Reset();
            Assert.AreEqual(name, agent.Name);
            Assert.AreEqual(codename, agent.Codename);
            Assert.AreEqual(gender, agent.Gender);

            agent.Visit(protector);
            agent.Reset();
            Assert.IsNotNull(agent.Visitors);
            Assert.AreEqual(0, agent.Visitors.Count);

            agent.Target = protector;
            agent.IsActing = true;
            agent.Reset();
            Assert.IsNull(agent.Target);
            Assert.IsFalse(agent.IsActing);

            agent.Protect(protector);
            agent.Reset();
            Assert.IsFalse(agent.IsProtected);
            Assert.IsNull(agent.Protector);

            agent.Block(blocker);
            agent.Reset();
            Assert.IsFalse(agent.IsBlocked);
            Assert.IsNull(agent.Blocker);

            //agent.Frame(framer);
            //agent.Reset();
            //Assert.IsFalse(agent.WasFramed);

            agent.Attack(killer);
            agent.Reset();
            Assert.IsFalse(agent.WasAttacked);
            Assert.IsFalse(agent.WasKilled);
            Assert.IsFalse(agent.IsAlive);
            Assert.IsNotNull(agent.Killer);
        }

        [Test]
        public void TestVisit()
        {
            Agent agent = new Hacker();
            Agent visitor1 = new Android();
            Agent visitor2 = new Saboteur();

            agent.Visit(visitor1);
            Assert.AreEqual(1, agent.Visitors.Count);
            Assert.IsTrue(agent.Visitors.Contains(visitor1.Codename));

            agent.Visit(visitor2);
            Assert.AreEqual(2, agent.Visitors.Count);
            Assert.IsTrue(agent.Visitors.Contains(visitor1.Codename));
            Assert.IsTrue(agent.Visitors.Contains(visitor2.Codename));
        }

        [Test]
        public void TestProtect()
        {
            Agent agent = new Hacker();
            Agent protector = new Medic();
            Agent attacker = new Android();

            agent.Protect(protector);
            Assert.IsTrue(agent.IsProtected);
            Assert.IsNotNull(agent.Protector);
            Assert.IsTrue(agent.Visitors.Contains(protector.Codename));

            agent.Attack(attacker);
            Assert.IsTrue(agent.WasAttacked);
            Assert.IsTrue(agent.IsAlive);
        }

        [Test]
        public void TestBlock()
        {
            Agent agent = new Hacker();
            Agent blocker = new Saboteur();

            agent.Block(blocker);
            Assert.IsTrue(agent.IsBlocked);
            Assert.IsNotNull(agent.Blocker);
            Assert.IsTrue(agent.Visitors.Contains(blocker.Codename));
            Assert.IsFalse(agent.CanAct);

            // CanAct should still be false after a target is assigned
            agent.Target = blocker;
            Assert.IsFalse(agent.CanAct);
        }

        [Test]
        public void TestFrame()
        {
            Agent agent = new Hacker();
            Agent framer = new Fabricator();

            agent.Frame(framer);
            Assert.IsTrue(agent.WasFramed);
            Assert.IsTrue(agent.Visitors.Contains(framer.Codename));
        }

        [Test]
        public void TestAttack()
        {
            Agent agent = new Hacker();
            Agent attacker = new Android();
            Agent protector = new Medic();

            agent.Protect(protector);
            agent.Attack(attacker);
            Assert.IsTrue(agent.WasAttacked);
            Assert.IsTrue(agent.IsAlive);
            Assert.IsNull(agent.Killer);

            agent.Reset();

            agent.Attack(attacker);
            Assert.IsTrue(agent.WasAttacked);
            Assert.IsFalse(agent.IsAlive);
            Assert.IsNotNull(agent.Killer);
            Assert.AreSame(attacker, agent.Killer);
        }

        [Test]
        public void TestExecute()
        {
            Agent agent = new Hacker();

            agent.Execute();
            Assert.IsFalse(agent.IsAlive);
            Assert.IsFalse(agent.WasKilled);
            Assert.IsNull(agent.Killer);
        }

        [Test]
        public void TestActIfAble()
        {
            Agent actor = new Assassin();
            Agent target = new Drudge();
            Agent blocker = new Saboteur();

            // No target and IsActing is false => no action
            Assert.Throws<NoTargetException>(() => actor.ActIfAble());
            Assert.IsFalse(target.WasAttacked);
            actor.Reset();

            // IsActing is true but no target => no action
            actor.IsActing = true;
            Assert.Throws<NoTargetException>(() => actor.ActIfAble());
            Assert.IsFalse(target.WasAttacked);
            actor.Reset();

            // Has a target but IsActing is false => no action
            actor.Target = target;
            actor.ActIfAble();
            Assert.IsFalse(target.WasAttacked);
            actor.Reset();

            // Has a target and IsActing is true but blocked => no action
            actor.Target = target;
            actor.IsActing = true;
            actor.Block(blocker);
            actor.ActIfAble();
            Assert.IsFalse(target.WasAttacked);
            actor.Reset();

            // Has a target and IsActing is true, and not blocked => action
            actor.Target = target;
            actor.IsActing = true;
            actor.ActIfAble();
            Assert.IsTrue(target.WasAttacked);
            actor.Reset();
            target.Reset();

            // Target is dead => no action
            actor.Target = target;
            actor.IsActing = true;
            actor.ActIfAble();
            Assert.IsFalse(target.WasAttacked);
            actor.Reset();
            target = new Drudge();

            // Actor is dead => no action
            actor.Target = target;
            actor.IsActing = true;
            actor.Execute();
            actor.ActIfAble();
            Assert.IsFalse(target.WasAttacked);
            actor.Reset();
        }
    }
}
