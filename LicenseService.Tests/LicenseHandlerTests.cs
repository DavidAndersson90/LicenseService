using LicenseService.Exceptions;
using LicenseService.Handlers;
using LicenseService.Models;
using LicenseService.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace LicenseService.Tests
{
    public class LicenseHandlerTests
    {
        private readonly Mock<ILicenseRepository> _mockRepository;
        private readonly LicenseHandler _licenseHandler;
        
        private string clientId = "client1";

        public LicenseHandlerTests() 
        {
            var _mockLogger = new Mock<ILogger<LicenseHandler>>();
            _mockRepository = new Mock<ILicenseRepository>();
            _licenseHandler = new LicenseHandler(_mockLogger.Object,_mockRepository.Object);
        }

        [Fact]
        public void AddLicense_Should_CreateLicense_When_RequestIsValid()
        {
            _mockRepository.Setup(_ => _.CreateLicense(It.IsAny<License>())).Verifiable();

            _ = _licenseHandler.AddLicense(new AddLicenseRequestModel { Name = "license1" });
            _mockRepository.Verify(_ => _.CreateLicense(It.IsAny<License>()), Times.Once);
        }

        [Fact]
        public void GetLicenses_Should_ReturnCorrectStatusInResponse_When_LicenseIsAvailableForRental()
        {
            _mockRepository.Setup(_ => _.GetLicenses(It.IsAny<Expression<Func<License, bool>>>())).ReturnsAsync(new List<License>() { new License { RentalExpirationDate = DateTime.UtcNow } });
            
            var result = _licenseHandler.GetLicenses().GetAwaiter().GetResult().First();
            Assert.Equal("Not Rented", result.Status);
        }

        [Fact]
        public void GetLicenses_Should_ReturnCorrectStatusInResponse_WhenLicenseIsNotAvailableForRental()
        {
            _mockRepository.Setup(_ => _.GetLicenses(It.IsAny<Expression<Func<License, bool>>>()))
                .ReturnsAsync(new List<License>() { new License { RentalExpirationDate = DateTime.UtcNow.AddSeconds(15) } });
            
            var result = _licenseHandler.GetLicenses().GetAwaiter().GetResult().First();
            Assert.Equal($"{result.ClientId}, {(result.RentalExpirationDate - DateTime.UtcNow).Seconds} second(s) left", result.Status);
        }


        [Fact]
        public void RentLicense_Should_ThrowHttpResponseException_When_NoLicenseIsAvailableForRental()
        {
            _mockRepository.Setup(_ => _.GetLicense((It.IsAny<Expression<Func<License, bool>>>())))
                .Returns<License>(null);

            Assert.ThrowsAsync<HttpResponseException>(() => 
                _licenseHandler.RentLicense(new RentLicenseRequestModel { ClientId = "client1" }));
        }

        [Fact]
        public void RentLicense_Should_ThrowHttpResponseException_When_ClientHasActiveRental()
        {
            var licenseWithOngoingRental = new License { ClientId = clientId, RentalExpirationDate = DateTime.UtcNow.AddSeconds(15) };
            _mockRepository.Setup(_ => _.GetLicense((It.IsAny<Expression<Func<License, bool>>>()))).ReturnsAsync(new License());
            _mockRepository.Setup(_ => _.GetLicenses((It.IsAny<Expression<Func<License, bool>>>())))
                .ReturnsAsync(new List<License> { licenseWithOngoingRental });

            Assert.ThrowsAsync<HttpResponseException>(() => 
                _licenseHandler.RentLicense(new RentLicenseRequestModel { ClientId = "client1" }));
        }

        [Fact]
        public void RentLicense_Should_UpdateLicenseAsRented_When_AvailableLicenseAndClientHasNoActiveRental()
        {
            _mockRepository.Setup(_ => _.GetLicense((It.IsAny<Expression<Func<License, bool>>>()))).ReturnsAsync(new License());
            _mockRepository.Setup(_ => _.GetLicenses((It.IsAny<Expression<Func<License, bool>>>()))).ReturnsAsync(new List<License>());
            _mockRepository.Setup(_ => _.RentLicense(It.IsAny<License>(),It.IsAny<string>())).Verifiable();

            _ = _licenseHandler.RentLicense(new RentLicenseRequestModel { ClientId =  "client1"});
            _mockRepository.Verify(_ => _.RentLicense(It.IsAny<License>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void HasLicenseRentalExpired_Should_ReturnTrue_When_LicenseRentalHasExpired()
        {
            _mockRepository.Setup(_ => _.GetLicense(It.IsAny<Expression<Func<License, bool>>>()))
                .ReturnsAsync(new License { RentalExpirationDate = DateTime.UtcNow });

            var result = _licenseHandler.HasLicenseRentalExpired(It.IsAny<string>()).GetAwaiter().GetResult();
            Assert.True(result);
        }
    }
}