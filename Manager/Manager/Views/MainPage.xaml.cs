using System;
using Xamarin.Forms;

namespace Manager.Views
{
    public partial class MainPage : MasterDetailPage
    {
        public static readonly NavigationPage AddTab;
        public static readonly NavigationPage TableTab;
        public static readonly NavigationPage AboutTab;

        static MainPage()
        {
            AddTab = new NavigationPage(new AddRecordUc());
            TableTab = new NavigationPage(new TableUc());
            AboutTab = new NavigationPage(new AboutUc());
        }

        public MainPage()
        {
            InitializeComponent();
            Detail = AddTab;
            IsPresented = false;
        }


        public void ButtonAddPageClicked(object sender, EventArgs e)
        {
            Detail = AddTab;
            IsPresented = false;
        }

        public void ButtonTablePageClicked(object sender, EventArgs e)
        {
            Detail = TableTab;
            IsPresented = false;
        }

        public void ButtonAboutPageClicked(object sender, EventArgs e)
        {
            Detail = AboutTab;
            IsPresented = false;
        }
    }
}
