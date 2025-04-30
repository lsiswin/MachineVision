using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MachineVision.Services
{
    public class BaseServie
    {
        public static string ConnectionString = @"DataSource=SqlSugar4xTest.sqlite";
        public static SqlSugarScope Db = new SqlSugarScope(
            new ConnectionConfig()
            {
                ConnectionString = ConnectionString, //连接符字串
                DbType = DbType.Sqlite, //数据库类型
                IsAutoCloseConnection = true, //不设成true要手动close
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {

                    SqliteCodeFirstEnableDefaultValue = true //启用默认值
                }
            },
            db =>
            {
                //(A)全局生效配置点，一般AOP和程序启动的配置扔这里面 ，所有上下文生效
                //调试SQL事件，可以删掉
                //db.Aop.OnLogExecuting = (sql, pars) =>
                //{
                //    //获取原生SQL推荐 5.1.4.63  性能OK
                //    Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

                //    //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                //    //Console.WriteLine(UtilMethods.GetSqlString(DbType.SqlServer,sql,pars))
                //};

                //多个配置就写下面
                //db.Ado.IsDisableMasterSlaveSeparation=true;

                //注意多租户 有几个设置几个
                //db.GetConnection(i).Aop
            }
        );
    }
}
