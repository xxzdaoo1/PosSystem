using PosSystem.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosSystem.App.ViewModels
{
    class DashboardViewModel : BaseViewModel
    {
        public DashboardViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
        }
    }
}
