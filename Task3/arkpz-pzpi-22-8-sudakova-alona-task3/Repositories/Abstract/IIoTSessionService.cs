using FocusLearn.Models.DTO;
namespace FocusLearn.Repositories.Abstract
{
    public interface IIoTSessionService
    {
        Task<bool> AddIoTSessionAsync(IoTSessionDTO sessionDto);
        Task<IEnumerable<IoTSessionDTO>> GetIoTSessionsByUserIdAsync(int userId);
    }
}
