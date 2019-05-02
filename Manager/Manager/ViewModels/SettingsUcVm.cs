using System.Windows.Input;
using Manager.Model.Interfaces;
using Manager.Resources;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class SettingsUcVm
    {
        public ICommand ClearAllRecords { get; }

        public SettingsUcVm()
        {
            ClearAllRecords = new Command(ClearRecords);
        }

        public async void ClearRecords()
        {
            if (await Application.Current.MainPage.DisplayAlert(AppResource.DialogRemoveTitle,
                AppResource.ClearDatabaseMessage, AppResource.Yes, AppResource.No))
            {
                MessagingCenter.Send<IBaseRecord>(new NoneRecord(), "ClearRecords");
            }
        }
    }
}