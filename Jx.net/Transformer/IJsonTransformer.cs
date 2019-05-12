using Newtonsoft.Json.Linq;

namespace Jx.net.Transformer
{
    public interface IJsonTransformer
    {
        TransformOptions Options { get; set; }
        JToken Transform(JToken source, JToken transformer);
        void AddPipe(IValuePipe pipe);
    }
}