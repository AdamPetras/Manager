using Manager.ViewModels;

namespace Manager.Views
{
    public partial class SettingsUc
    {
        public SettingsUc()
        {
            InitializeComponent();
            BindingContext = new SettingsUcVm();
        }
    }
}