using System;
using System.Linq;
using DotNetExtensions;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class AgentListTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void TestConstructor()
        {
            AgentList agentList = new();

            // Make sure there are the right number of agents in each list
            Assert.AreEqual(GameService.AGENT_TYPES_ORDERED.Length, agentList.Count);
            Assert.AreEqual(agentList.Count, agentList.OrderedList.Count);
            Assert.AreEqual(agentList.Count, agentList.ShuffledList.Count);
            int numAllies = GameService.AGENT_TYPES_ORDERED.Select(a => a.Instantiate<Agent>()).Where(a => a.Allegiance == Allegiance.Ally).Count();
            Assert.AreEqual(numAllies, agentList.Allies.Count);

            // Make sure all of the agent types are in each list exactly once
            foreach (Type type in GameService.AGENT_TYPES_ORDERED)
            {
                Assert.DoesNotThrow(() => _ = agentList[type.Name]);
                Assert.DoesNotThrow(() => agentList.OrderedList.Single(a => a.GetType() == type));
                Assert.DoesNotThrow(() => agentList.ShuffledList.Single(a => a.GetType() == type));

                if (type.Instantiate<Agent>().Allegiance == Allegiance.Ally)
                    Assert.DoesNotThrow(() => agentList.Allies.Single(a => a.GetType() == type));
            }

            // Make sure the shuffled list is actually shuffled
            Assert.IsFalse(agentList.OrderedList.SequenceEqual(agentList.ShuffledList));

            // Make sure the shuffling produces a different list each time
            AgentList agentList2 = new AgentList();
            Assert.IsFalse(agentList.ShuffledList.SequenceEqual(agentList2.ShuffledList));
        }
    }
}
