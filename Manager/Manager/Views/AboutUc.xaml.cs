using Manager.ViewModels;

namespace Manager.Views
{
    public partial class AboutUc
    {
        public AboutUc()
        {
            InitializeComponent();
            BindingContext = new AboutUcVm();
        }
    }
}