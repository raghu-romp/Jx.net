using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.Formulas
{
    public interface IFormulaSolver
    {
        T Solve<T>(string expression, Dictionary<string, dynamic> variables = null);
    }
}
