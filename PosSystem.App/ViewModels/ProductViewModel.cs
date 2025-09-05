using PosSystem.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using PosSystem.App.Helpers;

namespace PosSystem.App.ViewModels
{
    class ProductViewModel : BaseViewModel
    {
        public ViewBindToModel ViewBindToModel { get; set; }
        public ProductViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            ViewBindToModel = new ViewBindToModel();
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
        private void LoadProductsFromDatabase()
        {
            using (var context = new AppDBContext())
            {
                Products = new ObservableCollection<Product>(context.Products.ToList());
            }
        }

    }
}
