namespace WebNovels.Services.DesignServices
{
    using System.Threading.Tasks;

    public interface IThemeService
    {
        Task<bool> ApplyInitialThemeAsync();
        Task<bool> ToggleThemeAsync();
        Task<string> GetCurrentThemeAsync();
    }

}
