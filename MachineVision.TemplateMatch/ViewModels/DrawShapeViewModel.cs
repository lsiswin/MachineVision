using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using MachineVision.Core;
using MachineVision.shard.Controls;
using Microsoft.Win32;

namespace MachineVision.TemplateMatch.ViewModels
{
    public class DrawShapeViewModel:NavigationViewModel
    {
        public DrawShapeViewModel()
        {
            LoadImageCommand = new DelegateCommand(LoadImage);
            DrawingObjectList = new ObservableCollection<DrawingObjectInfo>();
        }

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
        private ObservableCollection<DrawingObjectInfo> drawingObjectList;

        public ObservableCollection<DrawingObjectInfo> DrawingObjectList
        {
            get { return drawingObjectList; }
            set { drawingObjectList = value; RaisePropertyChanged(); }
        }


        private HObject image;       

        public HObject Image
		{
			get { return image; }
			set { image = value; RaisePropertyChanged(); }
		}

		public DelegateCommand LoadImageCommand { get; private set; }
	}
}
