using Jx.net.Formulas;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Jx.net.test
{
    public class FormulaSolverTests : IClassFixture<FormulaSolver>
    {
        private readonly IFormulaSolver _formulaSolver;

        public FormulaSolverTests(FormulaSolver formulaSolver)
        {
            _formulaSolver = formulaSolver;
        }
        
        [Theory]
        [InlineData("2*2", 4)]
        [InlineData("2+2.5", 4.5)]
        [InlineData("5-2", 3)]
        [InlineData("5.0/2", 2.5)]
        [InlineData("(2*5)+5", 15)]
        [InlineData("(5+2)^2", 49)]
        public void SolveFormula_EvaluatesMath(string formula, decimal expectedResult)
        {
            var actualResult = _formulaSolver.Solve<decimal>(formula);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void SolveFormula_WithVariables()
        {
            var expression = "(A+B)^2";
            var variables = new Dictionary<string, dynamic> {
                { "A", 5 },
                { "B", 2 }
            };

            var actualResult = _formulaSolver.Solve<decimal>(expression, variables);

            Assert.Equal(49, actualResult);
        }
    }
}
