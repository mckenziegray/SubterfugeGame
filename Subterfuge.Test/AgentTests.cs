using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

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

            agent.Frame(framer);
            agent.Reset();
            Assert.IsFalse(agent.WasFramed);

            agent.Attack(killer);
            agent.Reset();
            Assert.IsFalse(agent.WasAttacked);
            Assert.IsFalse(agent.WasKilled);
            Assert.IsFalse(agent.IsAlive);
            Assert.IsNotNull(agent.Killer);
        }
    }
}
