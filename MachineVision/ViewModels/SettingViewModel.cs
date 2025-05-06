using System.Collections.ObjectModel;
using System.Windows.Media;
using Dm.util;
using MachineVision.Core;
using MachineVision.Extensions;
using MachineVision.Models;
using MachineVision.Services;
using MachineVision.shard.Events;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;

namespace MachineVision.ViewModels
{
    public class SettingViewModel : NavigationViewModel
    {
        public SettingViewModel(IEventAggregator eventAggregator, ISettingService settingService)
        {
            LanguageInfos = new ObservableCollection<LanguageInfo>();
            InitLanguageInfos();
            this.eventAggregator = eventAggregator;
            this.settingService = settingService;
            SaveCommand = new DelegateCommand(SaveSetting);
            colorInit();
        }

        public DelegateCommand SaveCommand { get; private set; }

        private void SaveSetting()
        {
            Setting.Language = CurrentLanguage.Key;
            Setting.SkinName = IsDarkTheme.toString();
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
            if (LanguageHelper.AppCurrentLanguage == CurrentLanguage.Key)
                return;
            //设置当前语言
            LanguageHelper.SetLanguage(CurrentLanguage.Key);
            //通知所有界面更新语言
            eventAggregator.GetEvent<LanguageEventBus>().Publish(true);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            Setting = await settingService.GetSettingAsync();
            CurrentLanguage = languageInfos.FirstOrDefault(t => t.Key == Setting.Language);
            base.OnNavigatedTo(navigationContext);
        }

        private void InitLanguageInfos()
        {
            LanguageInfos.Add(new LanguageInfo() { Key = "zh-CN", Value = "简体中文" });
            LanguageInfos.Add(new LanguageInfo() { Key = "en-US", Value = "English" });
        }

        private void colorInit()
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();
            if (theme is Theme internalTheme)
            {
                var colorAdjustment = internalTheme.ColorAdjustment ?? new ColorAdjustment();
                _colorSelectionValue = colorAdjustment.Colors;
            }
            IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;
        }

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(theme =>
                    {
                        if (theme is Theme internalTheme)
                        {
                            internalTheme.ColorAdjustment = value
                                ? new ColorAdjustment { Colors = ColorSelectionValue }
                                : null;
                        }
                        theme.SetBaseTheme(value ? BaseTheme.Dark : BaseTheme.Light);
                    });
                }
            }
        }

        public static void ModifyTheme(Action<Theme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }

        public IEnumerable<ColorSelection> ColorSelectionValues =>
            Enum.GetValues(typeof(ColorSelection)).Cast<ColorSelection>();

        private ColorSelection _colorSelectionValue;
        public ColorSelection ColorSelectionValue
        {
            get => _colorSelectionValue;
            set
            {
                if (SetProperty(ref _colorSelectionValue, value))
                {
                    ModifyTheme(theme =>
                    {
                        if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                            internalTheme.ColorAdjustment.Colors = value;
                    });
                }
            }
        }
    }
}
