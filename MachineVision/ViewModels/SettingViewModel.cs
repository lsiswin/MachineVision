using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class SettingViewModel : NavigationViewModel
    {
        public SettingViewModel(IEventAggregator eventAggregator, ISettingService settingService)
        {
            LanguageInfos = new ObservableCollection<LanguageInfo>();
            this.eventAggregator = eventAggregator;
            this.settingService = settingService;
            SaveCommand = new DelegateCommand(SaveSetting);
        }

        public DelegateCommand SaveCommand { get; private set; }

        private void SaveSetting()
        {
            Setting.Language = CurrentLanguage.Key;
            Setting.SkinName = "浅色";
            Setting.SkinColor = "#FF0000";
            settingService.SaveSettingAsync(Setting);
        }

        private ObservableCollection<LanguageInfo> languageInfos;

        public ObservableCollection<LanguageInfo> LanguageInfos
        {
            get { return languageInfos; }
            set
            {
                languageInfos = value;
                RaisePropertyChanged();
            }
        }
        private LanguageInfo currentLanguage = new LanguageInfo();
        public Setting Setting { get; set; }
        private readonly IEventAggregator eventAggregator;
        private readonly ISettingService settingService;

        public LanguageInfo CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                currentLanguage = value;
                LanguageChanged();
                RaisePropertyChanged();
            }
        }

        private void LanguageChanged()
        {
            if (LanguageHelper.AppCurrentLanguage == CurrentLanguage.Key) return;
            //设置当前语言
            LanguageHelper.SetLanguage(CurrentLanguage.Key);
            //通知所有界面更新语言
            eventAggregator.GetEvent<LanguageEventBus>().Publish(true);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            InitLanguageInfos();
            Setting = await settingService.GetSettingAsync();
            CurrentLanguage = languageInfos.FirstOrDefault(t => t.Key == Setting.Language);
            base.OnNavigatedTo(navigationContext);
        }

        private void InitLanguageInfos()
        {
            LanguageInfos.Add(new LanguageInfo() { Key = "zh-CN", Value = "简体中文" });
            LanguageInfos.Add(new LanguageInfo() { Key = "en-US", Value = "English" });
        }
    }
}
