using PosSystem.App.Helpers;
using PosSystem.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using System.Windows;

namespace PosSystem.App.ViewModels
{
    class SalesViewModel : BaseViewModel
    {   
        public viewBindToSalesModel ViewBindToSalesModel { get; set; }
        public ICommand AddToCartCommand { get; set; }
        public ICommand RemoveFromCartCommand { get; set; }
        public ICommand ProcessPaymentCommand { get; set; }
        public ICommand CancelSaleCommand { get; set; }
        public ICommand IncreaseQuantityCommand { get; set; }
        public ICommand DecreaseQuantityCommand { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                NotifyPropertyChanged();
                ViewBindToSalesModel?.SearchProducts(value);
            }
        }

        public SalesViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {   
            ViewBindToSalesModel = new viewBindToSalesModel();
            AddToCartCommand = new RelayCommand(AddToCart);
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            ProcessPaymentCommand = new RelayCommand(ProcessPayment);
            CancelSaleCommand = new RelayCommand(CancelSale);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);
        }
        private void AddToCart(object parameter)
        {
            if (parameter is ProductModel product)
            {
                ViewBindToSalesModel.AddToCart(product);
            }
            else if (ViewBindToSalesModel.SelectedProduct != null)
            {
                ViewBindToSalesModel.AddToCart(ViewBindToSalesModel.SelectedProduct);
            }
        }
        private void RemoveFromCart(object parameter)
        {
            if (parameter is CartItem cartItem)
            {
                ViewBindToSalesModel.RemoveFromCart(cartItem);
            }
        }
        private void ProcessPayment(object parameter)
        {
            ViewBindToSalesModel.ProcessPayment();
        }
        private void CancelSale(object parameter)
        {
            ViewBindToSalesModel.CancelSale();
        }
        private void IncreaseQuantity(object parameter)
        {
            if (parameter is CartItem cartItem)
            {
                ViewBindToSalesModel.IncreaseQuantity(cartItem);
            }
        }
        private void DecreaseQuantity(object parameter)
        {
            if (parameter is CartItem cartItem)
            {
                ViewBindToSalesModel.DecreaseQuantity(cartItem);
            }
        }
    }
    public class viewBindToSalesModel : ChangeNotifier
    {
        private ObservableCollection<ProductModel> _allproducts;
        private ObservableCollection<ProductModel> _filteredProducts;
        private ObservableCollection<CartItem> _cartItems;
        private ProductModel _selectedProduct;
        private decimal _subtotal;
        private decimal _taxAmount;
        private decimal _totalAmount;

        private readonly AppDBContext _context;
        #region Properties
        public ObservableCollection<ProductModel> AllProducts
        {
            get => _allproducts;
            set { _allproducts = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<ProductModel> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set { _cartItems = value; NotifyPropertyChanged(); }
        }
        public ProductModel SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; NotifyPropertyChanged(); }
        }
        public decimal Subtotal
        {
            get => _subtotal;
            set { _subtotal = value; NotifyPropertyChanged(); }
        }
        public decimal TaxAmount
        {
            get => _taxAmount;
            set { _taxAmount = value; NotifyPropertyChanged(); }
        }
        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; NotifyPropertyChanged(); }
        }
        #endregion
        public viewBindToSalesModel()
        {
            _context = new AppDBContext();
            LoadProductsFromDatabase();
            CartItems = new ObservableCollection<CartItem>();
            CalculateTotals();
        }
        public void LoadProductsFromDatabase()
        {
            try
            {
                AllProducts = new ObservableCollection<ProductModel>(_context.Products
                    .Where(p => p.StockQuantity > 0)
                    .ToList());
                FilteredProducts = new ObservableCollection<ProductModel>(AllProducts);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show($"Error searching products: {ex.Message}");
            }
        }
        public void AddToCart(ProductModel product) 
        {
            if (product == null || product.StockQuantity <= 0)
            {
                MessageBox.Show("Product is out of stock or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingItem = CartItems.FirstOrDefault(item => item.Product.ProductID == product.ProductID);

            if (existingItem != null)
            {
                if (existingItem.Quantity < product.StockQuantity)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    MessageBox.Show("Not enough stock available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    Product = product,
                    Quantity = 1,
                    UnitPrice = product.Price
                });
            }
            CalculateTotals();
        }
        public void RemoveFromCart(CartItem cartItem) 
        { 
            if (cartItem != null)
            {
                CartItems.Remove(cartItem);
                CalculateTotals();
            }
        }
        public void IncreaseQuantity(CartItem cartItem)
        {
            if (cartItem != null && cartItem.Quantity < cartItem.Product.StockQuantity)
            {
                cartItem.Quantity++;
                CalculateTotals();
            }
            else
            {
                MessageBox.Show("Not enough stock available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DecreaseQuantity(CartItem cartItem) 
        { 
            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                CalculateTotals();
            }
            else if (cartItem != null && cartItem.Quantity == 1)
            {
                RemoveFromCart(cartItem);
            }
        }
        public void ProcessPayment() 
        {
            if (!CartItems.Any())
            {
                MessageBox.Show("Cart is empty! Please add items before processing payment.");
                return;
            }
            try
            {
                var sale = new SaleModel
                {
                    SaleDate = DateTime.Now,
                    TotalAmount = TotalAmount,
                    PaymentMethod = "Cash",
                    SaleItems = CartItems.Select(item => new SaleItemModel
                    {
                        ProductID = item.Product.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };

                _context.Sales.Add(sale);

                foreach (var item in CartItems)
                {
                    var product = _context.Products.Find(item.Product.ProductID);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                        if (product.StockQuantity < 0)
                            product.StockQuantity = 0;
                    }
                }

                _context.SaveChanges();

                MessageBox.Show($"Payment processed successfully!\nTotal: {TotalAmount:C}");

                CancelSale();
                LoadProductsFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}");
            }
        }
        public void CancelSale() 
        { 
            CartItems.Clear();
            CalculateTotals();
            SelectedProduct = null;
        }
        public void CalculateTotals() 
        {
            Subtotal = CartItems.Sum(item => item.TotalPrice);
            TaxAmount = Subtotal * 0.08m;
            TotalAmount = Subtotal + TaxAmount;
        }

    }
}
