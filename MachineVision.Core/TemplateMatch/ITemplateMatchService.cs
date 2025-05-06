using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace MachineVision.Core.TemplateMatch
{
    /// <summary>
    /// 模板匹配接口
    /// </summary>
    public interface ITemplateMatchService:BindableBase
    {
        /// <summary>
        /// 模板匹配描述信息
        /// </summary>
        MethodInfo Info { get; set; }
        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="hObject">生成模板的指定区域图像</param>
        /// <returns></returns>
        Task CreateTemplate(HObject hObject);
        /// <summary>
        /// 设置模板参数
        /// </summary>
        void SetTemplateParameter();
        /// <summary>
        /// 设置运行时参数
        /// </summary>
        void SetRunParameter();
    }
}
