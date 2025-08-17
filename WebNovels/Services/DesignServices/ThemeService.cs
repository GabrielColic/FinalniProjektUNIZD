namespace WebNovels.Services.DesignServices
{
    using Microsoft.JSInterop;
    using System.Threading.Tasks;

    public class ThemeService : IThemeService
    {
        private readonly IJSRuntime _js;

        public ThemeService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<bool> ApplyInitialThemeAsync()
        {
            return await _js.InvokeAsync<bool>("themeFunctions.applyTheme");
        }

        public async Task<bool> ToggleThemeAsync()
        {
            return await _js.InvokeAsync<bool>("themeFunctions.toggleTheme");
        }

        public async Task<string> GetCurrentThemeAsync()
        {
            return await _js.InvokeAsync<string>("themeFunctions.getCurrentTheme");
        }

    }

}
