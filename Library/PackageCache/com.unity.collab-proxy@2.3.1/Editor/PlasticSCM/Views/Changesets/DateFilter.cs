using System;
using PlasticGui.WorkspaceWindow.QueryViews;

namespace Unity.PlasticSCM.Editor.Views.Changesets
{
    internal class DateFilter
    {
        internal enum Type
        {
            LastWeek,
            Last15Days,
            LastMonth,
            Last3Months,
            LastYear,
            AllTime
        }

        internal Type FilterType;

        internal DateFilter(Type filterType)
        {
            FilterType = filterType;
        }

        internal string GetTimeAgo()
        {
            switch (FilterType)
            {
                case DateFilter.Type.LastWeek:
                    return QueryConstants.OneWeekAgo;
                case DateFilter.Type.Last15Days:
                    return QueryConstants.HalfMonthAgo;
                case DateFilter.Type.LastMonth:
                    return QueryConstants.OneMonthAgo;
                case DateFilter.Type.Last3Months:
                    return QueryConstants.ThreeMonthsAgo;
                case DateFilter.Type.LastYear:
                    return QueryConstants.OneYearAgo;
            }

            return string.Empty;
        }
    }
}
