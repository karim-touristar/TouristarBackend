using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TouristarBackend.Constants;

public class SerialisationConstants
{
    static readonly JsonSerializerSettings _snakeSettings = new()
    {
        ContractResolver = new UnderscorePropertyNamesContractResolver()
    };

    public static JsonSerializerSettings SnakeCaseSettings => _snakeSettings;
}

public class UnderscorePropertyNamesContractResolver : DefaultContractResolver
{
    public UnderscorePropertyNamesContractResolver()
    {
        NamingStrategy = new SnakeCaseNamingStrategy();
    }
}