using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineVision.Models;

namespace MachineVision.Services
{
    /// <summary>
    /// 系统设置服务接口
    /// </summary>
    public interface ISettingService
    {
        Task<Setting> GetSettingAsync();
        Task SaveSettingAsync(Setting input);
    }
}
