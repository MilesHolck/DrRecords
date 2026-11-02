using DrRecords.Models;

namespace DrRecords.Repositories
{
    public class MusicRecordsRepositoryList
    {
        private readonly List<MusicRecord> _MusicRecords;

        private int _nextId = 1;

        public MusicRecordsRepositoryList()
        {
            _MusicRecords = new List<MusicRecord>();
            new MusicRecord { Id = 1, Title = "Ofelia", Artist = "Taylor Swift", Duration = 123, PublicationYear = 2023 };
            new MusicRecord { Id = 1, Title = "music of the night", Artist = "Andrew webber", Duration = 123, PublicationYear = 1980 };
            new MusicRecord { Id = 1, Title = "born to be wild", Artist = "Jon bon jovi", Duration = 123, PublicationYear = 1995};

        }



    }
}
