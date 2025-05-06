using System.Collections.ObjectModel;
using MachineVision.Core;
using MachineVision.Core.TemplateMatch;

namespace MachineVision.TemplateMatch.ViewModels
{
    public class ShapeViewModel:NavigationViewModel
    {
        public ITemplateMatchService MatchService { get; set; }
        public ShapeViewModel()
        {
            this.MatchService =  ContainerLocator.Current.Resolve<ITemplateMatchService>(nameof(TemplateMatchType.ShapeModel));
            LoadImageCommand = new DelegateCommand(LoadImage);
            SetRangeCommand = new DelegateCommand(SetRange);
            CreateTemplateCommand = new DelegateCommand(CreateTemplate);
            RunCommand = new DelegateCommand(Run);
            MatchResults = new ObservableCollection<TemplateMatchResult>();
        }


        private ObservableCollection<TemplateMatchResult> matchResults;
        /// <summary>
        /// 匹配结果集合
        /// </summary>
        public ObservableCollection<TemplateMatchResult> MatchResults
        {
            get { return matchResults; }
            set { matchResults = value; RaisePropertyChanged(); }
        }



        #region 命令实现方法
        /// <summary>
        /// 执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Run()
        {
            
        }
        /// <summary>
        /// 创建匹配模板
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateTemplate()
        {
            
        }
        /// <summary>
        /// 设置识别ROI范围
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void SetRange()
        {
            
        }
        /// <summary>
        /// 加载图像源
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadImage()
        {

        }
        #endregion
        #region 命令
        public DelegateCommand LoadImageCommand { get; private set; }
        public DelegateCommand SetRangeCommand { get; private set; }
        public DelegateCommand CreateTemplateCommand { get; private set; }
        public DelegateCommand RunCommand { get; private set; }
        #endregion



    }
}
