using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HalconDotNet;
using MachineVision.Core.Extensions;
using MachineVision.Core.TemplateMatch.Shard;

namespace MachineVision.Core.TemplateMatch.NccModel
{
    public class NccModelService : BindableBase, ITemplateMatchService
    {
        private HTuple NccModelID;

        public NccModelService()
        {
            Info = new MethodInfo()
            {
                Name = "find_ncc_model",
                Description = "Find the best matches of an NCC model in an image.",
                MethodParameters = new List<MethodParameter>()
                {
                    new MethodParameter()
                    {
                        Name = "Image",
                        Description = "Input image in which the model should be found.",
                    },
                    new MethodParameter()
                    {
                        Name = "ModelID",
                        Description = "Handle of the model.",
                    },
                    new MethodParameter()
                    {
                        Name = "AngleStart",
                        Description = "Smallest rotation of the model.",
                    },
                    new MethodParameter()
                    {
                        Name = "AngleExtent",
                        Description = "Extent of the rotation angles.",
                    },
                    new MethodParameter()
                    {
                        Name = "MinScore",
                        Description = "Minimum score of the instances of the model to be found.",
                    },
                    new MethodParameter()
                    {
                        Name = "NumMatches",
                        Description =
                            "Number of instances of the model to be found (or 0 for all matches).",
                    },
                    new MethodParameter()
                    {
                        Name = "MaxOverlap ",
                        Description = "Maximum overlap of the instances of the model to be found.",
                    },
                    new MethodParameter()
                    {
                        Name = "SubPixel",
                        Description = "Subpixel accuracy if not equal to 'none'.",
                    },
                    new MethodParameter()
                    {
                        Name = "NumLevels",
                        Description =
                            "Number of pyramid levels used in the matching (and lowest pyramid level to use if |NumLevels| = 2).",
                    },
                    new MethodParameter()
                    {
                        Name = "Row",
                        Description = "Row coordinate of the found instances of the model.",
                    },
                    new MethodParameter()
                    {
                        Name = "Column",
                        Description = "Column coordinate of the found instances of the model.",
                    },
                    new MethodParameter()
                    {
                        Name = "Angle",
                        Description = "Rotation angle of the found instances of the model.",
                    },
                    new MethodParameter()
                    {
                        Name = "Score",
                        Description = "Score of the found instances of the model.",
                    },
                },
                Predecessors = new List<string>()
                {
                    "create_ncc_model",
                    "read_ncc_model",
                    "write_ncc_model",
                },
            };
            TemplateParameter = new NccModelInputParameter();
            RunParameter = new NccModelRunParameter();
            Setting = new MatchResultSetting();
            TemplateParameter.ApplyDefaultParameter();
            RunParameter.ApplyDefaultParameter();
        }

        HTuple hv_Row = new HTuple(),
            hv_Column = new HTuple();
        HTuple hv_Angle = new HTuple(),
            hv_Score = new HTuple();
        public MethodInfo Info { get; set; }
        public RoiPararmeter RoiPararmeter { get; set; }
        public MatchResultSetting Setting { get; set; }

        public NccModelInputParameter TemplateParameter { get; set; }
        public NccModelRunParameter RunParameter { get; set; }

        public async Task CreateTemplate(HObject image, HObject hObject)
        {
            await Task.Run(() =>
            {
                var template = image.ReduceDomain(hObject);
                HOperatorSet.CreateNccModel(
                    template.Rgb1ToGray(),
                    TemplateParameter.NumLevels,
                    TemplateParameter.AngleStart,
                    TemplateParameter.AngleExtent,
                    TemplateParameter.AngleStep,
                    TemplateParameter.Metric,
                    out NccModelID
                );
            });
        }

        public MatchResult Run(HObject image)
        {
            MatchResult matchResult = new MatchResult();
            if (NccModelID == null)
            {
                matchResult.Message = "请先创建模板";
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
                HOperatorSet.FindNccModel(
                    imageReduced.Rgb1ToGray(),
                    NccModelID,
                    RunParameter.AngleStart,
                    RunParameter.AngleExtent,
                    RunParameter.MinScore,
                    RunParameter.NumMatches,
                    RunParameter.MaxOverlap,
                    RunParameter.SubPixel,
                    RunParameter.NumLevels,
                    out hv_Row,
                    out hv_Column,
                    out hv_Angle,
                    out hv_Score
                );
            });
            if (hv_Row.Length > 0)
            {
                matchResult.Message = $"匹配成功,耗时{timeSpane}ms";
            }

            for (int i = 0; i < hv_Row.Length; i++)
            {
                matchResult.Results.Add(
                    new TemplateMatchResult
                    {
                        Index = i + 1,
                        Row = hv_Row.DArr[i],
                        Column = hv_Column.DArr[i],
                        Angle = hv_Angle.DArr[i],
                        Score = hv_Score.DArr[i],
                        TransContours = GetNccModelContours(
                            NccModelID,
                            hv_Row.DArr[i],
                            hv_Column.DArr[i],
                            hv_Angle.DArr[i],
                            0
                        ),
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
        public void SetRunParameter() { }

        public void SetTemplateParameter() { }

        /// <summary>
        /// 获取相关性匹配的结果轮廓
        /// </summary>
        /// <param name="hv_ModelID"></param>
        /// <param name="hv_Row"></param>
        /// <param name="hv_Column"></param>
        /// <param name="hv_Angle"></param>
        /// <param name="hv_Model"></param>
        /// <returns></returns>
        public HObject GetNccModelContours(
            HTuple hv_ModelID,
            HTuple hv_Row,
            HTuple hv_Column,
            HTuple hv_Angle,
            HTuple hv_Model
        )
        {
            HObject ho_ModelContours = new HObject();
            HObject ho_ContoursAffinTrans = new HObject();
            HObject ho_MergedContours = new HObject();

            HTuple hv_HomMat2D = new HTuple();

            // 初始化输出对象
            HOperatorSet.GenEmptyObj(out ho_MergedContours);

            // 参数验证
            if (hv_Row.TupleLength() == 0)
                return ho_MergedContours;

            // 处理模型参数
            HTuple hv_ModelAdjusted = new HTuple();
            if (hv_Model.TupleLength() == 0)
            {
                hv_ModelAdjusted = HTuple.TupleGenConst(hv_Row.TupleLength(), 0);
            }
            else if (hv_Model.TupleLength() == 1)
            {
                hv_ModelAdjusted = HTuple.TupleGenConst(hv_Row.TupleLength(), hv_Model[0]);
            }
            else
            {
                hv_ModelAdjusted = new HTuple(hv_Model); // 使用构造函数复制元组
            }

            // 遍历所有模型实例
            for (int modelIdx = 0; modelIdx < hv_ModelID.TupleLength(); modelIdx++)
            {
                // 获取模板轮廓
                HObject ho_TemplateRegion;
                HOperatorSet.GetNccModelRegion(
                    out ho_TemplateRegion,
                    hv_ModelID.TupleSelect(modelIdx)
                );
                HOperatorSet.GenContourRegionXld(
                    ho_TemplateRegion,
                    out ho_ModelContours,
                    "border_holes"
                );
                ho_TemplateRegion.Dispose();

                // 处理匹配结果
                for (int matchIdx = 0; matchIdx < hv_Row.TupleLength(); matchIdx++)
                {
                    if (modelIdx == hv_ModelAdjusted.TupleSelect(matchIdx))
                    {
                        // 创建仿射矩阵
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                        HOperatorSet.HomMat2dRotate(
                            hv_HomMat2D,
                            hv_Angle.TupleSelect(matchIdx),
                            0,
                            0,
                            out hv_HomMat2D
                        );
                        HOperatorSet.HomMat2dTranslate(
                            hv_HomMat2D,
                            hv_Row.TupleSelect(matchIdx),
                            hv_Column.TupleSelect(matchIdx),
                            out hv_HomMat2D
                        );

                        // 执行仿射变换并合并结果
                        HOperatorSet.AffineTransContourXld(
                            ho_ModelContours,
                            out ho_ContoursAffinTrans,
                            hv_HomMat2D
                        );
                        HOperatorSet.ConcatObj(
                            ho_MergedContours,
                            ho_ContoursAffinTrans,
                            out ho_MergedContours
                        );
                    }
                }
            }

            // 清理临时对象
            ho_ModelContours.Dispose();
            ho_ContoursAffinTrans.Dispose();
            hv_HomMat2D.Dispose();
            hv_ModelAdjusted.Dispose();

            return ho_MergedContours;
        }
    }
}
