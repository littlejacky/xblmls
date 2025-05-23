﻿using FluentScheduler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using XBLMS.Configuration;
using XBLMS.Core.Utils;

namespace XBLMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InstallUtils.Init(AppDomain.CurrentDomain.BaseDirectory);

            var host = CreateHostBuilder(args).Build();

            // 注册应用程序关闭事件
            host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(() =>
            {
                // ֹͣ停止 FluentScheduler
                JobManager.StopAndBlock();
            });

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile(Constants.PackageFileName, optional: true, reloadOnChange: true)
                        .AddJsonFile(Constants.ConfigFileName, optional: true, reloadOnChange: true)
                        //.AddEnvironmentVariables(Constants.EnvironmentPrefix)
                        .AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel(options => { options.Limits.MaxRequestBodySize = long.MaxValue; })
                        .UseIIS()
                        .UseStartup<Startup>();
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
                    loggerConfiguration.Enrich.FromLogContext();
                });
    }
}
