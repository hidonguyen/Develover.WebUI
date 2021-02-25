using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Develover.WebUI.Models
{
    public class ViewBagFilter : IActionFilter
    {
        private static readonly string Enabled = "Enabled";
        private static readonly string Disabled = string.Empty;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                // SmartAdmin Toggle Features
                controller.ViewBag.AppSidebar = Enabled;
                controller.ViewBag.AppHeader = Enabled;
                controller.ViewBag.AppLayoutShortcut = Enabled;
                controller.ViewBag.AppFooter = Enabled;
                controller.ViewBag.AppShortcutModal = Disabled;
                controller.ViewBag.LayoutSettings = Enabled;
                controller.ViewBag.MyApps = Disabled;
                controller.ViewBag.ChatInterface = Disabled;
                controller.ViewBag.ShortcutMenu = Enabled;

                controller.ViewBag.User = "Hido Nguyen";
                controller.ViewBag.UserPosition = "CEO";
                controller.ViewBag.Email = "hido@develover.vn";
                controller.ViewBag.Avatar = "avatar-admin.png";

                // SmartAdmin Default Settings
                controller.ViewBag.App = "Develover ERP";
                controller.ViewBag.AppName = "Develover ERP WebApp";
                controller.ViewBag.AppFlavor = ".NET 5.0";
                controller.ViewBag.AppFlavorSubscript = ".NET 5.0";
                controller.ViewBag.Version = "1.0.0";
                controller.ViewBag.Bs4v = "4.5";
                controller.ViewBag.Logo = "logo.png";
                controller.ViewBag.LogoM = "logo.png";
                controller.ViewBag.Copyright = "© 2021 <a href='https://develover.vn/' class='text-primary fw-500' title='Develover ESMS Co., Ltd.' target='_blank'>Develover ESMS Co., Ltd.</a>";
                controller.ViewBag.CopyrightInverse = "© 2021 <a href='https://develover.vn/' class='text-white opacity-40 fw-500' title='Develover ESMS Co., Ltd.' target='_blank'>Develover ESMS Co., Ltd.</a>";
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
