using Microsoft.AspNetCore.Authorization.Infrastructure;
using servartur.DomainLogic;
using servartur.Enums;
using servartur.Models;
using servartur.Services;

namespace servartur.Tests;
using RoleInfo = GameStartHelper.RoleInfo;
public class GameStartHelperTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "style is ok now")]
    public static TheoryData<int, RoleInfo, List<Role>>  ValidTestCases()
    {
        var data = new TheoryData<int, RoleInfo, List<Role>>();
        data.Add( 10, new RoleInfo(true, true, true), [
            Role.Merlin, Role.Percival, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight,
            Role.Assassin, Role.Morgana, Role.Mordred, Role.Oberon,
        ] );
        data.Add( 10, new RoleInfo(true, true, false), [
            Role.Merlin, Role.Percival, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight,
            Role.Assassin, Role.Morgana, Role.EvilEntity, Role.EvilEntity,
        ] );
        data.Add( 10, new RoleInfo(true, false, true), [
            Role.Merlin, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight,
            Role.Assassin, Role.EvilEntity, Role.Mordred, Role.Oberon,
        ] );
        data.Add( 9, new RoleInfo(false, false, false), [
            Role.GoodKnight, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight,
            Role.EvilEntity, Role.EvilEntity, Role.EvilEntity, Role.EvilEntity,
        ] );
        data.Add( 7, new RoleInfo(true, true, false), [
            Role.Merlin, Role.Percival, Role.GoodKnight, Role.GoodKnight,
            Role.Assassin, Role.Morgana, Role.EvilEntity,
        ] );
        data.Add( 7, new RoleInfo(true, false, true), [
            Role.Merlin, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight,
            Role.Assassin, Role.Mordred, Role.Oberon,
        ] );
        data.Add( 6, new RoleInfo(true, false, false), [
            Role.Merlin, Role.GoodKnight, Role.GoodKnight, Role.GoodKnight,
            Role.Assassin, Role.EvilEntity,
        ] );
        data.Add( 5, new RoleInfo(true, true, false), [
            Role.Merlin, Role.Percival, Role.GoodKnight, 
            Role.Assassin, Role.Morgana, 
        ] );
        data.Add( 5, new RoleInfo(false, false, false), [
            Role.GoodKnight, Role.GoodKnight, Role.GoodKnight,
            Role.EvilEntity, Role.EvilEntity,
        ] );
        return data;
    }
    [Theory]
    [MemberData(nameof(ValidTestCases))]
    public void MakeRoleDeck_ShouldGenerateValidRoleDeck(int numberOfPlayers, RoleInfo roleInfo, List<Role> expectedResult)
    {
        // Arrange
        // Act
        var result = GameStartHelper.MakeRoleDeck(numberOfPlayers, roleInfo, out bool tooManyEvilRoles);

        // Assert
        tooManyEvilRoles.Should().BeFalse();
        result.Should().NotBeNull();
        result.Should().HaveCount(numberOfPlayers);
        result.Should().BeEquivalentTo(expectedResult, options => options.WithoutStrictOrdering());
    }

    public static TheoryData<int, RoleInfo> InvalidTestCases()
        => new TheoryData<int, RoleInfo> {
            {5, new RoleInfo(true, false, true) },
            {6, new RoleInfo(true, false, true) },
            {7, new RoleInfo(true, true, true) },
            {8, new RoleInfo(true, true, true) },};
    [Theory]
    [MemberData(nameof(InvalidTestCases))]
    public void MakeRoleDeck_TooManyEvilRoles_ReturnsEmptyListAndSetsFlag(int numberOfPlayers, RoleInfo roleInfo)
    {
        // Arrange
        // Act
        var result = GameStartHelper.MakeRoleDeck(numberOfPlayers, roleInfo, out bool tooManyEvilRoles);

        // Assert
        tooManyEvilRoles.Should().BeTrue();
        result.Should().BeNullOrEmpty();
    }

    [Fact]
    public void MakeRoleDeck_InvalidNumberOfPlayers_ThrowsArgumentException()
    {
        // Arrange
        int invalidPlayerCount = -1;
        RoleInfo roleInfo = new RoleInfo(true, true, true);

        // Act & Assert
        Action action = () => GameStartHelper.MakeRoleDeck(invalidPlayerCount, roleInfo, out _);
        Assert.Throws<ArgumentException>(action);
    }
}