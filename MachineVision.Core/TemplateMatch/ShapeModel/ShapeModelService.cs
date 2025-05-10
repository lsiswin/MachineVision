using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using HalconDotNet;
using MachineVision.Core.Extensions;
using MachineVision.Core.TemplateMatch.Shard;

namespace MachineVision.Core.TemplateMatch.ShapeModel
{
    public class ShapeModelService : BindableBase, ITemplateMatchService
    {
        public ShapeModelService()
        {
            Info = new MethodInfo
            {
                Name = "find_shape_model",
                Description = "Find the best matches of a shape model in an image",
                MethodParameters = new List<MethodParameter>()
                {
                    new MethodParameter
                    {
                        Name = "Image",
                        Description = "Input image in which the model should be found.",
                    },
                    new MethodParameter { Name = "ModelID", Description = "Handle of the model." },
                    new MethodParameter
                    {
                        Name = "AngleStart",
                        Description = "Input image in which the model should be found.",
                    },
                    new MethodParameter
                    {
                        Name = "AngleExtent ",
                        Description = "Extent of the rotation angles.",
                    },
                    new MethodParameter
                    {
                        Name = "MinScore",
                        Description = "Minimum score of the instances of the model to be found.",
                    },
                    new MethodParameter
                    {
                        Name = "NumMatches",
                        Description =
                            "Number of instances of the model to be found (or 0 for all matches).",
                    },
                    new MethodParameter
                    {
                        Name = "MaxOverlap",
                        Description = "Maximum overlap of the instances of the model to be found.",
                    },
                    new MethodParameter
                    {
                        Name = "SubPixel",
                        Description = "Subpixel accuracy if not equal to 'none'.",
                    },
                    new MethodParameter
                    {
                        Name = "NumLevels",
                        Description =
                            "Number of pyramid levels used in the matching (and lowest pyramid level to use if |NumLevels| = 2).",
                    },
                    new MethodParameter
                    {
                        Name = "Greediness",
                        Description =
                            "“Greediness” of the search heuristic (0: safe but slow; 1: fast but matches may be missed).",
                    },
                    new MethodParameter
                    {
                        Name = "Row",
                        Description = "Row coordinate of the found instances of the model.",
                    },
                    new MethodParameter
                    {
                        Name = "Column",
                        Description = "Column coordinate of the found instances of the model.",
                    },
                    new MethodParameter
                    {
                        Name = "Angle",
                        Description = "Rotation angle of the found instances of the model.",
                    },
                    new MethodParameter
                    {
                        Name = "Score",
                        Description = "Score of the found instances of the model.",
                    },
                },
                Predecessors = new List<string>()
                {
                    "create_shape_model",
                    "read_shape_model",
                    "set_shape_model_origin",
                },
            };
            //初始化模板和运行参数
            TemplateParameter = new ShapeModelInputParameter();
            Setting = new MatchResultSetting();
            RunParameter = new ShapeModelRunParameter();
            TemplateParameter.ApplyDefaultParameter();
            RunParameter.ApplyDefaultParameter();
        }

        private MatchResultSetting setting;

        public MatchResultSetting Setting
        {
            get { return setting; }
            set
            {
                setting = value;
                RaisePropertyChanged();
            }
        }

        private HTuple modelID;
        HTuple hv_Row = new HTuple(),
            hv_Column = new HTuple();
        HTuple hv_Angle = new HTuple(),
            hv_Score = new HTuple();
        private ShapeModelInputParameter templateParameter;

        /// <summary>
        /// 模板参数
        /// </summary>
        public ShapeModelInputParameter TemplateParameter
        {
            get { return templateParameter; }
            set
            {
                templateParameter = value;
                RaisePropertyChanged();
            }
        }
        private ShapeModelRunParameter runParameter;

        /// <summary>
        /// 运行参数
        /// </summary>
        public ShapeModelRunParameter RunParameter
        {
            get { return runParameter; }
            set
            {
                runParameter = value;
                RaisePropertyChanged();
            }
        }

        public MethodInfo Info { get; set; }
        public RoiPararmeter RoiPararmeter { get; set; }

        public async Task CreateTemplate(HObject image, HObject hObject)
        {
            await Task.Run(() =>
            {
                var template = image.ReduceDomain(hObject);
                //创建模板
                HOperatorSet.CreateShapeModel(
                    template,
                    TemplateParameter.NumLevels,
                    TemplateParameter.AngleStart,
                    TemplateParameter.AngleExtent,
                    TemplateParameter.AngleStep,
                    TemplateParameter.Optimization,
                    TemplateParameter.Metric,
                    TemplateParameter.Contrast,
                    TemplateParameter.MinContrast,
                    out modelID
                );
            });
        }

        public void SetRunParameter() { }

        public void SetTemplateParameter() { }

        public MatchResult Run(HObject image)
        {
            MatchResult matchResult = new MatchResult();
            if (modelID == null)
            {
                matchResult.Message = "创建模板无效";
                return matchResult;
            }
            if (image == null)
            {
                matchResult.Message = "输入图像无效";
                return matchResult;
            }
            var timeSpane = SetTimeHelper.SetTime(() =>
            {
                var imageReduced = image.ReduceDomain(RoiPararmeter);
                HOperatorSet.FindShapeModel(
                    imageReduced,
                    modelID,
                    RunParameter.AngleStart,
                    RunParameter.AngleExtent,
                    RunParameter.MinScore,
                    RunParameter.NumMatches,
                    RunParameter.MaxOverlap,
                    RunParameter.SubPixel,
                    RunParameter.NumLevels,
                    RunParameter.Greediness,
                    out hv_Row,
                    out hv_Column,
                    out hv_Angle,
                    out hv_Score
                );
            });
            //获取形状模板轮廓
            HOperatorSet.GetShapeModelContours(out HObject modelContours, modelID, 1);
            for (int i = 0; i < hv_Score.Length; i++)
            {
                //计算轮廓
                HOperatorSet.VectorAngleToRigid(
                    0,
                    0,
                    0,
                    hv_Row.DArr[i],
                    hv_Column.DArr[i],
                    hv_Angle.DArr[i],
                    out HTuple homMat2D
                );
                HOperatorSet.AffineTransContourXld(
                    modelContours,
                    out HObject transContours,
                    homMat2D
                );
                matchResult.Results.Add(
                    new TemplateMatchResult
                    {
                        Index = i + 1,
                        Row = hv_Row.DArr[i],
                        Column = hv_Column.DArr[i],
                        Angle = hv_Angle.DArr[i],
                        Score = hv_Score.DArr[i],
                        TransContours = transContours,
                    }
                );
            }
            matchResult.TimeSpane = timeSpane;
            matchResult.Message =
                $"生成匹配结果耗时:{timeSpane}" + $",结果数量:{matchResult.Results.Count}个";
            matchResult.Setting = Setting;
            if (matchResult.Results.Count > 0)
                matchResult.IsSuccess = true;
            return matchResult;
        }
    }
}
