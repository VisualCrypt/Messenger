namespace ObsidianMobile.Core.Interfaces.ViewModels
{
    public interface IChatDetailViewModel : IBaseViewModel
    {
        int ChatId { get; set; }

        string ChatName { get; }
    }
}