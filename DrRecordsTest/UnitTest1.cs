using DrRecords.Models;
using DrRecords.Repositories;
using DrRecords.Controllers;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var resultValue = okResult.Value as dynamic;
            Assert.NotNull(resultValue?.token);
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
            var resultValue = okResult.Value as dynamic;
            string token = resultValue?.token;

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
            var resultValue = okResult.Value as dynamic;
            string token = resultValue?.token;

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
            var resultValue = okResult.Value as dynamic;
            string token = resultValue?.token;

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            Assert.NotNull(jwtToken.ValidTo);
            Assert.True(jwtToken.ValidTo > DateTime.Now);
            Assert.True(jwtToken.ValidTo <= DateTime.Now.AddHours(2).AddMinutes(1)); // Allow 1 minute tolerance
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