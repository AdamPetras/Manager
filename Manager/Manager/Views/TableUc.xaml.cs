using System;
using Manager.ViewModels;

namespace Manager.Views
{
    public partial class TableUc
    {
        private TableUcVm _context;
        public TableUc()
        {
            InitializeComponent();
            _context = new TableUcVm();
            BindingContext = _context;
        }

        private void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            _context.ClearAndWriteStatistics();
        }

        protected override void OnAppearing()
        {
            _context.ClearAndWriteStatistics();
        }

        private void FindByDate(object sender, EventArgs e)
        {
            _context.FindByDate();
        }
    }
}