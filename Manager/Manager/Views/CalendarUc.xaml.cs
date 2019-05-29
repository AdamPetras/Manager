using System;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Manager.Model;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.Resources;
using Manager.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarUc
    {
        private Grid _grid;
        private bool _gridExists;
        private CalendarUcVm _calendarBinding;
        public CalendarUc()
        {
            InitializeComponent();
            _calendarBinding = new CalendarUcVm();
            BindingContext = _calendarBinding;
        }

        private void IndexChanged(object sender, EventArgs eventArgs)
        {
                if (_calendarBinding.SelectedMonth >= 0 && _calendarBinding.SelectedMonth <= 11)
                    DrawCalendar(_calendarBinding.Year, _calendarBinding.SelectedMonth + 1);
        }

        private void DrawCalendar(int year, int month)
        {
            _calendarBinding.ClearStats();
            Statistics.Month(TableUcVm.SavedRecordList,month,CalcAndSet);
            if (_gridExists)
            {
                Background.Children.Remove(_grid);
                _gridExists = false;
            }
            DrawCalendarHeader();
            DrawMonthAsCalendar(year, month);
            Background.Children.Add(_grid);
        }

        private void DrawCalendarHeader()
        {
            _grid = new Grid();
            _gridExists = true;
            for (int i = 0; i < 7; i++)
                CreateColumnsAndRows(true, 1, GridUnitType.Star);
            for (int i = 0; i < 6; i++)
                CreateColumnsAndRows(false, 1, GridUnitType.Star);
            CreateLabels(AppResource.Monday, AppResource.Tuesday, AppResource.Wednesday, AppResource.Thursday, AppResource.Friday, AppResource.Saturday, AppResource.Sunday);
        }

        private void CreateLabels(params string[] days)
        {
            int index = 0;
            foreach (string day in days)
            {
                _grid.Children.Add(
                    new Label
                    {
                        Text = day, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center
                    }, index++, 0);
            }
        }

        private void CreateColumnsAndRows(bool isColumn, int length, GridUnitType type)
        {
            if (isColumn)
                _grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(length, type)});
            else
                _grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(length, type)});
        }

        private void DrawMonthAsCalendar(int year, int month)
        {
            int offset = (int) (GetTheFirstDayOfMonth(new DateTime(year, month, 1)));
            if (offset == 0) //ošetření sunday == 0 => offset = 6
            {
                offset = 6;
            }
            else offset -= 1;

            for (int i = 0; i < DateTime.DaysInMonth(year, month); i++)
            {
                DateTime today = new DateTime(year,month,i+1);
                Color backgroundColor = Color.Gray;
                TableItemUcVm item = null;
                if (TableUcVm.SavedRecordList?.Count>0)
                    foreach (TableItemUcVm tableItem in TableUcVm.SavedRecordList)
                    {
                        if (tableItem?.Record?.Date.Year == today.Year &&
                            tableItem?.Record?.Date.Month == today.Month &&
                            tableItem?.Record?.Date.Day == today.Day)
                        {
                            item = tableItem;
                            backgroundColor = tableItem.Record.Type == ERecordType.Vacation ? Color.Blue : Color.Green;
                            break;
                        }
                    }

                CalendarButton butt = new CalendarButton()
                {
                    Text = (i + 1).ToString(),
                    BackgroundColor = backgroundColor,
                    Item = item
                };
                if (item != null)
                    butt.Command = new Command(()=>item.MoreInformationRecord(butt.Item));
                else
                {
                    butt.Command = new Command(() => ChangeTabToAddRecordAndSetupDate(today));
                }

                _grid.Children.Add(butt, (i + offset) % 7, (i + offset) / 7 + 1);
            }
        }

        private void ChangeTabToAddRecordAndSetupDate(DateTime today)
        {
            ((MasterDetailPage)Application.Current.MainPage).Detail = MainPage.AddTab;
            MessagingCenter.Send(new DateTimeAsReference(today), "SetupDate");
        }

        private DayOfWeek GetTheFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).DayOfWeek;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DrawCalendar(_calendarBinding.Year, _calendarBinding.SelectedMonth + 1);
        }

        private void EntryYear_OnCompleted(object sender, EventArgs e)
        {
            DrawCalendar(_calendarBinding.Year, _calendarBinding.SelectedMonth + 1);
        }

        private void CalcAndSet(TableItemUcVm rec)
        {
            _calendarBinding.TotalDays++;
            switch (rec.Record.Type)
            {
                case ERecordType.Hours:
                    SetHoursStats((IHoursRecord)rec.Record);
                    break;
                case ERecordType.Pieces:
                    _calendarBinding.TotalPieces += ((IPiecesRecord)rec.Record).Pieces;
                    break;
            }
            if (rec.Record.Type != ERecordType.Vacation)
            {
                _calendarBinding.TotalBonus += ((IRecord)rec.Record).Bonus;
                double.TryParse(rec.Record.TotalPrice, NumberStyles.Any, CultureInfo.InvariantCulture,
                    out double price);
                /*if (rec.Record.Type == ERecordType.Hours)
                {
                    _totalPriceForHourType += price;
                }*/
                _calendarBinding.TotalPrice += price;
            }
            else
            {
                _calendarBinding.VacationDays++;
            }
        }

        private void SetHoursStats(IHoursRecord rec)
        {
            _calendarBinding.TotalTime += rec.Time + rec.OverTime;
            /*_totalOvertimeHours += rec.OverTime;
            _totalOvertimePrice += rec.OverTime.Hours * rec.Price + (rec.OverTime.Minutes / 60.0) * rec.Price;*/
        }

    }
}