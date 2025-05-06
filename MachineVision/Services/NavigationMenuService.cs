using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MachineVision.Extensions;
using MachineVision.Models;

namespace MachineVision.Services
{
    public class NavigationMenuService : BindableBase, INavigationMenuService
    {
        public NavigationMenuService()
        {
            Items = new ObservableCollection<NavigationItem>();
        }

        private ObservableCollection<NavigationItem> items;

        public ObservableCollection<NavigationItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                RaisePropertyChanged();
            }
        }

        public void InitMenu()
        {
            Items.Clear();
            Items.Add(
                new NavigationItem(
                    "ExpandAll",
                    "All",
                    "全部",
                    "DashboardView",
                    new ObservableCollection<NavigationItem>()
                    {
                        new NavigationItem(
                            "ApplicationExport",
                            "TemplateMatch",
                            "模板匹配",
                            "",
                            new ObservableCollection<NavigationItem>()
                            {
                                new NavigationItem(
                                    "ShapeOutline",
                                    "ShapeMatch",
                                    "形状匹配",
                                    "ShapeView"
                                ),
                                new NavigationItem("Clouds", "NccMacth", "相似性匹配", "NccView"),
                                new NavigationItem(
                                    "ShapeOvalPlus",
                                    "DeformationMatch",
                                    "形变匹配",
                                    "LocalDeformableView"
                                ),
                            }
                        ),
                        new NavigationItem(
                            "ArchiveClock",
                            "Measure",
                            "比较测量",
                            "",
                            new ObservableCollection<NavigationItem>()
                            {
                                new NavigationItem(
                                    "Circle",
                                    "Caliper",
                                    "卡尺找圆",
                                    "CircleMeasureView"
                                ),
                            }
                        ),
                        new NavigationItem(
                            "ArrowDownBoldHexagonOutline",
                            "Character",
                            "字符识别",
                            "",
                            new ObservableCollection<NavigationItem>()
                            {
                                new NavigationItem(
                                    "Barcode",
                                    "BarCode",
                                    "一维码识别",
                                    "BarCodeView"
                                ),
                                new NavigationItem("Qrcode", "QrCode", "二维码识别", "QrCodeView"),
                            }
                        ),
                    }
                )
            );
            Items.Add(
                new NavigationItem(
                    "ApplicationExport",
                    "TemplateMatch",
                    "模板匹配",
                    "",
                    new ObservableCollection<NavigationItem>()
                    {
                        new NavigationItem(
                            "ShapeOutline",
                            "ShapeMatch",
                            "形状匹配",
                            "ShapeView"
                        ),
                        new NavigationItem("Clouds", "NccMacth", "相似性匹配", "NccView"),
                        new NavigationItem(
                            "ShapeOvalPlus",
                            "DeformationMatch",
                            "形变匹配",
                            "LocalDeformableView"
                        ),
                    }
                )
            );
            Items.Add(
                new NavigationItem(
                    "ArchiveClock",
                    "Measure",
                    "比较测量",
                    "",
                    new ObservableCollection<NavigationItem>()
                    {
                        new NavigationItem("Circle", "Caliper", "卡尺找圆", "CircleMeasureView"),
                    }
                )
            );
            Items.Add(
                new NavigationItem(
                    "ArrowDownBoldHexagonOutline",
                    "Character",
                    "字符识别",
                    "",
                    new ObservableCollection<NavigationItem>()
                    {
                        new NavigationItem("Barcode", "BarCode", "一维码识别", "BarCodeView"),
                        new NavigationItem("Qrcode", "QrCode", "二维码识别", "QrCodeView"),
                    }
                )
            );
            Items.Add(new NavigationItem("Doc", "Doc", "学习文档", "DocView"));
            Items.Add(new NavigationItem("Cog", "Setting", "系统设置", "SettingView"));
        }

        public void RefreshMenus()
        {
            foreach (var item in Items)
            {
                item.Name = LanguageHelper.KeyValues[item.Key];
                if (item.Items != null && item.Items.Count > 0)
                {
                    foreach (var subItem in item.Items)
                    {
                        subItem.Name = LanguageHelper.KeyValues[subItem.Key];
                        if (subItem.Items != null&&subItem.Items.Count>0)
                        {
                            foreach (var other in subItem.Items)
                            {
                                other.Name = LanguageHelper.KeyValues[other.Key];
                            }
                        }
                    }
                }
            }
        }
    }
}
