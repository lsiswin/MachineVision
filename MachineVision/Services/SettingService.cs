using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineVision.Models;

namespace MachineVision.Services
{
    public class SettingService : BaseServie, ISettingService
    {
        public async Task<Setting> GetSettingAsync()
        {
            //Db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(Setting));
            var setting = await Db.Queryable<Setting>().FirstAsync();
            if (setting == null)
            {
                setting = new Setting()
                {
                    Language = "zh-CN",
                    SkinName = "Default",
                    SkinColor = "#FF0000"
                };
                Db.Insertable(setting).ExecuteCommand();
                return await GetSettingAsync();
            }
            return setting; 
        }

        public async Task SaveSettingAsync(Setting input)
        {
            var setting = await Db.Queryable<Setting>().FirstAsync(t=>t.Id.Equals(input.Id));
            if (setting == null)
            {
                await Db.Insertable(input).ExecuteCommandAsync();
            }
            else
            {
                await Db.Updateable(input).Where(t => t.Id.Equals(input.Id))
                .ExecuteCommandAsync();
            }
            
        }
    }
}
