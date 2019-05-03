using System;
using System.Linq;
using System.Windows.Input;
using Manager.Model;
using Manager.Model.Enums;
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

        public CalendarUc()
        {
            InitializeComponent();
            BindingContext = new CalendarUcVm();
        }

        private void IndexChanged(object sender, EventArgs eventArgs)
        {
            if (sender is Picker pick)
            {
                if (pick.SelectedIndex >= 0 && pick.SelectedIndex <= 11)
                    DrawCalendar(2019, pick.SelectedIndex + 1);
            }
        }

        private void DrawCalendar(int year, int month)
        {
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
            CreateLabels("Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun");
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
                Color bgcol = Color.Gray;
                TableItemUcVm item = null;
                if (TableUcVm.SavedRecordList?.Count>0)
                    foreach (TableItemUcVm tableItem in TableUcVm.SavedRecordList)
                    {
                        if (tableItem?.Record == null) continue;
                        if (tableItem.Record.Date.Year == today.Year &&
                            tableItem.Record.Date.Month == today.Month &&
                            tableItem.Record.Date.Day == today.Day)
                        {
                            item = tableItem;
                            bgcol = tableItem.Record.Type == ERecordType.Vacation ? Color.Blue : Color.Green;
                            break;
                        }
                    }

                CalendarButton butt = new CalendarButton()
                {
                    Text = (i + 1).ToString(),
                    BackgroundColor = bgcol,
                    Item = item
                };
                if (item != null)
                    butt.Command = new Command(()=>item.MoreInformationRecord(butt.Item));
                _grid.Children.Add(butt, (i + offset) % 7, (i + offset) / 7 + 1);
            }
        }
        private DayOfWeek GetTheFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).DayOfWeek;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DrawCalendar(2019, PickerMonth.SelectedIndex+1);
        }
    }
}