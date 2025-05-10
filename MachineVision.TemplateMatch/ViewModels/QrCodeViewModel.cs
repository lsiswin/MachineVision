using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using HalconDotNet;
using MachineVision.Core;
using MachineVision.Core.TemplateMatch.QcrCodeModel;
using MachineVision.Core.TemplateMatch.Shard;
using MachineVision.shard.Controls;
using Microsoft.Win32;

namespace MachineVision.TemplateMatch.ViewModels
{
    /// <summary>
    /// 二维码视图模型
    /// </summary>
    public class QrCodeViewModel:NavigationViewModel
    {

        public QrCodeService MatchService { get; }
        public DelegateCommand LoadImageCommand { get; private set; }
        public DelegateCommand SetRangeCommand { get; private set; }
        public DelegateCommand RunCommand { get; private set; }

        public QrCodeViewModel(QrCodeService codeService)
        {
            MatchService = codeService;
            LoadImageCommand = new DelegateCommand(LoadImage);
            SetRangeCommand = new DelegateCommand(SetRange);
            RunCommand = new DelegateCommand(Run);
            OcrMatchResult = new OcrMatchResult();
            DrawingObjectList = new ObservableCollection<DrawingObjectInfo>();
        }

        

        /// <summary>
        /// 掩膜
        /// </summary>
        private HObject maskObject;
        private HObject image;
        private OcrMatchResult ocrMatchResult;

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
        /// 绘制集合
        /// </summary>
        public OcrMatchResult OcrMatchResult
        {
            get { return ocrMatchResult; }
            set
            {
                ocrMatchResult = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Run()
        {
            OcrMatchResult = MatchService.Run(Image);
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
                OcrMatchResult.Message = $"{DateTime.Now}:设置ROI范围成功!";
            }
            else
            {
                OcrMatchResult.Message = $"{DateTime.Now}:请先选择ROI范围!";
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

    }
}
