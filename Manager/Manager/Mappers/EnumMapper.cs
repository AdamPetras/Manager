using System;
using Manager.Model.Enums;
using Manager.Resources;

namespace Manager.Mappers
{
    public class EnumMapper
    {
        public static string MapEnumToString<T>(object value)
        {
            if (!typeof(T).IsEnum)
                return "";
            if (typeof(T) == typeof(EDeleteAction))
                return MapDeleteAction((EDeleteAction)value);
            if (typeof(T) == typeof(ERecordType))
                return MapRecordType((ERecordType)value);
            if (typeof(T) == typeof(ESelectedStage))
                return MapSelectedStage((ESelectedStage) value);
            if (typeof(T) == typeof(EMonths))
                return MapMonths((EMonths)value);
            return "";
        }
        public static string MapEnumToString<T>(Enum value)
        {
            return MapEnumToString<T>((object)value);
        }

        public static string[] MapEnumToStringArray<T>()
        {
            string[] s = new string[Enum.GetNames(typeof(T)).Length];
            int i = 0;
            foreach (T act in Enum.GetValues(typeof(T)))
            {
                s[i++] = MapEnumToString<T>(Enum.Parse(typeof(T), act.ToString()));
            }
            return s;
        }

        private static string MapMonths(EMonths parse)
        {
            switch (parse)
            {
                case EMonths.January:
                    return AppResource.January;
                case EMonths.February:
                    return AppResource.February;
                case EMonths.March:
                    return AppResource.March;
                case EMonths.April:
                    return AppResource.April;
                case EMonths.May:
                    return AppResource.May;
                case EMonths.June:
                    return AppResource.June;
                case EMonths.July:
                    return AppResource.July;
                case EMonths.August:
                    return AppResource.August;
                case EMonths.September:
                    return AppResource.September;
                case EMonths.October:
                    return AppResource.October;
                case EMonths.November:
                    return AppResource.November;
                case EMonths.December:
                    return AppResource.December;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parse), parse, null);
            }
        }

        private static string MapRecordType(ERecordType type)
        {
            switch (type)
            {
                case ERecordType.Hours:
                    return AppResource.HoursType;
                case ERecordType.Pieces:
                    return AppResource.PiecesType;
                case ERecordType.Vacation:
                    return AppResource.VacationType;
                case ERecordType.None:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static string MapSelectedStage(ESelectedStage type)
        {
            switch (type)
            {
                case ESelectedStage.All:
                    return AppResource.AllItem;
                case ESelectedStage.Week:
                    return AppResource.WeekItem;
                case ESelectedStage.LastWeek:
                    return AppResource.LastWeekItem;
                case ESelectedStage.Month:
                    return AppResource.MonthItem;
                case ESelectedStage.LastMonth:
                    return AppResource.LastMonthItem;
                case ESelectedStage.Year:
                    return AppResource.YearItem;
                case ESelectedStage.VacationAll:
                    return AppResource.VacationAllItem;
                case ESelectedStage.VacationYear:
                    return AppResource.VacationYearItem;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static string MapDeleteAction(EDeleteAction action)
        {
            switch (action)
            {
                case EDeleteAction.All:
                    return AppResource.DeleteActionAll;
                case EDeleteAction.VacationsThisYear:
                    return AppResource.DeleteActionVacationThisYear;
                case EDeleteAction.Vacations:
                    return AppResource.DeleteActionVacations;
                case EDeleteAction.ThisYear:
                    return AppResource.DeleteActionThisYear;
                case EDeleteAction.ThisMonth:
                    return AppResource.DeleteActionThisMonth;
                case EDeleteAction.LastYear:
                    return AppResource.DeleteActionLastYear;
                case EDeleteAction.LastMonth:
                    return AppResource.DeleteActionLastMonth;
                case EDeleteAction.LastYears:
                    return AppResource.DeleteActionLastYears;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}