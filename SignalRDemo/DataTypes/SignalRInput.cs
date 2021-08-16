using System.Collections.Generic;
using System.Text.Json;

namespace SignalRDemo.DataTypes
{
    public class SignalRInput
    {
        public List<JsonElement> Arguments { get; set; }
        public object Error { get; set; }
        public string Category { get; set; }
        public string Event { get; set; }
        public string Hub { get; set; }
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Query Query { get; set; }
        public Claims Claims { get; set; }
    }

    public class Query
    {
        public string hub { get; set; }
        public string id { get; set; }
        public string access_token { get; set; }
    }

    public class Claims
    {
        public string nbf { get; set; }
        public string exp { get; set; }
        public string iat { get; set; }
        public string aud { get; set; }
    }
}