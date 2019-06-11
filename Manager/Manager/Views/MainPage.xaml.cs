using System;
using System.Threading.Tasks;
using Manager.Resources;
using Xamarin.Forms;

namespace Manager.Views
{
    public partial class MainPage : MasterDetailPage
    {
        public static readonly NavigationPage AddTab;
        public static readonly NavigationPage TableTab;
        public static readonly NavigationPage AboutTab;
        public static readonly NavigationPage SettingsTab;
        public static readonly NavigationPage CalendarTab;

        static MainPage()
        {
            SettingsTab = new NavigationPage(new SettingsUc(){Title = AppResource.SettingsTab});
            AddTab = new NavigationPage(new AddRecordUc(){Title = AppResource.AddRecordTab});
            TableTab = new NavigationPage(new TableUc(){Title = AppResource.TableTab});
            AboutTab = new NavigationPage(new AboutUc() { Title = AppResource.AboutTab});
            CalendarTab = new NavigationPage(new CalendarUc(){Title = AppResource.CalendarTab});
        }

        public MainPage()
        {
            InitializeComponent();
            Detail = AddTab;
            IsPresented = false;
        }


        public async void ButtonAddPageClicked(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                Detail = AddTab;
            });
            IsPresented = false;
        }

        public async void ButtonTablePageClicked(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                Detail = TableTab;
            });
            IsPresented = false;
        }

        public async void ButtonAboutPageClicked(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                Detail = AboutTab;
            });
            IsPresented = false;
        }

        public async void ButtonSettingsPageClicked(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                Detail = SettingsTab;
            });
            IsPresented = false;
        }

        public async void ButtonCalendarPageClicked(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                Detail = CalendarTab;
            });
            IsPresented = false;
        }

    }
}
