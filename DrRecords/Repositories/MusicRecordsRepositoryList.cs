using DrRecords.Models;

namespace DrRecords.Repositories
{
    public class MusicRecordsRepositoryList : IMusicRecordsRepositoryList
    {
        private readonly List<MusicRecord> _MusicRecords;

        private int _nextId = 1;

        public MusicRecordsRepositoryList()
        {
            _MusicRecords = new List<MusicRecord>();
            _MusicRecords.Add(new MusicRecord { Id = 1, Title = "Ofelia", Artist = "Taylor Swift", Duration = 123, PublicationYear = 2023 });
            _MusicRecords.Add(new MusicRecord { Id = 2, Title = "music of the night", Artist = "Andrew webber", Duration = 123, PublicationYear = 1980 });
            _MusicRecords.Add(new MusicRecord { Id = 3, Title = "born to be wild", Artist = "Jon bon jovi", Duration = 123, PublicationYear = 1995 });
        }

        public IEnumerable<MusicRecord> GetAllMusicRecords(string? titleStartsWith = null, string? artistStartsWith = null)
        {
            IEnumerable<MusicRecord> filteredRecords = _MusicRecords;

            if (!string.IsNullOrEmpty(titleStartsWith))
            {
                filteredRecords = filteredRecords.Where(
                    m => m.Title?.StartsWith(
                        titleStartsWith,
                        StringComparison.OrdinalIgnoreCase) == true);
            }

            if (!string.IsNullOrEmpty(artistStartsWith))
            {
                filteredRecords = filteredRecords.Where(
                    m => m.Artist?.StartsWith(
                        artistStartsWith,
                        StringComparison.OrdinalIgnoreCase) == true);
            }

            return filteredRecords;
        }





    }
}
