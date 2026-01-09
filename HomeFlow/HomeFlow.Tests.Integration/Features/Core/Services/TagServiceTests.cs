using FluentAssertions;
using HomeFlow.Data;
using HomeFlow.Features.Core.Tags;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HomeFlow.Tests.Integration.Features.Core.Services;

public class TagServiceTests
{
    private DbContextOptions<HomeFlowDbContext> CreateInMemoryOptions() =>
        new DbContextOptionsBuilder<HomeFlowDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString() )
            .Options;

    [Fact]
    public async Task UpdateAsync_ShouldAddNewTags_WhenUsingInMemoryDb()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var entityType = "Recipe";
        var newTags = new List<string> { "Tag1", "Tag2" };

        var options = CreateInMemoryOptions();
        using var context = new HomeFlowDbContext( options );

        // Add existing tag
        context.Tags.Add( new TagEntity 
        { 
            Id = Guid.NewGuid(), 
            EntityType = entityType, 
            EntityId = entityId, 
            Name = "Tag1" 
        } );
        await context.SaveChangesAsync();

        // Create MediatR mock
        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup( m => m.Send( It.IsAny<UpdateTagsCommand>(), It.IsAny<CancellationToken>() ) )
            .Returns( Task.CompletedTask );

        var tagService = new TagService( mockMediator.Object );

        // Act
        await tagService.UpdateAsync( entityType, entityId, newTags );

        // Assert
        mockMediator.Verify( m => m.Send( 
            It.Is<UpdateTagsCommand>( cmd => 
                cmd.EntityTypeName == entityType && 
                cmd.EntityId == entityId && 
                cmd.Tags.Count == 2 &&
                cmd.Tags.Contains( "Tag1" ) &&
                cmd.Tags.Contains( "Tag2" ) ), 
            It.IsAny<CancellationToken>() ), 
            Times.Once );
    }

    [Fact]
    public async Task UpdateAsync_ShouldRemoveOldTags_WhenUsingInMemoryDb()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var entityType = "Recipe";
        var newTags = new List<string> { "Tag2" }; // Tag1 should be removed

        // Create MediatR mock
        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup( m => m.Send( It.IsAny<UpdateTagsCommand>(), It.IsAny<CancellationToken>() ) )
            .Returns( Task.CompletedTask );

        var tagService = new TagService( mockMediator.Object );

        // Act
        await tagService.UpdateAsync( entityType, entityId, newTags );

        // Assert
        mockMediator.Verify( m => m.Send( 
            It.Is<UpdateTagsCommand>( cmd => 
                cmd.EntityTypeName == entityType && 
                cmd.EntityId == entityId && 
                cmd.Tags.Count == 1 &&
                cmd.Tags.Contains( "Tag2" ) ), 
            It.IsAny<CancellationToken>() ), 
            Times.Once );
    }
}
