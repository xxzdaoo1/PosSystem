using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PosSystem.App.Helpers;
using PosSystem.Core.Models;
using PosSystem.Data.Context;

namespace PosSystem.App.ViewModels
{
    class EditProductWindowViewModel : ChangeNotifier
    {
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

        public ICommand SaveEditProductCommand { get; set; }
        public ICommand CloseEditProductCommand { get; set; }
        public Action CloseAction { get; set; }
        public Product Product { get; set; }
        public EditProductWindowViewModel(Product product)
        {
            Product = product;
            SaveEditProductCommand = new RelayCommand(SaveEdit);
            CloseEditProductCommand = new RelayCommand(CloseWindow);
        }

        private void SaveEdit(object parameter)
        {
            if (!CanSaveEdit())
            {
                MessageBox.Show("Please fill in all fields correctly!", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                using (var context  = new AppDBContext())
                {
                    var productToUpdate = context.Products.Find(Product.ProductID);
                    if (productToUpdate != null)
                    {
                        productToUpdate.Barcode = Barcode;
                        productToUpdate.Name = Name;
                        productToUpdate.Price = Price;
                        productToUpdate.StockQuantity = StockQuantity;
                        context.SaveChanges();
                        CloseAction?.Invoke();
                    }
                    else
                    {
                        MessageBox.Show("Product not found in the database.", "Error",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the product: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanSaveEdit()
        {
            return  !string.IsNullOrWhiteSpace(Barcode) &&
                    !string.IsNullOrWhiteSpace(Name) &&
                    Price > 0 &&
                    StockQuantity >= 0; ;
        }
        private void CloseWindow(object parameter)
        {
            CloseAction?.Invoke();
        }
    }
}
