using servartur.Utils;

namespace servartur.Tests;

internal class ListExtensionPopTests
{
    [Fact]
    public void PopRemovesFirstElementFromListReturnsRemovedElement()
    {
        // Arrange
        var elementToPop = 0;
        List<int> backOfList = [1, 2, 3];
        List<int> list = [elementToPop, .. backOfList];

        // Act
        var result = list.Pop();

        // Assert
        result.Should().Be(elementToPop);
        list.Should().BeEquivalentTo(backOfList);
    }

    [Fact]
    public void PopOnEmptyListThrowsException()
    {
        // Arrange
        var emptyList = new List<int>();

        // Act and Assert
        void action() => emptyList.Pop();
        Assert.Throws<InvalidOperationException>(action);
    }
}
