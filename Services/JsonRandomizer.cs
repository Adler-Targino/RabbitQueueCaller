using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace RabbitQueueCaller.Services
{
    internal class JsonRandomizer
    {
        private static readonly Random Random = new();

        public static string Randomize(string json)
        {
            var node = JsonNode.Parse(json);

            ProcessNode(node);

            return node!.ToJsonString(new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        private static void ProcessNode(JsonNode? node)
        {
            if (node is JsonObject obj)
            {
                foreach (var item in obj)
                {
                    if (item.Value is JsonValue value &&
                        value.TryGetValue<string>(out var text))
                    {
                        obj[item.Key] = ReplaceTokens(text);
                    }
                    else
                    {
                        ProcessNode(item.Value);
                    }
                }
            }
            else if (node is JsonArray array)
            {
                foreach (var item in array)
                    ProcessNode(item);
            }
        }

        private static string ReplaceTokens(string text)
        {
            if (text == "{DateTime}")
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (text == "{Guid}")
                return Guid.NewGuid().ToString();

            if (text == "{Int}")
                return Random.Next().ToString();

            if (text == "{Decimal}")
                return (Random.NextDouble() * 1000).ToString("F2");

            if (text == "{Bool}")
                return (Random.Next(2) == 0).ToString().ToLower();

            bool useSha256 = text.Contains("(SHA256)");
            text = text.Replace("(SHA256)", "");

            var sb = new StringBuilder();

            foreach (char c in text)
            {
                switch (c)
                {
                    case 'x':
                        sb.Append((char)('A' + Random.Next(26)));
                        break;

                    case '9':
                        sb.Append(Random.Next(10));
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }

            string result = sb.ToString();

            if (useSha256)
            {
                byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(result));
                result = Convert.ToHexString(hash).ToLowerInvariant();
            }

            return result;
        }
    }
}
