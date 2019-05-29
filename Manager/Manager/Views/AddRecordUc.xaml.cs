using System;
using System.Collections.ObjectModel;
using Manager.Model.Enums;
using Manager.ViewModels;
using Xamarin.Forms;

namespace Manager.Views
{
    public partial class AddRecordUc:ContentPage
    {
        private AddRecordVm _addRecord;
        public AddRecordUc()
        {
            InitializeComponent();
            _addRecord = new AddRecordVm();
            BindingContext = _addRecord;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _addRecord.ReloadConfigValues();
        }
    }
}