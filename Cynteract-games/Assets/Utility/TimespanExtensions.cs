using System;
namespace Cynteract.CCC.Charts
{
    public static class TimespanExtensions
    {
        public static TimeSpan Multiply (this TimeSpan timeSpan, int left)
        {
            return TimeSpan.FromTicks(timeSpan.Ticks * left);
        }

    }
}
