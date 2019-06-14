using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Mappers;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.Resources;
using Manager.SaveManagement;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class SettingsUcVm:INotifyPropertyChanged
    {
        private readonly IXmlSave _save;
        private double _defaultPrice;
        private uint _defaultPieces;
        private readonly IXmlManager _manager;
        private static readonly string Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SaveConfig.xml");
        private bool _isRestoreDefaultEnabled;
        private int _selectedDeleteAction;
        private TimeSpan _defaultTimeTo;
        private TimeSpan _defaultTimeFrom;

        public ICommand ClearAllRecordsCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand RestoreDefaultCommand { get; }
        public ICommand ExportDataCommand { get; }
        public ICommand ImportDataCommand { get; }

        public List<string> DeleteActionsList { get; } = new List<string>(EnumMapper.MapEnumToStringArray<EDeleteAction>());

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

        public bool IsRestoreDefaultEnabled
        {
            get => _isRestoreDefaultEnabled;
            set
            {
                _isRestoreDefaultEnabled = value;
                OnPropertyChanged(nameof(IsRestoreDefaultEnabled));
            }
        }

        public int SelectedDeleteAction
        {
            get => _selectedDeleteAction;
            set
            {
                _selectedDeleteAction = value;
                OnPropertyChanged(nameof(SelectedDeleteAction));
            }
        }

        public TimeSpan DefaultTimeFrom
        {
            get => _defaultTimeFrom;
            set
            {
                _defaultTimeFrom = value;
                SaveStaticVariables.DefaultTimeFrom = value;
                OnPropertyChanged(nameof(DefaultTimeFrom));
            }
        }

        public TimeSpan DefaultTimeTo
        {
            get => _defaultTimeTo;
            set
            {
                _defaultTimeTo = value;
                SaveStaticVariables.DefaultTimeTo = value;
                OnPropertyChanged(nameof(DefaultTimeTo));
            }
        }

        public SettingsUcVm()
        {
            _save = new XmlSave(Path);
            _manager = new XmlManager(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "recordData.xml"));
            LoadOnStartup();
            SaveSettingsCommand = new Command(SaveSettings);
            ClearAllRecordsCommand = new Command(ClearRecords);
            RestoreDefaultCommand = new Command(RestoreDefault);
            ExportDataCommand = new Command(ExportData);
            ImportDataCommand = new Command(ImportData);
            SelectedDeleteAction = 0;
        }

        private void ImportData()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(directory, "cachedData.xml");
            if (!File.Exists(filePath))
                return;
            ((IXmlBase)_manager).StringToXml(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "recordData.xml"), File.ReadAllText(filePath));
            LoadRecordsAsync();
        }

        private void ExportData()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(directory, "cachedData.xml");
            File.WriteAllText(filePath,((IXmlBase)_manager).XmlToString(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "recordData.xml")));
        }

        private async void LoadRecordsAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (IBaseRecord tmp in _manager.LoadXmlFile())
                {
                    TableUcVm.SavedRecordList.Add(new TableItemUcVm(tmp));
                }
            });
        }

        public void SetRestoreButtonEnabled()
        {
            if (DefaultTimeFrom == TimeSpan.Zero && DefaultTimeTo == TimeSpan.Zero && Math.Abs(DefaultPrice) < 0.2 && DefaultPieces == 0)
                IsRestoreDefaultEnabled = false;
            else IsRestoreDefaultEnabled = true;
        }

        private void ClearValues()
        {
            DefaultTimeFrom = TimeSpan.Zero;
            DefaultTimeTo = TimeSpan.Zero;
            DefaultPieces = 0;
            DefaultPrice = 0;
            SetRestoreButtonEnabled();
        }

        private void RestoreDefault()
        {
            ClearValues();
            SaveSettings();
        }

        private void LoadOnStartup()
        {
            foreach (SaveOption opt in _save.LoadXmlFile())
            {
                switch (opt.Title)
                {
                    case nameof(DefaultTimeFrom):
                        TimeSpan.TryParse(opt.Value, out _defaultTimeFrom);
                        DefaultTimeFrom = _defaultTimeFrom;
                        break;
                    case nameof(DefaultTimeTo):
                        TimeSpan.TryParse(opt.Value, out _defaultTimeTo);
                        DefaultTimeTo = _defaultTimeTo;
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
            SetRestoreButtonEnabled();
        }

        private void SaveSettings()
        {
            List<SaveOption> saveList = new List<SaveOption>
            {
                new SaveOption(nameof(DefaultTimeFrom), DefaultTimeFrom.ToString()),
                new SaveOption(nameof(DefaultTimeTo), DefaultTimeTo.ToString()),
                new SaveOption(nameof(DefaultPieces), DefaultPieces.ToString()),
                new SaveOption(nameof(DefaultPrice), DefaultPrice.ToString(CultureInfo.InvariantCulture))
            };
            _save.CreateXmlFile(saveList);
            ((IXmlBase)_save).WriteXmlToConsole(Path);
            SetRestoreButtonEnabled();
        }

        public async void ClearRecords()
        {
            if (await Application.Current.MainPage.DisplayAlert(AppResource.DialogRemoveTitle,
                AppResource.ClearDatabaseMessage, AppResource.Yes, AppResource.No))
            {
                MessagingCenter.Send<IBaseRecord, EDeleteAction>(new NoneRecord(), "ClearRecords", (EDeleteAction)SelectedDeleteAction);
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