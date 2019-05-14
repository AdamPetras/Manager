using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Model.Interfaces;
using Manager.Resources;
using Manager.SaveManagement;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class SettingsUcVm:INotifyPropertyChanged
    {
        private IXmlSave save;
        private uint _defaultHours;
        private uint _defaultMinutes;
        private double _defaultPrice;
        private uint _defaultPieces;

        private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SaveConfig.xml");

        public ICommand ClearAllRecordsCommand { get; }

        public ICommand SaveSettingsCommand { get; }
        public ICommand RestoreDefaultCommand { get; }

        public uint DefaultHours
        {
            get => _defaultHours;
            set
            {
                if (value <= 23 && value >= 0)
                {
                    _defaultHours = value;
                    SaveStaticVariables.DefaultHours = value;
                    OnPropertyChanged(nameof(DefaultHours));
                }

                else if (value > 23)
                {
                    _defaultHours = 23;
                    SaveStaticVariables.DefaultHours = 23;

                }
                else
                {
                    _defaultHours = 0;
                    SaveStaticVariables.DefaultHours = 0;
                }
            }
        }
        public uint DefaultMinutes
        {
            get => _defaultMinutes;
            set
            {
                if (value <= 59 && value >= 0)
                {
                    _defaultMinutes = value;
                    SaveStaticVariables.DefaultMinutes = value;
                    OnPropertyChanged(nameof(DefaultMinutes));
                }

                else if (value > 59)
                {
                    _defaultMinutes = 59;
                    SaveStaticVariables.DefaultMinutes = 59;
                }
                else
                {
                    _defaultMinutes = 0;
                    SaveStaticVariables.DefaultMinutes = 0;
                }
            }
        }
        public uint DefaultPieces
        {
            get => _defaultPieces;
            set
            {
                _defaultPieces = value;
                SaveStaticVariables.DefaultPieces = value;
                OnPropertyChanged(nameof(DefaultPieces));
            }
        }
        public double DefaultPrice
        {
            get => _defaultPrice;
            set
            {
                _defaultPrice = value;
                SaveStaticVariables.DefaultPrice = value;
                OnPropertyChanged(nameof(DefaultPrice));
            }
        }

        


        public SettingsUcVm()
        {
            save = new XmlSave(_path);
            LoadOnStartup();
            SaveSettingsCommand = new Command(SaveSettings);
            ClearAllRecordsCommand = new Command(ClearRecords);
            RestoreDefaultCommand = new Command(RestoreDefault);
        }

        private void ClearValues()
        {
            DefaultHours = 0;
            DefaultMinutes = 0;
            DefaultPieces = 0;
            DefaultPrice = 0;
        }

        private void RestoreDefault()
        {
            ClearValues();
            SaveSettings();
        }

        private void LoadOnStartup()
        {
            foreach (SaveOption opt in save.LoadXmlFile())
            {
                switch (opt.Title)
                {
                    case nameof(DefaultHours):
                        uint.TryParse(opt.Value, out _defaultHours);
                        DefaultHours = _defaultHours;
                        break;
                    case nameof(DefaultMinutes):
                        uint.TryParse(opt.Value, out _defaultMinutes);
                        DefaultMinutes = _defaultMinutes;
                        break;
                    case nameof(DefaultPieces):
                        uint.TryParse(opt.Value, out _defaultPieces);
                        DefaultPieces = _defaultPieces;
                        break;
                    case nameof(DefaultPrice):
                        double.TryParse(opt.Value, out _defaultPrice);
                        DefaultPrice = _defaultPrice;
                        break;
                }
            }
        }

        private void SaveSettings()
        {
            List<SaveOption> saveList = new List<SaveOption>();
            saveList.Add(new SaveOption(nameof(DefaultHours),DefaultHours.ToString()));
            saveList.Add(new SaveOption(nameof(DefaultMinutes), DefaultMinutes.ToString()));
            saveList.Add(new SaveOption(nameof(DefaultPieces), DefaultPieces.ToString()));
            saveList.Add(new SaveOption(nameof(DefaultPrice), DefaultPrice.ToString(CultureInfo.InvariantCulture)));
            save.CreateXmlFile(saveList);
            ((IXmlBase)save).WriteXmlToConsole(_path);
        }

        public async void ClearRecords()
        {
            if (await Application.Current.MainPage.DisplayAlert(AppResource.DialogRemoveTitle,
                AppResource.ClearDatabaseMessage, AppResource.Yes, AppResource.No))
            {
                MessagingCenter.Send<IBaseRecord>(new NoneRecord(), "ClearRecords");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}