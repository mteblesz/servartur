using servartur.Algorithms;

namespace servartur.Tests;
public class ListExtensionPopTests
{
    [Fact]
    public void Pop_RemovesFirstElementFromList_ReturnsRemovedElement()
    {
        // Arrange
        int elementToPop = 0;
        List<int> backOfList = [1, 2, 3];
        List<int> list = [elementToPop, ..backOfList];

        // Act
        var result = list.Pop();

        // Assert
        result.Should().Be(elementToPop);
        list.Should().BeEquivalentTo(backOfList);
    }

    [Fact]
    public void Pop_OnEmptyList_ThrowsException()
    {
        // Arrange
        var emptyList = new List<int>();

        // Act and Assert
        Action action = () => emptyList.Pop();
        Assert.Throws<InvalidOperationException>(action);
    }
}
