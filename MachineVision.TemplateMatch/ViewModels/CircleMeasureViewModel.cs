using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using HalconDotNet;
using MachineVision.Core;
using MachineVision.Core.TemplateMatch.CircleModel;
using MachineVision.shard.Controls;
using Microsoft.Win32;

namespace MachineVision.TemplateMatch.ViewModels
{
    public class CircleMeasureViewModel:NavigationViewModel
    {
        public CircleService MatchService { get; private set; }
        public DelegateCommand RunCommand { get; private set; }
        public DelegateCommand LoadImageCommand { get; private set; }
        public DelegateCommand GetParameterCommand { get; private set; }
        public CircleMeasureViewModel(CircleService circleService) {
            MatchService = circleService;
            RunCommand = new DelegateCommand(Run);
            LoadImageCommand = new DelegateCommand(LoadImage);
            GetParameterCommand = new DelegateCommand(GetParameter);
            DrawingObjectList = new ObservableCollection<DrawingObjectInfo>();
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

        private void GetParameter()
        {
            var obj = DrawingObjectList.FirstOrDefault(t => t.ShapType == ShapeType.Circle);
            if (obj!=null)
            {
                MatchService.RunParameter.Row = obj.HTuple[0];
                MatchService.RunParameter.Column = obj.HTuple[1];
                MatchService.RunParameter.Radius = obj.HTuple[2];
            }
        }

        public void Run()
        {
            MatchService.Run(Image);
        }
        private HObject image;
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

    }
}
