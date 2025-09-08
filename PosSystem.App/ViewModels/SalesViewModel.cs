using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosSystem.App.Interfaces;

namespace PosSystem.App.ViewModels
{
    class SalesViewModel : BaseViewModel
    {
        public SalesViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
        }
    }
}
