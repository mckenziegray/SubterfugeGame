using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;

namespace Subterfuge.Test
{
    public class GameServiceTests
    {
        private GameService _game;

        [SetUp]
        public void Setup()
        {
            _game = new GameService();
        }

        [Test]
        public void TestConstructor()
        {
            Assert.AreEqual(1, _game.Round);
            Assert.IsNotNull(_game.Agents);
            Assert.AreNotEqual(1, _game.Agents.Count);
        }

        [Test]
        public void TestGenerateUniqueCodename()
        {
            // Generate a bunch of codenames and make sure none of them were the same
            int codenamesToGenerate = 100000;
            List<string> generatedCodenames = new List<string>(codenamesToGenerate);
            for (int i = 0; i < codenamesToGenerate; i++)
            {
                string newCodename = GameService.GenerateUniqueCodename();
                generatedCodenames.Add(newCodename);
            }

            Assert.AreEqual(generatedCodenames.Distinct().Count(), generatedCodenames.Count);
        }

        [Test]
        public void TestEndRound()
        {
            int currentDay = _game.Round;

            // Change something on one of the agents that should get reset when the round ends
            _game.Agents[nameof(Hacker)].Target = _game.Agents[nameof(Android)];

            _game.EndRound();

            // Make sure that the values got updated as expected for a new round
            Assert.AreEqual(currentDay + 1, _game.Round);
            Assert.IsNull(_game.Agents[nameof(Hacker)].Target);
        }


        [Test]
        public void TestReset()
        {
            // Change some things that should get reset
            _game.EndRound();
            _game.Agents[nameof(Hacker)].Execute();

            // Make sure the initial values make sense
            Assert.IsTrue(_game.Round > 1);
            Assert.IsFalse(_game.Agents[nameof(Hacker)].IsActive);

            _game.Reset();

            // Make sure the game actually got reset
            Assert.AreEqual(1, _game.Round);
            Assert.IsTrue(_game.Agents[nameof(Hacker)].IsActive);
        }

        [Test]
        public void TestPlayRound()
        {
            // Reset just in case the game got messy from one of the other tests
            _game.Reset();

            // Make sure that the values to be checked later are what they should be before the game starts
            Assert.IsFalse(_game.Agents.OrderedList.Any(a => a.WasFramed));
            Assert.IsTrue(_game.Agents[nameof(Fabricator)].IsActive);
            Assert.IsNull(_game.Agents[nameof(Fabricator)].Target);

            _game.PlayRound();

            // Make sure that target selection actually happened
            Assert.IsNotNull(_game.Agents[nameof(Saboteur)].Target);

            // Make sure that actions actually happened
            // If the Fabricator's target was framed, the Fabricator acted; if the Fabricator is dead, the Android acted
            Assert.IsTrue(_game.Agents[nameof(Fabricator)].Target.WasFramed || !_game.Agents[nameof(Fabricator)].IsActive);
        }
    }
}