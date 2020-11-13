using SubterfugeGame.Enums;
using SubterfugeGame.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubterfugeGame.Code
{
    public class Agent
    {
        private Random random = new Random();
        public string Codename { get; protected set; }
        public AgentType AgentType { get; protected set; }
        public Gender Gender { get; protected set; }
        public bool IsAlive { get; protected set; }
        public Allegiance Allegiance { 
            get
            {
                switch (AgentType)
                {
                    case AgentType.Assassin:
                    case AgentType.Hacker:
                    case AgentType.HoneyTrapper:
                    case AgentType.Interrogator:
                    case AgentType.Medic:
                        return Allegiance.Ally;
                    default:
                        return Allegiance.Neutral;
                }
            }
        }
        /// <summary>
        /// Whether this agent is acting this round
        /// </summary>
        public bool IsActing { get; set; }
        public Agent Target { get; set; }
        /// <summary>
        /// The agent blocking this agent
        /// </summary>
        protected Agent Blocker { get; set; }
        /// <summary>
        /// The agent protecting this agent
        /// </summary>
        protected Agent Protector { get; set; }
        /// <summary>
        /// The agent who killed this agent
        /// </summary>
        protected Agent Killer { get; set; }
        public bool WasAttacked { get; protected set; }
        public bool IsBlocked { 
            get
            {
                return Blocker?.IsAlive ?? false;
            } 
        }
        public bool IsProtected
        {
            get
            {
                return Protector == null ? false : Protector.IsAlive && !Protector.IsBlocked;
            }
        }

        public Agent(AgentType type)
        {
            Codename = GenerateCodename();
            AgentType = type;
            Gender = (Gender)random.Next(2);
            Killer = null;
            Reset();
        }

        public string GenerateCodename()
        {
            return string.Format("{0}{1}-{2}{3}{4}",
                random.Next(26) + 'A',
                random.Next(26) + 'A',
                random.Next(10),
                random.Next(10),
                random.Next(10)
                );
        }

        public void Kill(Agent killer)
        {
            IsAlive = false;
            Killer = killer;
        }

        public void Block(Agent blocker)
        {
            Blocker = blocker;
        }

        public void Protect(Agent protector)
        {
            Protector = protector;
        }

        public void Act()
        {
            switch (AgentType)
            {
                case AgentType.Assassin:
                    if (Target == null)
                        throw new NoTargetException(AgentType);

                    if (Target.IsAlive && !IsBlocked && !Target.IsProtected)
                    {
                        Target.Kill(this);
                    }
                    break;
                case AgentType.Medic:
                    if (Target == null)
                        throw new NoTargetException(AgentType);

                    if (Target.IsAlive && !IsBlocked)
                    {
                        Target.Protect(this);
                    }
                    break;
                case AgentType.HoneyTrapper:
                    else if (Target == null)
                        throw new NoTargetException(AgentType);

                    if (Target.IsAlive && !IsBlocked)
                    {
                        Target.Block(this);
                        Target.Protect(this);
                    }
                    break;
                case AgentType.Interrogator:
                    throw new NotImplementedException();
                case AgentType.Hacker:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException($"{nameof(Enums.AgentType)} {AgentType} not recognized.");
            }
        }

        public string GetReport()
        {
            string report;

            switch (AgentType)
            {
                case AgentType.Assassin:
                    if (Target.Killer == this)
                    {
                        report = $"Orders received. Target {Target.Codename} neutralized.";
                    }
                    else
                    {
                        report = "Mission compromised. Standing by for further orders.";
                    }
                    break;
                case AgentType.Medic:
                    report = $"I got your orders to look after {Target.Codename}.";
                    if (!Target.IsAlive || IsBlocked)
                    {
                        report += " Unfortunately, I was unable to perform my duties. Apologies.";
                    }
                    else if (Target.WasAttacked)
                    {
                        report += " The target was attacked but is alive and recovering.";
                    }
                    else
                    {
                        report += " Thankfully, my services were not required.";
                    }
                    break;
                case AgentType.HoneyTrapper:
                    report = "I received your orders to carry out the \"special\" operation.";
                    if (!Target.IsAlive || IsBlocked)
                        report += " I'm sorry to say that I was unsuccessful.";
                    else
                        report += $" The mission was a success. {Target.Codename} was tied up all night.";
                    break;
                case AgentType.Interrogator:
                    if (!Target.IsAlive || IsBlocked)
                        report = "Due to unforeseen circumstances, I was unable to question the target. Don't worry, we'll get 'em next time.";
                    else
                    {
                        report = $"Per your orders, {Target.Codename} was detained and interrogated.";
                        // TODO: Get information
                    }
                    break;
                case AgentType.Hacker:
                    report = "I got your dead drop, boss.";
                    if (!Target.IsAlive || IsBlocked)
                        report += " Unfortunately, I hit a bit of a snag. Sorry. Maybe next time.";
                    else
                    {
                        report += $" I did some digging into {Target.Codename}.";
                        // TODO: Get information
                    }
                    break;
                default:
                    throw new ArgumentException($"{nameof(Enums.AgentType)} {AgentType} not recognized.");
            }

            return report;
        }

        public void Reset()
        {
            IsActing = false;
            Target = null;
            Blocker = null;
            Protector = null;
            WasAttacked = false;
        }
    }
}
