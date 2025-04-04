namespace StaffApp.Infrastructure.Services
{
    public class LoadingService
    {
        public event Action<bool> OnLoadingChanged;

        public bool IsLoading { get; private set; }

        public void Show()
        {
            IsLoading = true;
            OnLoadingChanged?.Invoke(true);
        }

        public void Hide()
        {
            IsLoading = false;
            OnLoadingChanged?.Invoke(false);
        }
    }
}
