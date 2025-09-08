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
        public ICommand SearchProductCommand { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                NotifyPropertyChanged();
            }
        }

        public ProductViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            ViewBindToModel = new ViewBindToModel();
            AddProductCommand = new RelayCommand(OpenAddProductWindow);
            EditProductCommand = new RelayCommand(EditProduct);
            DeleteProductCommand = new RelayCommand(DeleteProduct);
            RefreshProductCommand = new RelayCommand(RefreshProducts);
            SearchProductCommand = new RelayCommand(SearchProducts);
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
        private void SearchProducts(object parameter)
        {
            ViewBindToModel.SearchProducts(SearchText);
        }
    }
    public class ViewBindToModel : ChangeNotifier
    {
        private ObservableCollection<Product> _allProducts;
        private ObservableCollection<Product> _limitedProducts;
        private ObservableCollection<Product> _filteredProducts;
        private Product _selectedProduct;

        public ObservableCollection<Product> AllProducts
        {
            get => _allProducts;
            set { _allProducts = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<Product> LimitedProducts
        {
            get => _limitedProducts;
            set { _limitedProducts = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; NotifyPropertyChanged(); }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                NotifyPropertyChanged();
                Debug.WriteLine($"SelectedProduct set to: {value?.Name ?? "null"}");
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
                AllProducts = new ObservableCollection<Product>(context.Products.ToList());
                LimitedProducts = new ObservableCollection<Product>(AllProducts.Take(20));
                FilteredProducts = new ObservableCollection<Product>(AllProducts);
            }
        }

        public void DeleteProduct(Product product)
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
                LimitedProducts = new ObservableCollection<Product>(AllProducts.Take(20));
                FilteredProducts = new ObservableCollection<Product>(AllProducts);
                return;
            }
            else
            {
                var filtered = AllProducts.Where(p => p.Name != null && p.Name.Contains(
                    searchText, StringComparison.OrdinalIgnoreCase)).ToList();

                FilteredProducts = new ObservableCollection<Product>(filtered);
                LimitedProducts = new ObservableCollection<Product>(filtered.Take(20));
            }
        }
    }

}
