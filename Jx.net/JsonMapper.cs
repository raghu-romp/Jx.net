using JUST;

namespace Jx.net
{
    public static class JsonMapper
    {
        public static ValueMapper Mapper { get; private set; }

        static JsonMapper()
        {
            FactoryReset();
        }

        public static void FactoryReset()
        {
            Mapper = new ValueMapper();
            RegisterValueMapper();
        }

        private static void RegisterValueMapper()
        {
            var myself = typeof(JsonMapper);
            var assemblyName = myself.Assembly.GetName().Name;
            var className = $"{myself.Namespace}.{myself.Name}";
            JUSTContext.ClearCustomFunctionRegistrations();
            JUSTContext.RegisterCustomFunction(assemblyName, className, nameof(MapValue));
        }

        public static string Map(string sourceJson, string mapperJson)
        {
            return JsonTransformer.Transform(mapperJson, sourceJson);
        }

        public static dynamic MapValue(string mapperName, string fromValue)
        {
            return Mapper.MapValue(mapperName, fromValue);
        }
    }
}
