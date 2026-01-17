using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.ValueObjects
{
    public readonly struct EvaluationPeriod
    {
        public DateTime Value { get; }

        private EvaluationPeriod(DateTime value)
        {
            Value = value;
        }

        public static EvaluationPeriod CurrentMonthUtc()
        {
            var now = DateTime.UtcNow;
            return new EvaluationPeriod(new DateTime(now.Year, now.Month, 1));
        }
    }
}
