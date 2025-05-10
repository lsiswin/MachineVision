using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using MachineVision.Core.Extensions;
using MachineVision.Core.TemplateMatch.Shard;

namespace MachineVision.Core.TemplateMatch.CircleModel
{
    public class CircleService : BindableBase
    {
        public CircleService()
        {
            Info = new MethodInfo()
            {
                Name = "find_circle",
                Description = "Add a circle or a circular arc to a metrology model.",
                MethodParameters = new List<MethodParameter>()
                {
                    new MethodParameter()
                    {
                        Name = "MetrologyHandle",
                        Description = "Handle of the metrology model.",
                    },
                    new MethodParameter()
                    {
                        Name = "Row ",
                        Description =
                            "Row coordinate (or Y) of the center of the circle or circular arc.",
                    },
                    new MethodParameter()
                    {
                        Name = "Column ",
                        Description =
                            "Column (or X) coordinate of the center of the circle or circular arc.",
                    },
                    new MethodParameter()
                    {
                        Name = "Radius",
                        Description = "Radius of the circle or circular arc.",
                    },
                    new MethodParameter()
                    {
                        Name = "MeasureLength1 ",
                        Description =
                            "Half length of the measure regions perpendicular to the boundary.",
                    },
                    new MethodParameter()
                    {
                        Name = "MeasureLength2",
                        Description =
                            "Half length of the measure regions tangetial to the boundary.",
                    },
                    new MethodParameter()
                    {
                        Name = "MeasureSigma ",
                        Description = "Sigma of the Gaussian function for the smoothing.",
                    },
                    new MethodParameter()
                    {
                        Name = "MeasureThreshold",
                        Description = "Minimum edge amplitude.",
                    },
                    new MethodParameter()
                    {
                        Name = "GenParamName ",
                        Description = "Names of the generic parameters.",
                    },
                    new MethodParameter()
                    {
                        Name = "GenParamValue ",
                        Description = "Values of the generic parameters.",
                    },
                    new MethodParameter()
                    {
                        Name = "Index ",
                        Description = "Index of the created metrology object.",
                    },
                },
                Predecessors = new List<string>()
                {
                    "align_metrology_model",
                    "apply_metrology_model",
                },
            };
            HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
            RunParameter = new MeasureRunParameter();
            RunParameter.ApplyDefaultParameter();
        }
        HObject ho_Contours;
        HObject ho_Contour;

        
        HTuple hv_MetrologyHandle = new HTuple(),
            hv_Index = new HTuple();
        HTuple hv_Row1 = new HTuple(),
            hv_Column1 = new HTuple();
        HTuple hv_Parameter = new HTuple();
        private HWindow hWindow;

        public HWindow HWindow
        {
            get { return hWindow; }
            set
            {
                hWindow = value;
                RaisePropertyChanged();
            }
        }

        public MethodInfo Info { get; set; }

        private MeasureRunParameter runParameter;

        public MeasureRunParameter RunParameter
        {
            get { return runParameter; }
            set
            {
                runParameter = value;
                RaisePropertyChanged();
            }
        }

        public void Run(HObject image)
        {
            try
            {
                if (image == null) return;
                if (RunParameter.Radius == 0) return;
                if (RunParameter.Row == 0 && RunParameter.Column == 0) return;
                HOperatorSet.AddMetrologyObjectCircleMeasure(hv_MetrologyHandle, RunParameter.Row,RunParameter.Column,
                    RunParameter.Radius, RunParameter.MeasureLength1, RunParameter.MeasureLength2, RunParameter.MeasureSigma, RunParameter.MeasureThreshold, new HTuple(), new HTuple(), out hv_Index);
                var grayImage = image.Rgb1ToGray();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row1, out hv_Column1);
                HOperatorSet.ApplyMetrologyModel(grayImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, 0, "all", "result_type",
                    "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                    0, "all", 1.5);
                if (HWindow != null)
                {
                    HOperatorSet.SetColor(HWindow,"blue");
                    HWindow.DispObj(ho_Contour);
                    HWindow.DispObj(ho_Contours);
                    HWindow.SetMessage($"查找圆坐标:({Math.Round(hv_Parameter.DArr[0], 2)},{Math.Round(hv_Parameter.DArr[1], 2)}),半径:{Math.Round(hv_Parameter.DArr[2], 2)}","image",10,10,"black","true");
                    HOperatorSet.DispCross(HWindow, hv_Parameter[0], hv_Parameter[1],50,0);
                }
            }
            finally
            {
                ho_Contours.Dispose();
                ho_Contour.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Parameter.Dispose();
            }
        }
    }
}
