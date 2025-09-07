using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;
using PosSystem.App.Helpers;
using PosSystem.App.Interfaces;
using PosSystem.App.Views;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PosSystem.App.ViewModels
{
    class ProductViewModel : BaseViewModel
    {
        public ViewBindToModel ViewBindToModel { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand RefreshProductCommand { get; set; }

        public ProductViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            ViewBindToModel = new ViewBindToModel();
            AddProductCommand = new RelayCommand(OpenAddProductWindow);
            EditProductCommand = new RelayCommand(EditProduct);
            DeleteProductCommand = new RelayCommand(DeleteProduct);
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
        private void EditProduct(object parameter)
        {
            Product productToEdit = parameter as Product ?? ViewBindToModel.SelectedProduct;
            if (productToEdit != null)
            {
                var editProductWindow = new EditProductWindowView()
                {
                    Owner = App.Current.MainWindow
                };

                var viewModel = new EditProductWindowViewModel(productToEdit);
                viewModel.CloseAction = () =>
                {
                    editProductWindow.Close();
                    RefreshProducts(null);
                };
                editProductWindow.DataContext = viewModel;
                editProductWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a product first.", "Info",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void DeleteProduct(object parameter)
        {

            Product productToDelete = parameter as Product ?? ViewBindToModel.SelectedProduct;

            if (productToDelete == null)
            {
                MessageBox.Show("Please select a product first.", "Info",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{productToDelete.Name}'?",
                                       "Confirm Delete",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var context = new AppDBContext();
                context.Products.Remove(productToDelete);
                context.SaveChanges();
                RefreshProducts(null);
            }
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
        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                NotifyPropertyChanged();
            }
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
