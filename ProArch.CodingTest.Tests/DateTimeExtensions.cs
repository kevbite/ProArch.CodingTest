using System;

namespace ProArch.CodingTest.Tests
{
    public static class DateTimeExtensions
    {
        public static DateTime ChangeYear(this DateTime current, int year)
        {
            return current.AddYears(year - current.Year);
        }
    }
}