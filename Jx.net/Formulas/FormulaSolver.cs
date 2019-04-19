using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.Formulas
{
    public class FormulaSolver : IFormulaSolver
    {
        private static ExpressionContext context = new ExpressionContext();

        static FormulaSolver() {
            context = CreateExpressionContext();
        }

        public T Solve<T>(string expression, Dictionary<string, dynamic> variables) {
            throw new NotImplementedException();  
        }

        private static ExpressionContext CreateExpressionContext() {
            var context = new ExpressionContext();
            context.Imports.AddType(typeof(Math));

            //this.additionalTypesToRegister.ForEach(type => context.Imports.AddType(type.Value, type.Name));

            return context;
        }
    }
}
