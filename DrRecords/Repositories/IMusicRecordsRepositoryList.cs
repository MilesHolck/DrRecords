using DrRecords.Models;

namespace DrRecords.Repositories
{
    public interface IMusicRecordsRepositoryList
    {
        IEnumerable<MusicRecord> GetAllMusicRecords(string? titleStartsWith = null, string? artistStartsWith = null);
    }
}