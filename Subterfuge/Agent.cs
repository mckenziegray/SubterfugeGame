using System;
using Subterfuge.Enums;
using Subterfuge.Exceptions;

namespace Subterfuge
{
    public class Agent
    {
        public string Codename { get; protected set; }
        public AgentType AgentType { get; protected set; }
        public Gender Gender { get; protected set; }
        public bool IsAlive { get; protected set; }
        public Allegiance Allegiance {
            get
            {
                return AgentType switch
                {
                    AgentType.Assassin      => Allegiance.Ally,
                    AgentType.Convoy        => Allegiance.Ally,
                    AgentType.Hacker        => Allegiance.Ally,
                    AgentType.Interrogator  => Allegiance.Ally,
                    AgentType.Marshal       => Allegiance.Ally,
                    AgentType.Medic         => Allegiance.Ally,
                    AgentType.Swallow       => Allegiance.Ally,
                    AgentType.Sentry        => Allegiance.Ally,

                    AgentType.Cutout        => Allegiance.Neutral,
                    AgentType.Intern        => Allegiance.Neutral,

                    AgentType.Android       => Allegiance.Enemy,
                    AgentType.Drudge        => Allegiance.Enemy,
                    AgentType.Fabricator    => Allegiance.Enemy,
                    AgentType.Mastermind    => Allegiance.Enemy,
                    AgentType.Saboteur      => Allegiance.Enemy,

                    _ => throw new NotImplementedException()
                };
            }
        }
        /// <summary>
        /// Whether this agent is acting this round
        /// </summary>
        public bool IsActing { get; set; }
        /// <summary>
        /// The agent that this agent is targeting
        /// </summary>
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
        public bool IsBlocked => Blocker?.IsAlive ?? false;
        public bool IsProtected => Protector != null && Protector.IsAlive && !Protector.IsBlocked;
        public bool RequiresTarget => AgentType switch
        {
            AgentType.Marshal       => true,
            AgentType.Swallow       => true,
            AgentType.Saboteur      => true,
            AgentType.Convoy        => true,
            AgentType.Medic         => true,
            AgentType.Assassin      => true,
            AgentType.Android       => true,
            AgentType.Drudge        => true,
            AgentType.Mastermind    => true,
            AgentType.Fabricator    => true,
            AgentType.Cutout        => true,
            AgentType.Interrogator  => true,
            AgentType.Hacker        => true,
            AgentType.Sentry        => true,
            AgentType.Intern        => true,
            _ => false
        };
        public string Name => AgentType switch
        {
            AgentType.Swallow => Gender switch
            {
                Gender.Male => "Raven",
                Gender.Female => "Swallow",
                _ => "Swallow"
            },
            _ => AgentType.ToString()
        };

        public Agent(AgentType type)
        {
            AgentType = type;
            Codename = GameService.GenerateUniqueCodename();
            Gender = (Gender)GameService.Random.Next(2);
            IsAlive = true;
            Reset();
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
            if (RequiresTarget && Target is null)
                throw new NoTargetException(AgentType);

            switch (AgentType)
            {
                case AgentType.Assassin:
                    if (Target.IsAlive && !IsBlocked && !Target.IsProtected)
                    {
                        Target.Kill(this);
                    }
                    break;
                case AgentType.Medic:
                    if (Target.IsAlive && !IsBlocked)
                    {
                        Target.Protect(this);
                    }
                    break;
                case AgentType.Swallow:
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
                        report = $"Orders received. Target {Target.Codename} neutralized.";
                    else
                        report = "Mission compromised. Standing by for further orders.";
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
                case AgentType.Swallow:
                    report = "I received your orders to carry out the \"special\" operation.";
                    if (!Target.IsAlive || IsBlocked)
                        report += " I'm sorry to say that I was unsuccessful.";
                    else
                        report += $" The mission was a success. {Target.Codename} was tied up all night.";
                    break;
                case AgentType.Interrogator:
                    if (!Target.IsAlive || IsBlocked)
                    {
                        report = "Due to unforeseen circumstances, I was unable to question the target. Don't worry, we'll get 'em next time.";
                    }
                    else
                    {
                        report = $"Per your orders, {Target.Codename} was detained and interrogated.";
                        // TODO: Get information
                    }
                    break;
                case AgentType.Hacker:
                    report = "I got your dead drop, boss.";
                    if (!Target.IsAlive || IsBlocked)
                    {
                        report += " Unfortunately, I hit a bit of a snag. Sorry. Maybe next time.";
                    }
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
