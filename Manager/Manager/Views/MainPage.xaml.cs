using System;
using Xamarin.Forms;

namespace Manager.Views
{
    public partial class MainPage : MasterDetailPage
    {
        private readonly NavigationPage _addTab;
        private readonly NavigationPage _tableTab;
        private readonly NavigationPage _aboutTab;
        public MainPage()
        {
            InitializeComponent();
            _addTab = new NavigationPage(new AddRecordUc());
            _tableTab = new NavigationPage(new TableUc());
            _aboutTab = new NavigationPage( new AboutUc());
            Detail = _addTab;
            IsPresented = false;
        }

        public void ButtonAddPageClicked(object sender, EventArgs e)
        {
            Detail = _addTab;
            IsPresented = false;
        }

        public void ButtonTablePageClicked(object sender, EventArgs e)
        {
            Detail = _tableTab;
            IsPresented = false;
        }

        public void ButtonAboutPageClicked(object sender, EventArgs e)
        {
            Detail = _aboutTab;
            IsPresented = false;
        }
    }
}
