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

            // 注册每分钟更新计时练习状态
            JobManager.AddJob(
                async () => await ScanUnfinishPlanPractices(),
                s => s.ToRunEvery(1).Minutes()
            );
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

        private async Task ScanUnfinishPlanPractices()
        {
            try
            {
                // 在需要时创建作用域并解析服务
                using (var scope = _serviceProvider.CreateScope())
                {
                    var examManager = scope.ServiceProvider.GetRequiredService<IExamManager>();
                    await examManager.MarkUnfinishPlanPractices();
                }
            }
            catch (Exception ex)
            {
                // 记录错误日志
                Console.WriteLine($"创建每日培训任务失败: {ex.Message}");
            }
        }
    }
}
