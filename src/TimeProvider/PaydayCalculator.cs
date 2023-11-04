namespace DotNet8Samples;

public class PaydayCalculator(TimeProvider timeProvider)
{
    public bool IsItPayday()
    {
        var today = timeProvider.GetUtcNow().DateTime;
        var thisMonthsPayday = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);

        if (thisMonthsPayday.DayOfWeek == DayOfWeek.Sunday)
        {
            thisMonthsPayday = thisMonthsPayday.AddDays(-2);
        }
        else if (thisMonthsPayday.DayOfWeek == DayOfWeek.Saturday)
        {
            thisMonthsPayday = thisMonthsPayday.AddDays(-1);
        }

        return thisMonthsPayday.Equals(today);
    }
}
