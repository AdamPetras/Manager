using System;
using System.Collections.ObjectModel;
using Manager.Model.Enums;
using Manager.ViewModels;
using Xamarin.Forms;

namespace Manager.Views
{
    public partial class AddRecordUc:ContentPage
    {
        public AddRecordUc()
        {
            InitializeComponent();
            BindingContext = new AddRecordVm();
        }
    }
}