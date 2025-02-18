using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc; // Add this to configure FormOptions

namespace razor_data_tools
{
    public class Startup
    {
        private static readonly int UploadSizeLimit = 1048576000; // Set global upload size limit to 1 GB (in bytes)

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    // Increase multipart body length limit to UploadSizeLimit
                    options.Conventions.ConfigureFilter(new RequestSizeLimitAttribute(UploadSizeLimit));
                });

            // Configure Kestrel server options
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = UploadSizeLimit;
            });

            // Alternatively, configure IIS server options (if running under IIS)
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = UploadSizeLimit;
            });

            // Configure form options to increase the multipart body length limit
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = UploadSizeLimit;
            });

            // Add other services if required
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
