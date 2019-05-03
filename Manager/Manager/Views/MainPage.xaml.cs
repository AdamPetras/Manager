using System;
using System.Threading.Tasks;
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
            AddTab = new NavigationPage(new AddRecordUc());
            TableTab = new NavigationPage(new TableUc());
            AboutTab = new NavigationPage(new AboutUc());
            SettingsTab = new NavigationPage(new SettingsUc());
            CalendarTab = new NavigationPage(new CalendarUc());
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
