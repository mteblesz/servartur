using servartur.Utils;

namespace servartur.Tests;

#pragma warning disable CA1515 // Consider making public types internal
public class ListExtensionPopTests
#pragma warning restore CA1515 // Consider making public types internal
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
