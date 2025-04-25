using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineVision.Core;
using MachineVision.Services;

namespace MachineVision.ViewModels
{
    public class DashboardViewModel:NavigationViewModel
    {
        public INavigationMenuService NavigationService { get; }

        public DashboardViewModel(INavigationMenuService navigationService)
        {
            this.NavigationService = navigationService;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
        }
    }
}
