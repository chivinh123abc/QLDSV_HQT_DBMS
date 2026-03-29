using QLDSV_HTC.Domain.Constants;
using DevExpress.AspNetCore;

namespace QLDSV_HTC.Web.Extensions
{
    public static class PipelineExtensions
    {
        public static WebApplication UseAppPipeline(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler(RouteConstants.Home.ErrorPath);
            }

            // Automatically handle 404 and other status codes
            app.UseStatusCodePagesWithReExecute(RouteConstants.Home.NotFoundPath);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDevExpressControls();

            app.UseRouting();

            app.UseAuthentication(); // Important: must be placed before UseAuthorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=home}/{action=index}/{id?}");

            return app;
        }
    }
}
