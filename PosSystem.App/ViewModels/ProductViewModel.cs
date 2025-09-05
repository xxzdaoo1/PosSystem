using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PosSystem.App.Helpers;
using PosSystem.App.Interfaces;
using PosSystem.App.Views;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PosSystem.App.ViewModels
{
    class ProductViewModel : BaseViewModel
    {
        public ViewBindToModel ViewBindToModel { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand RefreshProductCommand { get; set; }

        public ProductViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            ViewBindToModel = new ViewBindToModel();
            AddProductCommand = new RelayCommand(OpenAddProductWindow);
            RefreshProductCommand = new RelayCommand(RefreshProducts);
        }

        private void OpenAddProductWindow(object parameter)
        {
            var addProductWindow = new AddProductWindowView()
            {
                Owner = App.Current.MainWindow
            };

            var viewModel = new AddProductWindowViewModel();
            viewModel.CloseAction = () =>
            {
                addProductWindow.Close();
                RefreshProducts(null);
            };

            addProductWindow.DataContext = viewModel;
            addProductWindow.ShowDialog();
        }
        private void RefreshProducts(object parameter)
        {
            ViewBindToModel.LoadProductsFromDatabase();
        }
    }
    public class ViewBindToModel : ChangeNotifier
    {   
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; NotifyPropertyChanged(); }
        }

        public ViewBindToModel()
        {
            LoadProductsFromDatabase();
        }
        public void LoadProductsFromDatabase()
        {
            using (var context = new AppDBContext())
            {
                Products = new ObservableCollection<Product>(context.Products.ToList());
            }
        }

    }

}
