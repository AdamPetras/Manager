using System;
using Manager.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsUc
    {
        public SettingsUc()
        {
            InitializeComponent();
            BindingContext = new SettingsUcVm();
        }
    }
}