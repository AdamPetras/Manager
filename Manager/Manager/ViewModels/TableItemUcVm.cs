using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Model;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.Resources;
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
            if (rec.Date.DayOfWeek == DayOfWeek.Saturday || rec.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                IsOverTimeColor = Color.Red;
            }
            if (rec.Type == ERecordType.Hours)
            {
                if (((IHoursRecord) rec).OverTime != new WorkTime())
                    IsOverTimeColor = Color.DeepSkyBlue;
            }
            _record = rec;
        }

        public void MoreInformationRecord(TableItemUcVm item)
        {
            MessagingCenter.Send(new TableItemUcVm(new NoneRecord()), "ModifyItem");
            if (item.Record.Type == ERecordType.Vacation)
                Application.Current.MainPage.DisplayAlert("Info", AppResource.Date+": " + item.Record.DateString+"\n"+AppResource.VacationType+"!" + "\n"+AppResource.Description+":\n" + item.Record.Description, AppResource.Ok);
            if (item.Record.Type == ERecordType.Hours)
            {
                IHoursRecord rec = (IHoursRecord) item.Record;
                Application.Current.MainPage.DisplayAlert("Info", AppResource.Date + ": " + rec.DateString+"\n"+AppResource.From+": "+rec.WorkTimeFrom.ToString()+"\n"+AppResource.To+": "+rec.WorkTimeTo+"\n"+AppResource.HoursAndMinutes+": " + rec.Time+"\n"+AppResource.BreakTime+": "+rec.BreakTime +"\n"+AppResource.PricePerHour+": "+rec.Price+ "\n"+AppResource.Bonus+": " + rec.Bonus+ "\n"+AppResource.TotalPrice+": " + rec.TotalPrice+ "\n"+AppResource.OverTime+": " + rec.OverTime + "\n"+AppResource.Description+":\n" + rec.Description, AppResource.Ok);
            }
            else if (item.Record.Type == ERecordType.Pieces)
            {
                IPiecesRecord rec = (IPiecesRecord)item.Record;
                Application.Current.MainPage.DisplayAlert("Info", AppResource.Date + ": " + rec.DateString + "\n"+AppResource.Pieces+": " + rec.Pieces+ "\n"+AppResource.PricePerPiece+": " + rec.Price + "\nBonus: " + rec.Bonus + "\n" + AppResource.TotalPrice + ": " + rec.TotalPrice + "\n" + AppResource.Description + ":\n" + rec.Description, AppResource.Ok);
            }
        }

        private void ModifyRecord(TableItemUcVm item)
        {
            MessagingCenter.Send(item, "ModifyItem");
            ((MasterDetailPage) Application.Current.MainPage).Detail = MainPage.AddTab;
        }

        private async void RemoveRecord(TableItemUcVm item)
        {
            if (await Application.Current.MainPage.DisplayAlert(AppResource.DialogRemoveTitle, AppResource.DialogRemoveMessage, AppResource.Yes, AppResource.No))
            {
                MessagingCenter.Send(new TableItemUcVm(new NoneRecord()), "ModifyItem");
                MessagingCenter.Send(item, "Remove");
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