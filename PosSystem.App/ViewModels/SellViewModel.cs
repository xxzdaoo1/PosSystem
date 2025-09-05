using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosSystem.App.Interfaces;

namespace PosSystem.App.ViewModels
{
    class SellViewModel : BaseViewModel
    {
        public SellViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
        }
    }
}
