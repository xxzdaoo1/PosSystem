using PosSystem.App.Helpers;
using PosSystem.App.Interfaces;
using PosSystem.App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PosSystem.App.ViewModels
{
    class MainWindowViewModel : ChangeNotifier, IChangeViewModel
    {
        BaseViewModel _currentViewModel;
        Stack<BaseViewModel> _viewModels;

        public MainWindowViewModel()
        {
            _viewModels = new Stack<BaseViewModel>();
            var initialViewModel = new DashboardViewModel(this);
            _viewModels.Push(initialViewModel);
            CurrentViewModel = initialViewModel;
        }

        public BaseViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set { _currentViewModel = value; NotifyPropertyChanged(); }
        }

        // Add page navigation command here
        #region Navigation Commands
        public ICommand GoDashboardCommand => new RelayCommand(GoDashboard);
        public ICommand GoProductCommand => new RelayCommand(GoProduct);
        public ICommand GoSalesCommand => new RelayCommand(GoSales);
        public ICommand GoInventoryCommand => new RelayCommand(GoInventory);

        private void GoDashboard()
        {
            var dashboardVm = new DashboardViewModel(this);
            _viewModels.Clear();
            _viewModels.Push(dashboardVm);
            CurrentViewModel = dashboardVm;
        }
        private void GoProduct()
        {
            var productVm = new ProductViewModel(this);
            _viewModels.Clear();
            _viewModels.Push(productVm);
            CurrentViewModel = productVm;
        }
        private void GoSales()
        {
            var salesVm = new SalesViewModel(this);
            _viewModels.Clear();
            _viewModels.Push(salesVm);
            CurrentViewModel = salesVm;
        }
        private void GoInventory()
        {
            var inventoryVm = new InventoryViewModel(this);
            _viewModels.Clear();
            _viewModels.Push(inventoryVm);
            CurrentViewModel = inventoryVm;
        }
        #endregion

        #region IChangeViewModel

        public void PushViewModel(BaseViewModel model)
        {
            _viewModels.Push(model);
            CurrentViewModel = model;
        }

        public void PopViewModel()
        {
            if (_viewModels.Count > 1)
            {
                _viewModels.Pop();
                CurrentViewModel = _viewModels.Peek();
            }
        }

        #endregion
    }
}
