using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Medic : PlayerAgent
    {
        protected enum MedicReportType
        {
            Attacked,
            NotAttacked,
            Blocked,
            SelfIdentify
        }

        public override bool RequiresTarget => true;

        public Medic() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Protect(this, false);
        }

        public override string GetReport()
        {
            return $"I got your orders to look after {Target.Codename}." + GetMedicReportType() switch
            {
                MedicReportType.Blocked => " Unfortunately, I was unable to perform my duties. Sorry.",
                MedicReportType.Attacked => $" {Target.Gender.ToCommonPronoun()} was attacked but is alive and recovering.",
                MedicReportType.NotAttacked => " Thankfully, my services were not required.",
                _ => throw new NotImplementedException()
            };
        }

        protected override ReportType GetReportType()
        {
            return GetMedicReportType() switch
            {
                MedicReportType.Attacked or
                MedicReportType.NotAttacked     => ReportType.Action,
                MedicReportType.Blocked         => ReportType.Blocked,
                MedicReportType.SelfIdentify    => ReportType.SelfIdentify,
                _ => throw new NotImplementedException()
            };
        }

        protected MedicReportType GetMedicReportType()
        {
            if (!Target.IsAlive || IsBlocked)
                return MedicReportType.Blocked;
            else if (Target.WasAttacked)
                return MedicReportType.Attacked;
            else
                return MedicReportType.NotAttacked;
        }
    }
}
