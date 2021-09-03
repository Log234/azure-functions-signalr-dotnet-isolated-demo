using System.Text.Json;

namespace SignalRDemo
{
    public static class JsonElementExtensionMethods
    {
        public static T Parse<T>(this JsonElement element)
        {
            return JsonSerializer.Deserialize<T>(element.GetRawText());
        }
    }
}