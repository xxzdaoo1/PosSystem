using System;
using System.Windows;
using System.Windows.Input;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using PosSystem.App.Helpers;

namespace PosSystem.App.ViewModels
{
    class AddProductWindowViewModel : ChangeNotifier
    {
        // Properties for binding
        private string _barcode;
        public string Barcode
        {
            get => _barcode;
            set { _barcode = value; NotifyPropertyChanged(); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; NotifyPropertyChanged(); }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set { _price = value; NotifyPropertyChanged(); }
        }

        private int _stockQuantity;
        public int StockQuantity
        {
            get => _stockQuantity;
            set { _stockQuantity = value; NotifyPropertyChanged(); }
        }

        public ICommand SaveAddProductCommand { get; set; }
        public ICommand CloseAddProductCommand { get; set; }

        public Action CloseAction { get; set; }

        public AddProductWindowViewModel()
        {
            SaveAddProductCommand = new RelayCommand(SaveProduct);
            CloseAddProductCommand = new RelayCommand(CloseWindow);
        }

        private void SaveProduct(object parameter)
        {
            if (!CanSaveProduct())
            {
                MessageBox.Show("Please fill in all fields correctly!", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new AppDBContext())
                {
                    var product = new Product
                    {
                        Barcode = Barcode,
                        Name = Name,
                        Price = Price,
                        StockQuantity = StockQuantity
                    };

                    context.Products.Add(product);
                    context.SaveChanges();
                }

                MessageBox.Show("Product added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveProduct()
        {
            return !string.IsNullOrWhiteSpace(Barcode) &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   Price > 0 &&
                   StockQuantity >= 0;
        }

        private void CloseWindow(object parameter)
        {
            CloseAction?.Invoke();
        }
    }
}