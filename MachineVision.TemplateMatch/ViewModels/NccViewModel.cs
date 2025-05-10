using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using MachineVision.Core;
using MachineVision.Core.TemplateMatch;
using MachineVision.Core.TemplateMatch.NccModel;
using MachineVision.Core.TemplateMatch.Shard;
using MachineVision.shard.Controls;
using Microsoft.Win32;

namespace MachineVision.TemplateMatch.ViewModels
{
    public class NccViewModel : NavigationViewModel
    {
        public NccViewModel()
        {
            this.MatchService = ContainerLocator.Current.Resolve<ITemplateMatchService>(
                nameof(TemplateMatchType.NccModel)
            );
            LoadImageCommand = new DelegateCommand(LoadImage);
            SetRangeCommand = new DelegateCommand(SetRange);
            CreateTemplateCommand = new DelegateCommand(CreateTemplate);
            RunCommand = new DelegateCommand(Run);
            MatchResult = new MatchResult();
            DrawingObjectList = new ObservableCollection<DrawingObjectInfo>();
        }

        public ITemplateMatchService MatchService { get; set; }
        private ObservableCollection<DrawingObjectInfo> drawingObjectList;

        public ObservableCollection<DrawingObjectInfo> DrawingObjectList
        {
            get { return drawingObjectList; }
            set
            {
                drawingObjectList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 掩膜
        /// </summary>
        private HObject maskObject;
        private HObject image;
        private MatchResult matchResult;

        public HObject MaskObject
        {
            get { return maskObject; }
            set
            {
                maskObject = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 图像源
        /// </summary>
        public HObject Image
        {
            get { return image; }
            set
            {
                image = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 绘制集合
        /// </summary>
        public MatchResult MatchResult
        {
            get { return matchResult; }
            set
            {
                matchResult = value;
                RaisePropertyChanged();
            }
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
            if (hobject != null)
            {
                if (MaskObject != null)
                {
                    HOperatorSet.Difference(
                        hobject.HObject,
                        MaskObject,
                        out HObject regionDifference
                    );
                    MatchService.CreateTemplate(Image, regionDifference);
                }
                else
                {
                    MatchService.CreateTemplate(Image, hobject.HObject);
                }
                MatchResult.Message = $"{DateTime.Now}:创建模板成功!";
            }
        }

        /// <summary>
        /// 设置识别ROI范围
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void SetRange()
        {
            var hobject = DrawingObjectList.FirstOrDefault();
            if (hobject != null)
            {
                MatchService.RoiPararmeter = new RoiPararmeter()
                {
                    Row1 = hobject.HTuple[0],
                    Column1 = hobject.HTuple[1],
                    Row2 = hobject.HTuple[2],
                    Column2 = hobject.HTuple[3],
                };
                MatchResult.Message = $"{DateTime.Now}:设置ROI范围成功!";
            }
            else
            {
                MatchResult.Message = $"{DateTime.Now}:请先选择ROI范围!";
            }
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
