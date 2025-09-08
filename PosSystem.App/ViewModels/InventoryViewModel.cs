using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosSystem.App.Interfaces;

namespace PosSystem.App.ViewModels
{
    class InventoryViewModel : BaseViewModel
    {
        public InventoryViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
        }
    }
}
