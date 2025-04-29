using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineVision.Core;
using MachineVision.Models;
using MachineVision.Services;

namespace MachineVision.ViewModels
{
    public class DashboardViewModel:NavigationViewModel
    {
        private readonly IRegionManager regionManager;

        public INavigationMenuService NavigationService { get; }

        public DashboardViewModel(INavigationMenuService navigationService,IRegionManager regionManager)
        {
            this.NavigationService = navigationService;
            this.regionManager = regionManager;
            OpenPageCommand = new DelegateCommand<NavigationItem>(OpenPage);
        }        
             
        public DelegateCommand<NavigationItem> OpenPageCommand { get; private set; }

        private void OpenPage(NavigationItem item)
        {
            regionManager.Regions["MainViewRegion"].RequestNavigate(item.PageName);
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
        }
    }
}
