using System.Collections.ObjectModel;
using HalconDotNet;
using MachineVision.Core;
using MachineVision.Core.TemplateMatch;
using MachineVision.shard.Controls;
using Microsoft.Win32;

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
            DrawingObjectList = new ObservableCollection<DrawingObjectInfo>();
        }
        private ObservableCollection<DrawingObjectInfo> drawingObjectList;

        public ObservableCollection<DrawingObjectInfo> DrawingObjectList
        {
            get { return drawingObjectList; }
            set { drawingObjectList = value; RaisePropertyChanged(); }
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
        private HObject image;

        public HObject Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

        private MatchResult matchResult;

        public MatchResult MatchResult
        {
            get { return matchResult; }
            set { matchResult = value; RaisePropertyChanged(); }
        }



        #region 命令实现方法
        /// <summary>
        /// 执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Run()
        {
            MatchResult = MatchService.Run(Image);
            MatchResult.Setting = MatchService.Setting;
        }
        /// <summary>
        /// 创建匹配模板
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateTemplate()
        {
            var hobject = DrawingObjectList.FirstOrDefault();
            if(hobject != null)
            {
                MatchService.CreateTemplate(Image,hobject.HObject);
            }           

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
            OpenFileDialog dialog = new OpenFileDialog();
            var dialogResult = (bool)dialog.ShowDialog();
            if (dialogResult)
            {
                var image = new HImage();
                image.ReadImage(dialog.FileName);
                Image = image;

            }
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
