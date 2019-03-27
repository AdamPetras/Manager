using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class TableItemUcVm: INotifyPropertyChanged
    {
        public ICommand More { get; }
        public ICommand Modify { get; }
        public ICommand Delete { get; }
        private IBaseRecord _record;
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
            More = new Command<TableItemUcVm>(MoreInformationsRecord);
            Delete = new Command<TableItemUcVm>(RemoveRecord);
            Modify = new Command<TableItemUcVm>(ModifyRecord);
            _record = rec;
        }

        private void MoreInformationsRecord(TableItemUcVm item)
        {
            if (item.Record.Type == ERecordType.Vacation)
                return;
            if (item.Record.Type == ERecordType.Hours)
            {
                IHoursRecord rec = (IHoursRecord) item.Record;
                Application.Current.MainPage.DisplayAlert("Info", "Date: \t\t\t\t\t\t\t" + rec.DateString+ "\nHours: \t\t\t\t\t\t" + rec.Time+ "\nPrice per hour: \t"+rec.Price+ "\nBonus: \t\t\t\t\t\t" + rec.Bonus+ "\nTotal price: \t\t\t" + rec.TotalPrice, "OK");
            }
            else if (item.Record.Type == ERecordType.Pieces)
            {
                IPiecesRecord rec = (IPiecesRecord)item.Record;
                Application.Current.MainPage.DisplayAlert("Info", "Date: \t\t\t\t\t\t\t\t" + rec.DateString + "\nPieces: \t\t\t\t\t\t\t" + rec.Pieces+ "\nPrice per piece: \t" + rec.Price + "\nBonus: \t\t\t\t\t\t\t" + rec.Bonus + "\nTotal price: \t\t\t\t" + rec.TotalPrice, "OK");
            }
        }

        private void ModifyRecord(TableItemUcVm item)
        {
            MessagingCenter.Send(item, "ModifyItem");
        }

        private void RemoveRecord(TableItemUcVm item)
        {
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