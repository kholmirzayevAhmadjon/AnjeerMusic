using AnjeerMusic.Models.MusicModels;

namespace AnjeerMusic.Service.Interface;

public interface IMusicService
{
    Task<MusicViewModel> CreateAsync(MusicCreationModel music);
    Task<bool> DeleteAsync(long id);
    Task<IEnumerable<MusicViewModel>> GetAllAsync();
    Task<IEnumerable<MusicViewModel>> SearchAsync(string searchString);
}