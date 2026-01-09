using FluentAssertions;
using FluentValidation;
using HomeFlow.Behaviors;
using HomeFlow.Data;
using HomeFlow.Features.MealPlanning.GroceryItems;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ValidationException = HomeFlow.Exceptions.ValidationException;

namespace HomeFlow.Tests.IntegrationTests.Features.MealPlanning.Commands;

public class CreateGroceryItemCommandHandlerTests
{
    private static DbContextOptions<HomeFlowDbContext> CreateInMemoryOptions() =>
        new DbContextOptionsBuilder<HomeFlowDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString() )
            .Options;

    private static IServiceProvider CreateServiceProvider( DbContextOptions<HomeFlowDbContext> options )
    {
        var services = new ServiceCollection();

        // Use the same database name as the options to ensure the same in-memory database is used.
        var inMemoryDatabaseName = Guid.NewGuid().ToString();
        services.AddDbContext<HomeFlowDbContext>( opt => opt.UseInMemoryDatabase( inMemoryDatabaseName ) );
        services.AddScoped<IHomeFlowDbContext, HomeFlowDbContext>();
        services.AddLogging(); // Required for MediatR 13.0.0
        services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( CreateGroceryItemCommand ).Assembly ) );
        services.AddValidatorsFromAssembly( typeof( CreateGroceryItemCommand ).Assembly );
        services.AddTransient( typeof( IPipelineBehavior<,> ), typeof( ValidationBehavior<,> ) );

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Handle_ShouldCreateGroceryItem_WhenValid()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var serviceProvider = CreateServiceProvider( options );

        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<HomeFlowDbContext>();

        var request = new CreateGroceryItemCommand( new GroceryItem { Name = "Onion" } );

        // Act
        var resultId = await mediator.Send( request );

        // Assert
        var entity = await context.GroceryItems.FindAsync( resultId );
        entity.Should().NotBeNull();
        entity!.Name.Should().Be( "Onion" );
        resultId.Should().NotBe( Guid.Empty );
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvalid()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var serviceProvider = CreateServiceProvider( options );

        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<HomeFlowDbContext>();

        var request = new CreateGroceryItemCommand( new GroceryItem { Name = string.Empty } );

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>( () => mediator.Send( request ) );

        (await context.GroceryItems.ToListAsync()).Should().BeEmpty();
    }
}