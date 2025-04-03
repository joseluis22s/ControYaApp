namespace ControYaApp.Services.Navigation
{
    public class MauiNavigationService : INavigationService
    {
        public Task GoToAsync(string route, IDictionary<string, object> routeParameters = null)
        {
            ShellNavigationState shellNavigation = new(route);

            return routeParameters != null ? Shell.Current.GoToAsync(shellNavigation, routeParameters) : Shell.Current.GoToAsync(shellNavigation);
        }

        public Task GoBackAsync(IDictionary<string, object> routeParameters = null)
        {

            return routeParameters != null ? Shell.Current.GoToAsync("..", routeParameters) : Shell.Current.GoToAsync("..");
        }
    }
}
