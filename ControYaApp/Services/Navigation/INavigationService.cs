namespace ControYaApp.Services.Navigation
{
    public interface INavigationService
    {
        Task LogOutAsync();

        Task GoToAsync(string route, IDictionary<string, object> routeParameters = null);

        Task GoBackAsync(IDictionary<string, object> routeParameters = null);
    }
}
