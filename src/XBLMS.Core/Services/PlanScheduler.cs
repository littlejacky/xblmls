using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XBLMS.Services;

namespace XBLMS.Core.Services
{
    public class PlanScheduler : IPlanScheduler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITaskManager _taskManager;

        public PlanScheduler(IServiceProvider serviceProvider, ITaskManager taskManager)
        {
            _serviceProvider = serviceProvider;
            _taskManager = taskManager;
        }

        public void Initialize()
        {
            // 注册每日培训任务
            JobManager.AddJob(
                async () => await CreateDailyTrainingTasks(),
                s => s.ToRunEvery(1).Days().At(6, 0) // 每天早上6点
            );

            //// 注册每周统计任务
            //JobManager.AddJob(
            //    async () => await AdjustUserCategoryWeights(),
            //    s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Sunday).At(23, 0) // 每周晚上11点
            //);
        }

        private async Task CreateDailyTrainingTasks()
        {
            try
            {
                // 在需要时创建作用域并解析服务
                using (var scope = _serviceProvider.CreateScope())
                {
                    var examManager = scope.ServiceProvider.GetRequiredService<IExamManager>();
                    await examManager.CreateDailyTrainingTasks();
                }
            }
            catch (Exception ex)
            {
                // 记录错误日志
                Console.WriteLine($"创建每日培训任务失败: {ex.Message}");
            }
        }

        //private async Task AdjustUserCategoryWeights()
        //{
        //    try
        //    {
        //        // 在需要时创建作用域并解析服务
        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            var trainingManager = scope.ServiceProvider.GetRequiredService<ITrainingManager>();

        //            // 获取所有活跃用户
        //            var users = await trainingManager.GetAllActiveUsers();

        //            // 为每个用户调整题目分类权重
        //            foreach (var user in users)
        //            {
        //                await trainingManager.AdjustUserCategoryWeights(user.Id);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // 记录错误日志
        //        Console.WriteLine($"调整用户题目分类权重失败: {ex.Message}");
        //    }
        //}
    }
}
