using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineVision.Core;
using MachineVision.Extensions;
using MachineVision.Models;
using MachineVision.Services;
using MachineVision.shard.Events;

namespace MachineVision.ViewModels
{
    public class MainViewModel : NavigationViewModel
    {
        public MainViewModel(ISettingService settingService, INavigationMenuService menuService, IRegionManager regionManager,IEventAggregator eventAggregator)
        {
            this.settingService = settingService;
            this.NavigationService = menuService;
            this.regionManager = regionManager;
            eventAggregator.GetEvent<LanguageEventBus>().Subscribe(LanguageChanged);
            NavigateCommand = new DelegateCommand<NavigationItem>(Navigate);
        }

        public INavigationMenuService NavigationService { get; }
        public DelegateCommand<NavigationItem> NavigateCommand { get; private set; }

        private bool isTopDrawerOpen;
        private readonly ISettingService settingService;
        private readonly IRegionManager regionManager;
        public bool IsTopDrawerOpen
        {
            get { return isTopDrawerOpen; }
            set
            {
                isTopDrawerOpen = value;
                RaisePropertyChanged();
            }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavigationService.InitMenu();
            NavigationPage("DashboardView");
            await ApplySettingAsync();
            NavigationService.RefreshMenus();
            base.OnNavigatedTo(navigationContext);
        }

        private void NavigationPage(string PageName)
        {
            regionManager
                .Regions["MainViewRegion"]
                .RequestNavigate(
                    PageName,
                    back =>
                    {
                        if (!back.Success)
                        {
                            System.Diagnostics.Debug.WriteLine(back.Exception.Message);
                        }
                    }
                );
        }

        private void Navigate(NavigationItem item)
        {
            if (item == null)
                return;
            if (item.Name.Equals("全部"))
            {
                IsTopDrawerOpen = true;
                return;
            }
            IsTopDrawerOpen = false;
            NavigationPage(item.PageName);
        }
        private void LanguageChanged(bool status)
        {
            NavigationService.RefreshMenus();
        }
        /// <summary>
        /// 初始化语言和主题
        /// </summary>
        private async Task ApplySettingAsync()
        {
            var setting = await settingService.GetSettingAsync();
            if (setting!=null)
            {
                LanguageHelper.SetLanguage(setting.Language);
            }
        }
    }
}
