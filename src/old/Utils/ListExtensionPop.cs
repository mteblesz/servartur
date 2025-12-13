namespace servartur.Utils;

internal static class ListExtensionPop
{
    public static T Pop<T>(this List<T> list)
    {
        var elem = list.First();
        list.RemoveAt(0);
        return elem;
    }
}
