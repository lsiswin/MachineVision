using System.Collections.ObjectModel;
using System.Runtime.Intrinsics.X86;
using System.Windows;
using System.Windows.Controls;
using HalconDotNet;
using MachineVision.shard.Extensions;

namespace MachineVision.shard.Controls
{
    public class ImageEditView : Control
    {
        private HSmartWindowControlWPF HSmart;
        private HWindow HWindow;
        private TextBlock txtMsg;
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
            HWindow.DispObj(hObject);
            HWindow.SetPart(0, 0, -2, -2);
        }

        public override void OnApplyTemplate()
        {
            txtMsg = (TextBlock)GetTemplateChild("PART_MSG");
            if (GetTemplateChild("PART_SMART") is HSmartWindowControlWPF hSmart)
            {
                HSmart = hSmart;
                hSmart.Loaded += HsmartLoad;
            }
            if (GetTemplateChild("PART_Rect") is Button btnRect)
            {
                btnRect.Click += BtnRect_Click;
            }
            if (GetTemplateChild("PART_Ellipse") is Button btnEllipse)
            {
                btnEllipse.Click += BtnEllipse_Click;
            }
            if (GetTemplateChild("PART_Circle") is Button btnCircle)
            {
                btnCircle.Click += BtnCircle_Click;
            }
            if (GetTemplateChild("PART_DrawPen") is Button btnDrawpen)
            {
                btnDrawpen.Click += BtnDrawPen_Click;
            }
            
            if (GetTemplateChild("Clear") is Button btnClear)
            {
                btnClear.Click += (s, e) =>
                {
                    DrawingObjectList.Clear();
                    HWindow.ClearWindow();
                    DisPlay(Image);
                };
            }

            base.OnApplyTemplate();
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
                HOperatorSet.SetColor(HWindow, "blue");
                switch (shapeType)
                {
                    case ShapeType.Rectangle1:
                        {
                            HOperatorSet.DrawRectangle1(
                                HWindow,
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
                                HWindow,
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
                                HWindow,
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
                            HOperatorSet.DrawRegion(out hObject, HWindow);
                        }
                        break;
                }
            });
            if (hObject != null)
            {
                DrawingObjectList.Add(
                    new DrawingObjectInfo { ShapType = shapeType,HObject = hObject, HTuple = hTuples }
                );
                HOperatorSet.DispObj(hObject, HWindow);
            }
            txtMsg.Text = string.Empty;
            HSmart.HZoomContent = HSmartWindowControlWPF.ZoomContent.WheelForwardZoomsIn;
        }

        private void HsmartLoad(object sender, RoutedEventArgs e)
        {
            HWindow = HSmart.HalconWindow;
        }
    }
}
