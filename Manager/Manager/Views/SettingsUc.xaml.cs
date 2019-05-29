using System;
using Manager.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsUc
    {
        private SettingsUcVm _settingsUcVm;
        public SettingsUc()
        {
            InitializeComponent();
            _settingsUcVm = new SettingsUcVm();
            BindingContext = _settingsUcVm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _settingsUcVm.SetRestoreButtonEnabled();
        }
    }
}