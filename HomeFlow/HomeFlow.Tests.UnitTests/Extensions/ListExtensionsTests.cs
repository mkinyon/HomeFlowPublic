using HomeFlow.Extensions;
using HomeFlow.Interfaces;

namespace HomeFlow.Tests.UnitTests.Extensions;

public class ListExtensionsTests
{
    public class TestItem : IOrderable
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    [Fact]
    public void Reorder_MovesItemCorrectly_AndUpdatesOrder()
    {
        // Arrange
        var list = new List<TestItem>
        {
            new TestItem { Name = "A", Order = 0 },
            new TestItem { Name = "B", Order = 1 },
            new TestItem { Name = "C", Order = 2 }
        };

        // Act
        list.Reorder( 0, 2 ); // Move "A" from index 0 to index 2

        // Assert
        Assert.Equal( "B", list[0].Name );
        Assert.Equal( "C", list[1].Name );
        Assert.Equal( "A", list[2].Name );

        Assert.Equal( 0, list[0].Order );
        Assert.Equal( 1, list[1].Order );
        Assert.Equal( 2, list[2].Order );
    }

    [Fact]
    public void Reorder_SameIndex_DoesNothing()
    {
        var list = new List<TestItem>
        {
            new TestItem { Name = "X", Order = 0 },
            new TestItem { Name = "Y", Order = 1 }
        };

        list.Reorder( 1, 1 );

        Assert.Equal( "X", list[0].Name );
        Assert.Equal( "Y", list[1].Name );
        Assert.Equal( 0, list[0].Order );
        Assert.Equal( 1, list[1].Order );
    }

    [Fact]
    public void Reorder_NullList_Throws()
    {
        List<TestItem> list = null!;
        Assert.Throws<ArgumentNullException>( () => list.Reorder( 0, 1 ) );
    }

    [Theory]
    [InlineData( -1, 1 )]
    [InlineData( 3, 0 )]
    public void Reorder_InvalidOldIndex_Throws( int oldIndex, int newIndex )
    {
        var list = new List<TestItem>
        {
            new TestItem(),
            new TestItem(),
            new TestItem()
        };

        Assert.Throws<ArgumentOutOfRangeException>( () => list.Reorder( oldIndex, newIndex ) );
    }

    [Theory]
    [InlineData( 0, -1 )]
    [InlineData( 1, 5 )]
    public void Reorder_InvalidNewIndex_Throws( int oldIndex, int newIndex )
    {
        var list = new List<TestItem>
        {
            new TestItem(),
            new TestItem(),
            new TestItem()
        };

        Assert.Throws<ArgumentOutOfRangeException>( () => list.Reorder( oldIndex, newIndex ) );
    }
}