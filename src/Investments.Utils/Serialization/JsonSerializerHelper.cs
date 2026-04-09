using System.Text.Json;

namespace Investments.Utils.Serialization
{
	/// <summary>
	/// Helper class to simplify using of default serialization settings. 
	/// Consider removing this when this issue is closed: https://github.com/dotnet/runtime/issues/31094
	/// </summary>
	public static class JsonSerializerHelper
	{
		private static readonly JsonSerializerOptions _serializerOptions = 
			new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

		public static T? Deserialize<T>(string json)
		{
			return JsonSerializer.Deserialize<T>(json, _serializerOptions);
		}

		public static string Serialize<T>(T obj)
		{
			return JsonSerializer.Serialize<T>(obj, _serializerOptions);
		}
	}
}
