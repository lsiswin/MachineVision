using System.Configuration;
using System.Data;
using System.Windows;
using MachineVision.Core.TemplateMatch;
using MachineVision.Core.TemplateMatch.CircleModel;
using MachineVision.Core.TemplateMatch.NccModel;
using MachineVision.Core.TemplateMatch.QcrCodeModel;
using MachineVision.Core.TemplateMatch.ShapeModel;
using MachineVision.Services;
using MachineVision.TemplateMatch;
using MachineVision.TemplateMatch.ViewModels;
using MachineVision.TemplateMatch.Views;
using MachineVision.ViewModels;
using MachineVision.Views;

namespace MachineVision
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => null;

        protected override void OnInitialized()
        {
            //从容器当中获取MainView的实例对象
            var container = ContainerLocator.Container;
            var shell = container.Resolve<MainView>("MainView");
            if (shell is Window view)
            {
                //更新Prism注册区域信息
                var regionManager = container.Resolve<IRegionManager>();
                RegionManager.SetRegionManager(view, regionManager);
                RegionManager.UpdateRegions();

                //调用首页的INavigationAware 接口做一个初始化操作
                if (view.DataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(null);
                    //呈现首页
                    App.Current.MainWindow = view;
                }
            }
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry services)
        {
            


            services.RegisterForNavigation<DashboardView, DashboardViewModel>();
            services.RegisterForNavigation<MainView, MainViewModel>();
            services.RegisterForNavigation<SettingView,SettingViewModel>();
            services.RegisterForNavigation<ShapeView, ShapeViewModel>();
            services.RegisterForNavigation<NccView, NccViewModel>();
            services.RegisterForNavigation<QrCodeView, QrCodeViewModel>();
            services.RegisterForNavigation<BarCodeView, BarCodeViewModel>();
            services.RegisterForNavigation<CircleMeasureView, CircleMeasureViewModel>();

            services.Register<ISettingService, SettingService>();
            services.RegisterScoped<INavigationMenuService, NavigationMenuService>();
            services.Register<ITemplateMatchService,ShapeModelService>(nameof(TemplateMatchType.ShapeModel));
            services.Register<ITemplateMatchService, NccModelService>(nameof(TemplateMatchType.NccModel));
            services.Register<BarCodeService>();
            services.Register<QrCodeService>();
            services.Register<CircleService>();

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //注册模块
            moduleCatalog.AddModule<TemplateMatchModel>();
            base.ConfigureModuleCatalog(moduleCatalog);
        }
    }

}
