using BaresTucuman.API.Domain.Entities;

namespace BaresTucuman.API.Domain.Interfaces
{
    
    public interface IBarProvider
    {
        Task<List<Bar>> GetBaresAsync();
    }
    
}
