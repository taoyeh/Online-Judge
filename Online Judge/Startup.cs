using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Online_Judge.Hubs;
using Online_Judge.Models;
using Online_Judge.Services;

namespace Online_Judge
{
    public class Startup
    {

        //����ע�룬��ȡ���ָ��������ú���Ϣ
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //�������ݿ�
            string connecttext = Configuration.GetConnectionString("MyContext");
            services.AddDbContext<MyContext>(options => options.UseSqlite(connecttext));

            //ģ�Ͱ�
            services.AddTransient<IEvaluationMachine, EvaluationMachine>();

            //Session����
            services.AddSession();

            //��ͻ��˷�����Ϣ
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // ͨ��
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // ���þ�̬��Դ
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<TrackHub>("/Track");
            });
        }
    }
}
