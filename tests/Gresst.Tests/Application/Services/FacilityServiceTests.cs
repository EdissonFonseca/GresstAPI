using FluentAssertions;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Moq;
using Xunit;

namespace Gresst.Tests.Application.Services;

/// <summary>
/// Unit tests for FacilityService
/// </summary>
public class FacilityServiceTests
{
    private readonly Mock<IRepository<Facility>> _facilityRepositoryMock;
    private readonly Mock<IDataSegmentationService> _segmentationServiceMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly FacilityService _facilityService;

    public FacilityServiceTests()
    {
        _facilityRepositoryMock = new Mock<IRepository<Facility>>();
        _segmentationServiceMock = new Mock<IDataSegmentationService>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _facilityService = new FacilityService(
            _facilityRepositoryMock.Object,
            _segmentationServiceMock.Object,
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserServiceMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenFacilityExists_ReturnsFacilityDto()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var facility = new Facility
        {
            Id = facilityId,
            Name = "Test Facility",
            Code = "FAC-001",
            IsActive = true
        };

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        _segmentationServiceMock
            .Setup(s => s.UserHasAccessToFacilityAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _facilityService.GetByIdAsync(facilityId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(facilityId);
        result.Name.Should().Be("Test Facility");
        result.Code.Should().Be("FAC-001");
    }

    [Fact]
    public async Task GetByIdAsync_WhenFacilityDoesNotExist_ReturnsNull()
    {
        // Arrange
        var facilityId = Guid.NewGuid();

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facility?)null);

        // Act
        var result = await _facilityService.GetByIdAsync(facilityId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserHasNoAccess_ReturnsNull()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var facility = new Facility
        {
            Id = facilityId,
            Name = "Test Facility",
            Code = "FAC-001",
            IsActive = true
        };

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        _segmentationServiceMock
            .Setup(s => s.UserHasAccessToFacilityAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _facilityService.GetByIdAsync(facilityId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WhenUserIsAdmin_ReturnsAllFacilities()
    {
        // Arrange
        var facilities = new List<Facility>
        {
            new Facility { Id = Guid.NewGuid(), Name = "Facility 1", Code = "FAC-001", IsActive = true },
            new Facility { Id = Guid.NewGuid(), Name = "Facility 2", Code = "FAC-002", IsActive = true }
        };

        _segmentationServiceMock
            .Setup(s => s.CurrentUserIsAdminAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _facilityRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(facilities);

        // Act
        var result = await _facilityService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Name == "Facility 1");
        result.Should().Contain(f => f.Name == "Facility 2");
    }

    [Fact]
    public async Task GetAllAsync_WhenUserIsNotAdmin_ReturnsOnlyAssignedFacilities()
    {
        // Arrange
        var userFacilityId = Guid.NewGuid();
        var otherFacilityId = Guid.NewGuid();
        var facilities = new List<Facility>
        {
            new Facility { Id = userFacilityId, Name = "My Facility", Code = "FAC-001", IsActive = true },
            new Facility { Id = otherFacilityId, Name = "Other Facility", Code = "FAC-002", IsActive = true }
        };

        _segmentationServiceMock
            .Setup(s => s.CurrentUserIsAdminAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _segmentationServiceMock
            .Setup(s => s.GetUserFacilityIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { userFacilityId });

        // Mock FindAsync to return only the facility that matches the user's assigned IDs
        _facilityRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Facility, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((System.Linq.Expressions.Expression<Func<Facility, bool>> predicate, CancellationToken ct) =>
            {
                var compiled = predicate.Compile();
                return facilities.Where(compiled).ToList();
            });

        // Act
        var result = await _facilityService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(f => f.Id == userFacilityId);
        result.Should().NotContain(f => f.Name == "Other Facility");
    }

    [Fact]
    public async Task CreateAsync_WhenValidDto_CreatesFacility()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var createDto = new CreateFacilityDto
        {
            Name = "New Facility",
            Code = "FAC-NEW",
            FacilityType = "Storage"
        };

        var createdFacility = new Facility
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Code = createDto.Code,
            FacilityType = createDto.FacilityType,
            IsActive = true
        };

        _currentUserServiceMock
            .Setup(s => s.GetCurrentAccountId())
            .Returns(accountId);

        var accountPersonId = Guid.NewGuid();
        _accountRepositoryMock
            .Setup(r => r.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Account { Id = accountId, PersonId = accountPersonId });

        _facilityRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facility f, CancellationToken ct) => f);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _facilityService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.Code.Should().Be(createDto.Code);
        _facilityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenFacilityExists_UpdatesFacility()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var existingFacility = new Facility
        {
            Id = facilityId,
            Name = "Old Name",
            Code = "FAC-001",
            IsActive = true
        };

        var updateDto = new UpdateFacilityDto
        {
            Id = facilityId,
            Name = "Updated Name"
        };

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFacility);

        _segmentationServiceMock
            .Setup(s => s.UserHasAccessToFacilityAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _facilityService.UpdateAsync(updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        _facilityRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenFacilityExists_DeletesFacility()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var facility = new Facility
        {
            Id = facilityId,
            Name = "Test Facility",
            Code = "FAC-001",
            IsActive = true
        };

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        _segmentationServiceMock
            .Setup(s => s.UserHasAccessToFacilityAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _facilityService.DeleteAsync(facilityId);

        // Assert
        result.Should().BeTrue();
        _facilityRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

