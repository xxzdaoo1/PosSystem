using PosSystem.App.Helpers;
using PosSystem.App.Interfaces;
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

        // Add navigation commands here
        #region Navigation Commands
        public ICommand GoDashboardCommand => new RelayCommand(GoDashboard);
        public ICommand GoProductCommand => new RelayCommand(GoProduct);
        public ICommand GoSellCommand => new RelayCommand(GoSell);
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
        private void GoSell()
        {
            var sellVm = new SellViewModel(this);
            _viewModels.Clear();
            _viewModels.Push(sellVm);
            CurrentViewModel = sellVm;
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
