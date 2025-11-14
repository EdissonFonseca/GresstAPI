using FluentAssertions;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Moq;
using Xunit;

namespace Gresst.Tests.Application.Services;

/// <summary>
/// Unit tests for WasteClassService
/// </summary>
public class WasteClassServiceTests
{
    private readonly Mock<IRepository<WasteClass>> _wasteClassRepositoryMock;
    private readonly Mock<IRepository<PersonWasteClass>> _personWasteClassRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly WasteClassService _wasteClassService;

    public WasteClassServiceTests()
    {
        _wasteClassRepositoryMock = new Mock<IRepository<WasteClass>>();
        _personWasteClassRepositoryMock = new Mock<IRepository<PersonWasteClass>>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _wasteClassService = new WasteClassService(
            _wasteClassRepositoryMock.Object,
            _personWasteClassRepositoryMock.Object,
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserServiceMock.Object
        );
    }

    [Fact]
    public async Task GetAllWasteClassesAsync_ReturnsAllWasteClasses()
    {
        // Arrange
        var wasteClasses = new List<WasteClass>
        {
            new WasteClass
            {
                Id = Guid.NewGuid(),
                Name = "Organic Waste",
                Code = "ORG-001",
                IsActive = true
            },
            new WasteClass
            {
                Id = Guid.NewGuid(),
                Name = "Plastic Waste",
                Code = "PLA-001",
                IsActive = true
            }
        };

        _wasteClassRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(wasteClasses);

        // Act
        var result = await _wasteClassService.GetAllWasteClassesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(w => w.Name == "Organic Waste");
        result.Should().Contain(w => w.Name == "Plastic Waste");
    }

    [Fact]
    public async Task GetWasteClassByIdAsync_WhenExists_ReturnsWasteClassDto()
    {
        // Arrange
        var wasteClassId = Guid.NewGuid();
        var wasteClass = new WasteClass
        {
            Id = wasteClassId,
            Name = "Organic Waste",
            Code = "ORG-001",
            IsActive = true
        };

        _wasteClassRepositoryMock
            .Setup(r => r.GetByIdAsync(wasteClassId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wasteClass);

        // Act
        var result = await _wasteClassService.GetWasteClassByIdAsync(wasteClassId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(wasteClassId);
        result.Name.Should().Be("Organic Waste");
        result.Code.Should().Be("ORG-001");
    }

    [Fact]
    public async Task GetWasteClassByIdAsync_WhenDoesNotExist_ReturnsNull()
    {
        // Arrange
        var wasteClassId = Guid.NewGuid();

        _wasteClassRepositoryMock
            .Setup(r => r.GetByIdAsync(wasteClassId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WasteClass?)null);

        // Act
        var result = await _wasteClassService.GetWasteClassByIdAsync(wasteClassId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateWasteClassAsync_WhenValidDto_CreatesWasteClass()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var createDto = new CreateWasteClassDto
        {
            Name = "New Waste Class",
            Code = "NWC-001",
            ClassificationId = Guid.NewGuid()
        };

        var createdWasteClass = new WasteClass
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Code = createDto.Code,
            IsActive = true
        };

        _currentUserServiceMock
            .Setup(s => s.GetCurrentAccountId())
            .Returns(accountId);

        var accountPersonId = Guid.NewGuid();
        _accountRepositoryMock
            .Setup(r => r.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Account { Id = accountId, PersonId = accountPersonId });

        _wasteClassRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<WasteClass>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WasteClass wc, CancellationToken ct) => wc);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _wasteClassService.CreateWasteClassAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.Code.Should().Be(createDto.Code);
        _wasteClassRepositoryMock.Verify(r => r.AddAsync(It.IsAny<WasteClass>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWasteClassAsync_WhenExists_UpdatesWasteClass()
    {
        // Arrange
        var wasteClassId = Guid.NewGuid();
        var existingWasteClass = new WasteClass
        {
            Id = wasteClassId,
            Name = "Old Name",
            Code = "OLD-001",
            IsActive = true
        };

        var updateDto = new UpdateWasteClassDto
        {
            Id = wasteClassId,
            Name = "Updated Name"
        };

        _wasteClassRepositoryMock
            .Setup(r => r.GetByIdAsync(wasteClassId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingWasteClass);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _wasteClassService.UpdateWasteClassAsync(updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        _wasteClassRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<WasteClass>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteWasteClassAsync_WhenExists_DeletesWasteClass()
    {
        // Arrange
        var wasteClassId = Guid.NewGuid();
        var wasteClass = new WasteClass
        {
            Id = wasteClassId,
            Name = "Test Waste Class",
            Code = "TWC-001",
            IsActive = true
        };

        _wasteClassRepositoryMock
            .Setup(r => r.GetByIdAsync(wasteClassId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wasteClass);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _wasteClassService.DeleteWasteClassAsync(wasteClassId);

        // Assert
        result.Should().BeTrue();
        _wasteClassRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<WasteClass>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

