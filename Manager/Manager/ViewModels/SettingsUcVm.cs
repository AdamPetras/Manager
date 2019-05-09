using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Manager.Model.Interfaces;
using Manager.Resources;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class SettingsUcVm
    {
        public ICommand ClearAllRecordsCommand { get; }

        public ICommand SaveSettingsCommand { get; }

        public SettingsUcVm()
        {
            SaveSettingsCommand = new Command(SaveSettings);
            ClearAllRecordsCommand = new Command(ClearRecords);
        }

        private void SaveSettings()
        {
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