using Flee.PublicTypes;
using Jx.net.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.Formulas
{
    public class FormulaSolver : IFormulaSolver
    {

        public T Solve<T>(string expression, Dictionary<string, dynamic> variables = null) {
            var context = CreateExpressionContext();
            if (variables != null) {
                variables.Each(x => context.Variables[x.Key] = x.Value);
            }

            var compilied = context.CompileDynamic(expression);
            return (T)compilied.Evaluate();
        }

        private static ExpressionContext CreateExpressionContext() {
            var context = new ExpressionContext();
            context.Imports.AddType(typeof(Math));

            //this.additionalTypesToRegister.ForEach(type => context.Imports.AddType(type.Value, type.Name));

            return context;
        }
    }
}
