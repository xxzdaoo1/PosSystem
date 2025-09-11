using PosSystem.App.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PosSystem.Data.Context;
using System.Collections.ObjectModel;
using PosSystem.Core.Models;

namespace PosSystem.App.ViewModels
{
    internal class ReceiveStockWindowViewModel : ChangeNotifier
    {   
        private readonly AppDBContext _context;
        private ObservableCollection<ProductModel> _allProducts;
        private ProductModel _selectedProduct;
        private int _quantity;
        private decimal _unitCost;
        private string _supplier;
        private string _batchNumber;
        private string _notes;

        public ObservableCollection<ProductModel> AllProducts
        {
            get => _allProducts;
            set { _allProducts = value; NotifyPropertyChanged(); }
        }
        public ProductModel SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; NotifyPropertyChanged(); UpdateUnitCost(); }
        }
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(TotalCost)); }
        }
        public decimal UnitCost
        {
            get => _unitCost;
            set { _unitCost = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(TotalCost)); }
        }
        public decimal TotalCost
        {
            get => Quantity * UnitCost;
            set { NotifyPropertyChanged(); }
        }
        public string Supplier
        {
            get => _supplier;
            set { _supplier = value; NotifyPropertyChanged(); }
        }
        public string BatchNumber
        {
            get => _batchNumber;
            set { _batchNumber = value; NotifyPropertyChanged(); }
        }
        public string Notes
        {
            get => _notes;
            set { _notes = value; NotifyPropertyChanged(); }
        }

        public ICommand ReceiveStockCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public Action CloseAction { get; set; }

        public ReceiveStockWindowViewModel()
        {   
            _context = new AppDBContext();
            LoadProducts();

            CloseCommand = new RelayCommand(CloseWindow);
            ReceiveStockCommand = new RelayCommand(ReceiveStock);
        }

        public void LoadProducts()
        {   
            try
            {
                AllProducts = new ObservableCollection<ProductModel>(_context.Products.ToList());
                NotifyPropertyChanged();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ReceiveStock()
        {
            if (SelectedProduct != null)
            {
                if (CanReceiveStock())
                {
                    try
                    {
                        SelectedProduct.StockQuantity += Quantity;
                        SelectedProduct.LastStockUpdate = DateTime.Now;
                        _context.Products.Update(SelectedProduct);

                        var stockReceipt = new StockReceiptModel
                        {
                            ProductID = SelectedProduct.ProductID,
                            QuantityReceived = Quantity,
                            UnitCost = UnitCost,
                            TotalCost = TotalCost,
                            Supplier = Supplier,
                            BatchNumber = BatchNumber,
                            Notes = Notes,
                            ReceiveDate = DateTime.Now
                        };

                        _context.StockReceipts.Add(stockReceipt);
                        _context.SaveChanges();
                        MessageBox.Show("Stock received successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        CloseAction?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error receiving stock: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in all fields correctly!", "Operation Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please select a product to receive stock.", "No Product Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        public bool CanReceiveStock()
        {
            return SelectedProduct != null && 
                    Quantity > 0 && 
                    UnitCost >= 0 && 
                    !string.IsNullOrWhiteSpace(Supplier);
        }

        public void UpdateUnitCost()
        {
            if (SelectedProduct != null)
            {
                UnitCost = SelectedProduct.Price;
            }
        }

        private void CloseWindow(object parameter)
        {
            CloseAction?.Invoke();
        }

    }
}
