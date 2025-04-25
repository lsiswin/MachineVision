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
    public class MainViewModel : NavigationViewModel
    {
        public MainViewModel(INavigationMenuService menuService,IRegionManager regionManager)
        {
            this.NavigationService = menuService;
            this.regionManager = regionManager;
            NavigateCommand = new DelegateCommand<NavigationItem>(Navigate);
        }
        public INavigationMenuService NavigationService { get; }
        public DelegateCommand<NavigationItem> NavigateCommand { get; private set; }

        private bool isTopDrawerOpen;
        private readonly IRegionManager regionManager;

        public bool IsTopDrawerOpen
        {
            get { return isTopDrawerOpen; }
            set { isTopDrawerOpen = value;RaisePropertyChanged(); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavigationService.InitMenu();
            NavigationPage("DashboardView");
            base.OnNavigatedTo(navigationContext);
        }

        private void NavigationPage(string PageName)
        {
            regionManager.Regions["MainViewRegion"].RequestNavigate(PageName, back =>
            {
                if (!back.Success)
                {
                    System.Diagnostics.Debug.WriteLine(back.Exception.Message);
                }
            });
        }

        private void Navigate(NavigationItem item)
        {
            if (item == null) return;
            if (item.Name.Equals("全部"))
            {
                IsTopDrawerOpen = true;
                return;
            }
            IsTopDrawerOpen = false;
        }

        
    }
}
