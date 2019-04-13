using Newtonsoft.Json.Linq;

namespace Jx.net.Transformer
{
    public interface IJsonTransformer
    {
        bool SuppressErrors { get; set; }

        JToken Transform(JToken source, JToken transformer);
    }
}