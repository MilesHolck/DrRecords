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


        //testing af listen

        [Fact]
        public void ReturnsAllFromTheList()
        {
            // Act
            var result = _repo.GetAllMusicRecords();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        //Filterings test//

        [Fact]
        public void FilterByTitleStartsWith_ReturnMatchingRecords()
        {
            // Act
            var result = _repo.GetAllMusicRecords(titleStartsWith: "born");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("born to be wild", result.First().Title);
        }

        [Fact]
        public void FilterByTitleStartsWith_CaseInsensitive()
        {
            // Act
            var result = _repo.GetAllMusicRecords(titleStartsWith: "BORN");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("born to be wild", result.First().Title);
        }

        [Fact]
        public void FilterByArtistStartsWith_ReturnMatchingRecords()
        {
            // Act
            var result = _repo.GetAllMusicRecords(artistStartsWith: "Taylor");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Taylor Swift", result.First().Artist);
        }

        [Fact]
        public void FilterByArtistStartsWith_CaseInsensitive()
        {
            // Act
            var result = _repo.GetAllMusicRecords(artistStartsWith: "andrew");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Andrew webber", result.First().Artist);
        }

        [Fact]
        public void FilterByBothParameters_ReturnMatchingRecords()
        {
            // Act
            var result = _repo.GetAllMusicRecords(titleStartsWith: "music", artistStartsWith: "Andrew");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("music of the night", result.First().Title);
            Assert.Equal("Andrew webber", result.First().Artist);
        }

        [Fact]
        public void FilterWithNoMatches_ReturnEmptyList()
        {
            // Act
            var result = _repo.GetAllMusicRecords(titleStartsWith: "xyz");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}