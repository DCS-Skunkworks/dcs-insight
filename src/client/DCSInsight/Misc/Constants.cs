namespace DCSInsight.Misc
{
    internal static class Constants
    {
        internal const string ListEnvironmentSnippet = "local keys = {}\r\nfor k, v in pairs(_G) do\r\n\tkeys[#keys+1] = k  \r\nend\r\nreturn keys";
    }
}
