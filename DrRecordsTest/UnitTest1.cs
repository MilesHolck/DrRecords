using DrRecords.Models;
using DrRecords.Repositories;

namespace DrRecordsTest
{
    public class UnitTest1
    {
        private readonly IMusicRecordsRepositoryList _repo;

        public UnitTest1()
        {
            _repo = new MusicRecordsRepositoryList();
        }

        [Fact]
        public void ReturnsAllFromTheList()
        {
            // Act
            var result = _repo.GetAllMusicRecords();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }
    }
}