using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Core.TemplateMatch.Shard
{
    /// <summary>
    /// 定位服务处理结果
    /// </summary>
    public class MatchResult : BindableBase
    {
        public MatchResult()
        {
            Results = new ObservableCollection<TemplateMatchResult>();
            Setting = new MatchResultSetting();
        }


        /// <summary>
        /// 处理结果
        /// </summary>
        public bool IsSuccess { get; set; }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 耗时
        /// </summary>
        public double TimeSpane { get; set; }
        /// <summary>
        /// 匹配结果设置
        /// </summary>
        public MatchResultSetting Setting { get; set; }

        /// <summary>
        /// 目标结果
        /// </summary>
        public ObservableCollection<TemplateMatchResult> Results { get; }
    }
    public class MatchResultSetting : BindableBase
    {
        public MatchResultSetting()
        {
            IsShowCenter = true;
            IsShowDisplayText = true;
            IsShowMatchRange = true;
        }

        /// <summary>
        /// 显示中点
        /// </summary>
        private bool isShowCenter;

        public bool IsShowCenter
        {
            get { return isShowCenter; }
            set { isShowCenter = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 显示文本
        /// </summary>
        private bool isShowDisplayText;

        public bool IsShowDisplayText
        {
            get { return isShowDisplayText; }
            set { isShowDisplayText = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 显示检测范围
        /// </summary>
        private bool isShowMatchRange;

        public bool IsShowMatchRange
        {
            get { return isShowMatchRange; }
            set { isShowMatchRange = value; RaisePropertyChanged(); }
        }

    }
}
