using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MachineVision.Core;
using MachineVision.Extensions;
using MachineVision.Models;
using MachineVision.Services;
using MachineVision.shard.Events;
using MaterialDesignThemes.Wpf;
using SqlSugar.Extensions;

namespace MachineVision.ViewModels
{
    public class MainViewModel : NavigationViewModel
    {
        public MainViewModel(
            ISettingService settingService,
            INavigationMenuService menuService,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
        )
        {
            this.settingService = settingService;
            this.NavigationService = menuService;
            this.regionManager = regionManager;
            eventAggregator.GetEvent<LanguageEventBus>().Subscribe(LanguageChanged);
            NavigateCommand = new DelegateCommand<NavigationItem>(Navigate);
            GoHomeCommand = new DelegateCommand(() =>
            {
                NavigationPage("DashboardView");
            });
        }
        private object _selectedNavItem;
        public object SelectedNavItem
        {
            get => _selectedNavItem;
            set => SetProperty(ref _selectedNavItem, value);
        }

        public INavigationMenuService NavigationService { get; }
        public DelegateCommand<NavigationItem> NavigateCommand { get; private set; }
        public DelegateCommand GoHomeCommand { get; private set; }
        private readonly ISettingService settingService;
        private readonly IRegionManager regionManager;

        private bool _isTopDrawerOpen;

        public bool IsTopDrawerOpen
        {
            get { return _isTopDrawerOpen; }
            set { _isTopDrawerOpen = value;  if (!value) { SelectedNavItem = null; }  RaisePropertyChanged(); }
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
            SelectedItem = null;
            IsTopDrawerOpen = false; 
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
        private object selectedItem;

        public object SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; RaisePropertyChanged(); }
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
            if (setting != null)
            {
                LanguageHelper.SetLanguage(setting.Language);
            }
            SettingViewModel.ModifyTheme(theme =>
                theme.SetBaseTheme(setting.SkinName.ObjToBool() ? BaseTheme.Dark : BaseTheme.Light)
            );
        }
    }
}
