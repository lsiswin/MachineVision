using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using MachineVision.Core.Extensions;
using MachineVision.Core.TemplateMatch.Shard;

namespace MachineVision.Core.TemplateMatch.QcrCodeModel
{
    public class BarCodeService
    {
        public BarCodeService()
        {
            HOperatorSet.CreateBarCodeModel(new HTuple(), new HTuple(), out hv_BarCodeHandle);
            Info = new MethodInfo()
            {
                Name = "find_bar_code",
                Description = "Detect and read bar code symbols in an image.",
                MethodParameters = new List<MethodParameter>()
                {
                   new MethodParameter(){ Name="Image", Description="Input image. If the image has a reduced domain, the barcode search is reduced to that domain. This usually reduces the runtime of the operator. However, if the barcode is not fully inside the domain, the barcode cannot be decoded correctly." },
                   new MethodParameter(){ Name="SymbolRegions", Description="Regions of the successfully decoded bar code symbols." },
                   new MethodParameter(){ Name="BarCodeHandle", Description="Handle of the bar code model." },
                   new MethodParameter(){ Name="CodeType", Description="Type of the searched bar code." },
                   new MethodParameter(){ Name="DecodedDataStrings ", Description="Data strings of all successfully decoded bar codes." },
                },
                Predecessors = new List<string>()
                {
                     "get_bar_code_result",
                     "get_bar_code_object",
                     "clear_bar_code_model",
                }
            };
        }

        HObject ho_SymbolRegions = null;
        HTuple hv_BarCodeHandle = new HTuple();
        HTuple hv_DecodedDataStrings = new HTuple();

        public MethodInfo Info { get ; set; }
        public RoiPararmeter RoiPararmeter { get; set; }
        public OcrMatchResult Run(HObject image)
        {
            try
            {
                OcrMatchResult result = new OcrMatchResult();
                result.TimeSpan = SetTimeHelper.SetTime(() =>
                {
                    HOperatorSet.FindBarCode(image, out ho_SymbolRegions,
                                       hv_BarCodeHandle, "Code 128", out hv_DecodedDataStrings);
                });
                if (!string.IsNullOrWhiteSpace(hv_DecodedDataStrings.S))
                {
                    result.IsSuccess = true;
                    result.Message = $"条形码匹配成功,生成匹配结果耗时:{result.TimeSpan}" + $",匹配结果:{hv_DecodedDataStrings.S}";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = $"{DateTime.Now} 匹配失败";
                }
                return result;
                

            }
            finally
            {
                ho_SymbolRegions.Dispose();
                hv_BarCodeHandle.Dispose();
                hv_DecodedDataStrings.Dispose();
            }
        }
        
    }
}
