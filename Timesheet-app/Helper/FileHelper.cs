namespace Timesheet_app.Helper
{
    public class FileHelper
    {

        public static string GetTemplate(IConfiguration config)
        {
            var basePath = Directory.GetCurrentDirectory();
            var relative = config["FileLocation:Templates"];

            return Path.Combine(basePath, relative);
        }

        public static string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }
    }
}
