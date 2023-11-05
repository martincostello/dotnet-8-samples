namespace DotNet8Samples;

public class PaydayCalculator(TimeProvider timeProvider)
{
    public PaydayCalculator()
        : this(TimeProvider.System)
    {
    }

    public bool IsItPayday()
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().DateTime);

        // Payday is on the last weekday of the month
        var thisMonthsPayday = new DateOnly(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);

        if (thisMonthsPayday.DayOfWeek == DayOfWeek.Sunday)
        {
            thisMonthsPayday = thisMonthsPayday.AddDays(-2);
        }
        else if (thisMonthsPayday.DayOfWeek == DayOfWeek.Saturday)
        {
            thisMonthsPayday = thisMonthsPayday.AddDays(-1);
        }

        return thisMonthsPayday == today;
    }
}
