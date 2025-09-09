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
        public ViewBindToProductModel ViewBindToProductModel { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand RefreshProductCommand { get; set; }
        public ICommand SearchProductCommand { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                NotifyPropertyChanged();
                ViewBindToProductModel.SearchProducts(value);
            }
        }

        public ProductViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            ViewBindToProductModel = new ViewBindToProductModel();
            AddProductCommand = new RelayCommand(OpenAddProductWindow);
            EditProductCommand = new RelayCommand(EditProduct);
            DeleteProductCommand = new RelayCommand(DeleteProduct);
            RefreshProductCommand = new RelayCommand(RefreshProducts);
        }
        #region Command Methods
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
            ProductModel productToEdit = parameter as ProductModel ?? ViewBindToProductModel.SelectedProduct;
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

            ProductModel productToDelete = parameter as ProductModel ?? ViewBindToProductModel.SelectedProduct;

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
            ViewBindToProductModel.LoadProductsFromDatabase();
        }
        #endregion
    }
    public class ViewBindToProductModel : ChangeNotifier
    {
        private ObservableCollection<ProductModel> _allProducts;
        private ObservableCollection<ProductModel> _limitedProducts;
        private ObservableCollection<ProductModel> _filteredProducts;
        private ProductModel _selectedProduct;
        public ObservableCollection<ProductModel> AllProducts
        {
            get => _allProducts;
            set { _allProducts = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<ProductModel> LimitedProducts
        {
            get => _limitedProducts;
            set { _limitedProducts = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<ProductModel> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; NotifyPropertyChanged(); }
        }
        public ProductModel SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                NotifyPropertyChanged();
            }
        }

        public ViewBindToProductModel()
        {
            LoadProductsFromDatabase();
        }
        public void LoadProductsFromDatabase()
        {
            using (var context = new AppDBContext())
            {
                AllProducts = new ObservableCollection<ProductModel>(context.Products.ToList());
                LimitedProducts = new ObservableCollection<ProductModel>(AllProducts.Take(20));
                FilteredProducts = new ObservableCollection<ProductModel>(AllProducts);
            }
        }

        public void DeleteProduct(ProductModel product)
        {
            if (product == null) return;

            try
            {
                using (var context = new AppDBContext())
                {
                    context.Products.Attach(product);
                    context.Products.Remove(product);
                    context.SaveChanges();
                }
                if (FilteredProducts?.Contains(product) == true)
                    FilteredProducts.Remove(product);

                if (LimitedProducts?.Contains(product) == true)
                    LimitedProducts.Remove(product);

                SelectedProduct = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SearchProducts(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LimitedProducts = new ObservableCollection<ProductModel>(AllProducts.Take(20));
                FilteredProducts = new ObservableCollection<ProductModel>(AllProducts);
                return;
            }

            var filtered = AllProducts
                .Where(p => (p.Name != null && p.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                           (p.Barcode != null && p.Barcode.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();

            FilteredProducts = new ObservableCollection<ProductModel>(filtered);
            LimitedProducts = new ObservableCollection<ProductModel>(filtered.Take(20));
        }
    }

}
