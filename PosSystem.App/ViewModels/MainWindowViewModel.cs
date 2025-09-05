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

        #region Navigation Commands
        public ICommand GoDashboardCommand => new RelayCommand(GoDashboard);
        public ICommand GoProductCommand => new RelayCommand(GoProduct);

        private void GoDashboard()
        {
            var homeVm = new DashboardViewModel(this);
            _viewModels.Clear();
            _viewModels.Push(homeVm);
            CurrentViewModel = homeVm;
        }

        private void GoProduct()
        {
            var anotherVm = new ProductViewModel(this);
            _viewModels.Push(anotherVm);
            CurrentViewModel = anotherVm;
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
