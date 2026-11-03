using DrRecords.Models;
using DrRecords.Repositories;
using DrRecords.Controllers;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

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
            Assert.NotEmpty(result);
        }

        [Fact]
        public void AddMusicRecord_AddsRecordToList()
        {
            // Arrange
            var newRecord = new MusicRecord
            {
                Title = "Test Song",
                Artist = "Test Artist",
                Duration = 200,
                PublicationYear = 2024
            };
            var initialCount = _repo.GetAllMusicRecords().Count();

            // Act
            var result = _repo.AddMusicRecord(newRecord);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
            Assert.Equal(initialCount + 1, _repo.GetAllMusicRecords().Count());
        }

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

        [Fact]
        public void GetMusicRecordById_WithValidId_ReturnsCorrectRecord()
        {
            // Act
            var result = _repo.GetMusicRecordById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Ofelia", result.Title);
            Assert.Equal("Taylor Swift", result.Artist);
        }

        [Fact]
        public void GetMusicRecordById_WithNonExistentId_ReturnsNull()
        {
            // Act
            var result = _repo.GetMusicRecordById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetMusicRecordById_WithDifferentValidId_ReturnsCorrectRecord()
        {
            // Act
            var result = _repo.GetMusicRecordById(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("music of the night", result.Title);
            Assert.Equal("Andrew webber", result.Artist);
        }

        [Fact]
        public void DeleteByIdMusicRecord_WithValidId_RemovesRecordAndReturnsIt()
        {
            // Act
            var deletedRecord = _repo.DeleteByIdMusicRecord(1);
            var remainingRecords = _repo.GetAllMusicRecords();

            // Assert
            Assert.NotNull(deletedRecord);
            Assert.Equal(1, deletedRecord.Id);
            Assert.Equal("Ofelia", deletedRecord.Title);
            Assert.Equal(2, remainingRecords.Count());
        }

        [Fact]
        public void DeleteByIdMusicRecord_WithNonExistentId_ReturnsNullAndDoesNotModifyList()
        {
            // Act
            var deletedRecord = _repo.DeleteByIdMusicRecord(999);
            var remainingRecords = _repo.GetAllMusicRecords();

            // Assert
            Assert.Null(deletedRecord);
            Assert.Equal(3, remainingRecords.Count());
        }

        [Fact]
        public void UpdateMusicRecord_WithValidId_UpdatesRecordSuccessfully()
        {
            // Arrange
            var updatedRecord = new MusicRecord
            {
                Title = "Updated Title",
                Artist = "Updated Artist",
                Duration = 200,
                PublicationYear = 2024
            };

            // Act
            var result = _repo.UpdateMusicRecord(1, updatedRecord);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Updated Title", result.Title);
            Assert.Equal("Updated Artist", result.Artist);
            Assert.Equal(200, result.Duration);
            Assert.Equal(2024, result.PublicationYear);
        }

        [Fact]
        public void UpdateMusicRecord_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var updatedRecord = new MusicRecord
            {
                Title = "Updated Title",
                Artist = "Updated Artist",
                Duration = 200,
                PublicationYear = 2024
            };

            // Act
            var result = _repo.UpdateMusicRecord(999, updatedRecord);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateMusicRecord_UpdateOnlySpecificFields_VerifyOnlyFieldsChanged()
        {
            // Arrange
            var updatedRecord = new MusicRecord
            {
                Title = "New Title Only",
                Artist = "New Artist Only",
                Duration = 300,
                PublicationYear = 2025
            };
            var originalRecord = _repo.GetMusicRecordById(2);

            // Act
            var result = _repo.UpdateMusicRecord(2, updatedRecord);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id); // ID should not change
            Assert.Equal("New Title Only", result.Title); // Should be updated
            Assert.Equal("New Artist Only", result.Artist); // Should be updated
            Assert.Equal(300, result.Duration); // Should be updated
            Assert.Equal(2025, result.PublicationYear); // Should be updated
            Assert.NotEqual(originalRecord?.Title, result.Title);
        }
    }

    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();

            // Setup JWT configuration
            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(x => x["Key"]).Returns("your-secret-key-must-be-at-least-32-characters-long-for-hmacsha256");
            jwtSection.Setup(x => x["Issuer"]).Returns("YourApp");
            jwtSection.Setup(x => x["Audience"]).Returns("YourAppUsers");

            _mockConfig.Setup(x => x.GetSection("Jwt")).Returns(jwtSection.Object);

            _authController = new AuthController(_mockConfig.Object);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "User", Password = "1234" };

            // Act
            var result = _authController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var responseType = okResult.Value!.GetType();
            var tokenProperty = responseType.GetProperty("token");
            Assert.NotNull(tokenProperty);
            var token = (string?)tokenProperty?.GetValue(okResult.Value);
            Assert.NotNull(token);
        }

        [Fact]
        public void Login_WithInvalidUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "InvalidUser", Password = "1234" };

            // Act
            var result = _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "User", Password = "wrongpassword" };

            // Act
            var result = _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_GeneratedToken_IsValidJwt()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "User", Password = "1234" };

            // Act
            var result = _authController.Login(loginRequest);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseType = okResult.Value!.GetType();
            var tokenProperty = responseType.GetProperty("token");
            var token = (string?)tokenProperty?.GetValue(okResult.Value);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            Assert.Equal("YourApp", jwtToken.Issuer);
            Assert.Equal("YourAppUsers", jwtToken.Audiences.First());
        }

        [Fact]
        public void Login_GeneratedToken_ContainsCorrectClaims()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "User", Password = "1234" };

            // Act
            var result = _authController.Login(loginRequest);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseType = okResult.Value!.GetType();
            var tokenProperty = responseType.GetProperty("token");
            var token = (string?)tokenProperty?.GetValue(okResult.Value);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            Assert.NotNull(usernameClaim);
            Assert.Equal("User", usernameClaim.Value);

            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            Assert.NotNull(roleClaim);
            Assert.Equal("admin", roleClaim.Value);
        }

        [Fact]
        public void Login_GeneratedToken_HasValidExpirationTime()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "User", Password = "1234" };

            // Act
            var result = _authController.Login(loginRequest);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseType = okResult.Value!.GetType();
            var tokenProperty = responseType.GetProperty("token");
            var token = (string?)tokenProperty?.GetValue(okResult.Value);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            Assert.NotNull(jwtToken.ValidTo);
            Assert.True(jwtToken.ValidTo > DateTime.Now);
            Assert.True(jwtToken.ValidTo <= DateTime.Now.AddHours(2).AddMinutes(1));
        }

        [Fact]
        public void Login_WithEmptyUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "", Password = "1234" };

            // Act
            var result = _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_WithEmptyPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "User", Password = "" };

            // Act
            var result = _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }
    }
}