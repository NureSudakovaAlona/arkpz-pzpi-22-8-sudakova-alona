using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;

namespace FocusLearn.Repositories.Abstract
{
    public interface IConcentrationMethodService
    {
        Task<IEnumerable<ConcentrationMethod>> GetAllMethodsAsync();
        Task<ConcentrationMethod?> GetMethodByIdAsync(int id);
        Task AddMethodAsync(ConcentrationMethodDTO methodDto);
        Task<bool> UpdateMethodAsync(int id, ConcentrationMethodDTO methodDto);
        Task<bool> DeleteMethodAsync(int id);
    }
}
