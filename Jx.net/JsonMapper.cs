using JUST;

namespace Jx.net
{
    public class JsonMapper : IJsonMapper
    {
        public JsonMapper()
        {
        }

        public string Map(string sourceJson, string mapperJson)
        {
            return JsonTransformer.Transform(mapperJson, sourceJson);
        }
    }
}
