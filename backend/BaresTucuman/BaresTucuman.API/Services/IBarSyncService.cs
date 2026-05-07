namespace BaresTucuman.API.Services 
{
    public interface IBarSyncService
    {
        Task<int> SyncBaresAsync();
    }
}