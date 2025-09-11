using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PosSystem.App.Helpers;
using PosSystem.App.Interfaces;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using PosSystem.App.Views;

namespace PosSystem.App.ViewModels
{
    class InventoryViewModel : BaseViewModel
    {   
        public ViewBindToInventoryModel ViewBindToInventoryModel { get; set; }
        public ICommand OpenReceiveStockWindowCommand { get; set; }
        public ICommand AdjustStockCommand { get; set; }
        public ICommand GenerateReportCommand { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                NotifyPropertyChanged();
                ViewBindToInventoryModel?.SearchProducts(value);
            }
        }
        public int TotalProducts => ViewBindToInventoryModel?.TotalProducts ?? 0;
        public int InStockProducts => ViewBindToInventoryModel?.InStockProducts ?? 0;
        public int OutOfStockProducts => ViewBindToInventoryModel?.OutOfStockProducts ?? 0;
        public int LowStockProducts => ViewBindToInventoryModel?.LowStockProducts ?? 0;

        public InventoryViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            ViewBindToInventoryModel = new ViewBindToInventoryModel();

            OpenReceiveStockWindowCommand = new RelayCommand(OpenReceiveStockWindow);
            AdjustStockCommand = new RelayCommand(AdjustStockWindow);
            //GenerateReportCommand = new RelayCommand(GenerateReport);
        }

        private void OpenReceiveStockWindow(object parameter)
        {
            try
            {
                var receiveStockWindow = new ReceiveStockWindowView()
                {
                    Owner = Application.Current.MainWindow
                };
                var viewModel = new ReceiveStockWindowViewModel();

                viewModel.CloseAction = () =>
                {
                    receiveStockWindow.Close();
                    RefreshProducts(null);
                };

                receiveStockWindow.DataContext = viewModel;
                receiveStockWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Receive Stock window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AdjustStockWindow(object parameter)
        {
            //try
            //{
            //    var receiveWindow = new ReceiveStockWindowView();
            //    receiveWindow.Closed += (s, e) => ViewBindToInventoryModel.LoadInventoryData;
            //    receiveWindow.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error opening Receive Stock window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            //{

            //}
        }
        private void RefreshProducts(object parameter)
        {
            ViewBindToInventoryModel.LoadProductsFromDatabase();
        }
    }

    public class ViewBindToInventoryModel : ChangeNotifier
    {
        private readonly AppDBContext _context;

        private ObservableCollection<ProductModel> _allProducts;
        private ObservableCollection<ProductModel> _filteredProducts;
        private int _totalProducts;
        private int _inStockProducts;
        private int _outOfStockProducts;
        private int _lowStockProducts;

        public ObservableCollection<ProductModel> AllProducts
        {
            get => _allProducts;
            set { _allProducts = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<ProductModel> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; NotifyPropertyChanged(); }
        }
        public int TotalProducts
        {
            get => _totalProducts;
            set { _totalProducts = value; NotifyPropertyChanged(); }
        }
        public int InStockProducts
        {
            get => _inStockProducts;
            set { _inStockProducts = value; NotifyPropertyChanged(); }
        }
        public int OutOfStockProducts
        {
            get => _outOfStockProducts;
            set { _outOfStockProducts = value; NotifyPropertyChanged(); }
        }
        public int LowStockProducts
        {
            get => _lowStockProducts;
            set { _lowStockProducts = value; NotifyPropertyChanged(); }
        }

        public ViewBindToInventoryModel()
        {
            _context = new AppDBContext();
            LoadProductsFromDatabase();
            FilteredProducts = new ObservableCollection<ProductModel>(AllProducts);
        }
        public void LoadProductsFromDatabase()
        {
            try
            {
                AllProducts = new ObservableCollection<ProductModel>(_context.Products.ToList());
            }
            catch (Exception ex)
            { 
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadInventoryData();
        }

        public void SearchProducts(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredProducts = new ObservableCollection<ProductModel>(AllProducts);
                return;
            }

            try
            {
                var filtered = AllProducts
                    .Where(p => (p.Name != null &&
                                p.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                               (p.Barcode != null &&
                                p.Barcode.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
                FilteredProducts = new ObservableCollection<ProductModel>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadInventoryData()
        {   
            int productsLowStockThreshold = 10;

            TotalProducts = AllProducts.Count;
            InStockProducts = AllProducts.Count(p => p.StockQuantity > productsLowStockThreshold);
            LowStockProducts = AllProducts.Count(p => p.StockQuantity > 0 && p.StockQuantity <= productsLowStockThreshold);
            OutOfStockProducts = AllProducts.Count(p => p.StockQuantity <= 0);
        }
    }
}
