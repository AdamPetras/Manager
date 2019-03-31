using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.Views;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class TableItemUcVm: INotifyPropertyChanged
    {
        public ICommand More { get; }
        public ICommand Modify { get; }
        public ICommand Delete { get; }
        private Color _isOverTimeColor;
        private IBaseRecord _record;

        public Color IsOverTimeColor
        {
            get => _isOverTimeColor;
            set
            {
                _isOverTimeColor = value;
                OnPropertyChanged(nameof(IsOverTimeColor));
            }
        }

        public IBaseRecord Record
        {
            get => _record;
            set
            {
                _record = value; 
                OnPropertyChanged(nameof(Record));
            }
        }

        public TableItemUcVm(IBaseRecord rec)
        {
            More = new Command<TableItemUcVm>(MoreInformationRecord);
            Delete = new Command<TableItemUcVm>(RemoveRecord);
            Modify = new Command<TableItemUcVm>(ModifyRecord);
            if (rec.Type != ERecordType.Vacation && rec.Type != ERecordType.None)
            {
                if (((IRecord) rec).IsOverTime)
                    IsOverTimeColor = Color.Red;
            }
            _record = rec;
        }

        private void MoreInformationRecord(TableItemUcVm item)
        {
            MessagingCenter.Send(new TableItemUcVm(new NoneRecord()), "ModifyItem");
            if (item.Record.Type == ERecordType.Vacation)
                Application.Current.MainPage.DisplayAlert("Info", "Date: " + item.Record.DateString+"\nVACATION!" + "\nDescription:\n" + item.Record.Description, "OK");
            if (item.Record.Type == ERecordType.Hours)
            {
                IHoursRecord rec = (IHoursRecord) item.Record;
                string overTime = rec.IsOverTime ? "Yes" : "No";
                Application.Current.MainPage.DisplayAlert("Info", "Date: \t\t\t\t\t\t\t" + rec.DateString+ "\nHours: \t\t\t\t\t\t" + rec.Time+ "\nPrice per hour: \t"+rec.Price+ "\nBonus: \t\t\t\t\t\t" + rec.Bonus+ "\nTotal price: \t\t\t" + rec.TotalPrice+ "\nOvertime:\t\t\t\t\t" + overTime + "\nDescription:\n" + rec.Description, "OK");
            }
            else if (item.Record.Type == ERecordType.Pieces)
            {
                IPiecesRecord rec = (IPiecesRecord)item.Record;
                string overTime = rec.IsOverTime ? "Yes" : "No";
                Application.Current.MainPage.DisplayAlert("Info", "Date: \t\t\t\t\t\t\t\t" + rec.DateString + "\nPieces: \t\t\t\t\t\t\t" + rec.Pieces+ "\nPrice per piece: \t" + rec.Price + "\nBonus: \t\t\t\t\t\t\t" + rec.Bonus + "\nTotal price: \t\t\t\t" + rec.TotalPrice + "\nOvertime:\t\t\t\t\t" + overTime + "\nDescription:\n" + rec.Description, "OK");
            }
        }

        private void ModifyRecord(TableItemUcVm item)
        {
            MessagingCenter.Send(item, "ModifyItem");
            ((MasterDetailPage) Application.Current.MainPage).Detail = MainPage.AddTab;
        }

        private void RemoveRecord(TableItemUcVm item)
        {
            MessagingCenter.Send(new TableItemUcVm(new NoneRecord()), "ModifyItem");
            MessagingCenter.Send(item, "Remove");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}