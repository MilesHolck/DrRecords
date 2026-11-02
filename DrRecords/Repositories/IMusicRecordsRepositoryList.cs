using DrRecords.Models;

namespace DrRecords.Repositories
{
    public interface IMusicRecordsRepositoryList
    {
        IEnumerable<MusicRecord> GetAllMusicRecords();
    }
}