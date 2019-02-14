using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net
{
    interface IJsonMapper
    {
        /// <summary>
        /// Deserializes sourceJson and mapperJson, transforms and serializes output json into a string
        /// </summary>
        /// <param name="sourceJson"></param>
        /// <param name="mapperJson"></param>
        /// <returns></returns>
        string Map(string sourceJson, string mapperJson);
    }
}
