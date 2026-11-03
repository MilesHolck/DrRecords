using DrRecords.Models;

namespace DrRecords.Repositories
{
    public interface IMusicRecordsRepositoryList
    {
        MusicRecord AddMusicRecord(MusicRecord musicRecord);
        IEnumerable<MusicRecord> GetAllMusicRecords(string? titleStartsWith = null, string? artistStartsWith = null);
        MusicRecord? GetMusicRecordById(int id);
        MusicRecord? UpdateMusicRecord(int id, MusicRecord updatedMusicRecord);
        MusicRecord? DeleteByIdMusicRecord(int id);
    }
}
