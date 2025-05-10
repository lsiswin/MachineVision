using System.Collections.ObjectModel;
using System.Runtime.Intrinsics.X86;
using System.Windows;
using System.Windows.Controls;
using HalconDotNet;
using MachineVision.Core.Extensions;
using MachineVision.Core.TemplateMatch.Shard;
using MachineVision.shard.Extensions;

namespace MachineVision.shard.Controls
{
    public class ImageEditView : Control
    {
        private HSmartWindowControlWPF HSmart;
        private HWindow hWindow;
        private TextBlock txtMsg;



        public HWindow HWindow
        {
            get { return (HWindow)GetValue(HWindowProperty); }
            set { SetValue(HWindowProperty, value); }
        }
        public static readonly DependencyProperty HWindowProperty =
            DependencyProperty.Register("HWindow", typeof(HWindow), typeof(ImageEditView), new PropertyMetadata(null));



        /// <summary>
        /// 掩膜
        /// </summary>
        public HObject MaskObject
        {
            get { return (HObject)GetValue(MaskObjectProperty); }
            set { SetValue(MaskObjectProperty, value); }
        }

        public static readonly DependencyProperty MaskObjectProperty =
            DependencyProperty.Register("MaskObject", typeof(HObject), typeof(ImageEditView), new PropertyMetadata(null));

        /// <summary>
        /// 绘制形状集合
        /// </summary>
        public ObservableCollection<DrawingObjectInfo> DrawingObjectList
        {
            get
            {
                return (ObservableCollection<DrawingObjectInfo>)GetValue(DrawingObjectListProperty);
            }
            set { SetValue(DrawingObjectListProperty, value); }
        }
        public static readonly DependencyProperty DrawingObjectListProperty =
            DependencyProperty.Register(
                "DrawingObjectList",
                typeof(ObservableCollection<DrawingObjectInfo>),
                typeof(ImageEditView),
                new PropertyMetadata(new ObservableCollection<DrawingObjectInfo>())
            );

        public MatchResult MatchResult
        {
            get
            {
                return (MatchResult)GetValue(MatchResultProperty);
            }
            set { SetValue(MatchResultProperty, value); }
        }

        public static readonly DependencyProperty MatchResultProperty =
            DependencyProperty.Register(
                "MatchResult",
                typeof(MatchResult),
                typeof(ImageEditView),
                new PropertyMetadata(MatchResultCallBack)
            );

        public static void MatchResultCallBack(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d is ImageEditView view && e.NewValue != null)
            {
                view.DisplayMatchRender();
            }
        }

        private void DisplayMatchRender()
        {
            if(Image != null)
                DisPlay(Image);
            if (MatchResult != null&&MatchResult.Results!=null)
            {
                var setting = MatchResult.Setting;
                foreach (var item in MatchResult.Results)
                {
                    if(setting.IsShowCenter)
                        hWindow.DispCross(item.Row, item.Column, 50, item.Angle);
                    if(setting.IsShowDisplayText)
                        hWindow.SetMessage($"({Math.Round(item.Row,2)},{Math.Round(item.Column, 2)})","image",item.Row,item.Column,"black","true");
                    if(setting.IsShowMatchRange)
                        hWindow.DispObj(item.TransContours);    
                    
                }
            }
            
        }

        public HObject Image
        {
            get { return (HObject)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image",
            typeof(HObject),
            typeof(ImageEditView),
            new PropertyMetadata(ImageChangedCallBack)
        );

        public static void ImageChangedCallBack(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d is ImageEditView view && e.NewValue != null)
            {
                view.DisPlay((HObject)e.NewValue);
            }
        }

        private void DisPlay(HObject hObject)
        {
            hWindow.DispObj(hObject);
            hWindow.SetPart(0, 0, -2, -2);
        }

        public override void OnApplyTemplate()
        {
            txtMsg = (TextBlock)GetTemplateChild("PART_MSG");
            if (GetTemplateChild("PART_SMART") is HSmartWindowControlWPF hSmart)
            {
                HSmart = hSmart;
                hSmart.Loaded += HsmartLoad;
            }
            if (GetTemplateChild("PART_Rect") is MenuItem btnRect)
            {
                btnRect.Click += BtnRect_Click;
            }
            if (GetTemplateChild("PART_Ellipse") is MenuItem btnEllipse)
            {
                btnEllipse.Click += BtnEllipse_Click;
            }
            if (GetTemplateChild("PART_Circle") is MenuItem btnCircle)
            {
                btnCircle.Click += BtnCircle_Click;
            }
            if (GetTemplateChild("PART_DrawPen") is MenuItem btnDrawpen)
            {
                btnDrawpen.Click += BtnDrawPen_Click;
            }
            if(GetTemplateChild("PART_Mask")is MenuItem btnMask)
            {
                btnMask.Click += BtnMask_Click;
            }

            if (GetTemplateChild("Clear") is MenuItem btnClear)
            {
                btnClear.Click += (s, e) =>
                {
                    DrawingObjectList.Clear();
                    hWindow.ClearWindow();
                    DisPlay(Image);
                };
            }

            base.OnApplyTemplate();
        }

        private void BtnMask_Click(object sender, RoutedEventArgs e)
        {
            DrawShape(ShapeType.Region);
        }

        private void BtnDrawPen_Click(object sender, RoutedEventArgs e)
        {
            DrawShape(ShapeType.Region);
        }

        private void BtnCircle_Click(object sender, RoutedEventArgs e)
        {
            DrawShape(ShapeType.Circle, new HTuple(), new HTuple(), new HTuple());
        }

        private void BtnEllipse_Click(object sender, RoutedEventArgs e)
        {
            DrawShape(
                ShapeType.Ellipse,
                new HTuple(),
                new HTuple(),
                new HTuple(),
                new HTuple(),
                new HTuple()
            );
        }

        private void BtnRect_Click(object sender, RoutedEventArgs e)
        {
            DrawShape(ShapeType.Rectangle1, new HTuple(), new HTuple(), new HTuple(), new HTuple());
        }

        private async void DrawShape(ShapeType shapeType, params HTuple[] hTuples)
        {
            txtMsg.Text = "按鼠标左键绘制,右键结束";
            HObject hObject;
            HOperatorSet.GenEmptyObj(out hObject);
            HSmart.HZoomContent = HSmartWindowControlWPF.ZoomContent.Off;
            await Task.Run(() =>
            {
                HOperatorSet.SetColor(hWindow, "blue");
                switch (shapeType)
                {
                    case ShapeType.Rectangle1:
                        {
                            HOperatorSet.DrawRectangle1(
                                hWindow,
                                out hTuples[0],
                                out hTuples[1],
                                out hTuples[2],
                                out hTuples[3]
                            );
                            hObject = hTuples.GenRectangle();
                        }
                        break;
                    case ShapeType.Circle:
                        {
                            HOperatorSet.DrawCircle(
                                hWindow,
                                out hTuples[0],
                                out hTuples[1],
                                out hTuples[2]
                            );
                            hObject = hTuples.GenCircle();
                        }
                        break;
                    case ShapeType.Ellipse:
                        {
                            HOperatorSet.DrawEllipse(
                                hWindow,
                                out hTuples[0],
                                out hTuples[1],
                                out hTuples[2],
                                out hTuples[3],
                                out hTuples[4]
                            );
                            hObject = hTuples.GenEllipse();
                        }
                        break;
                    case ShapeType.Region:
                        {
                            HOperatorSet.DrawRegion(out hObject, hWindow);
                        }
                        break;
                }
            });
            if(shapeType == ShapeType.Region)
            {
                MaskObject = hObject;
            }
            else if (hObject != null)
            {
                DrawingObjectList.Add(
                    new DrawingObjectInfo
                    {
                        ShapType = shapeType,
                        HObject = hObject,
                        HTuple = hTuples,
                    }
                );
                HOperatorSet.GenContourRegionXld(hObject, out HObject hContour, "border");//获取对象的轮廓
                HOperatorSet.DispObj(hContour, hWindow);
            }
            txtMsg.Text = string.Empty;
            HSmart.HZoomContent = HSmartWindowControlWPF.ZoomContent.WheelForwardZoomsIn;
        }

        private void HsmartLoad(object sender, RoutedEventArgs e)
        {
            hWindow = HSmart.HalconWindow;
            HWindow = hWindow;
        }
    }
}
