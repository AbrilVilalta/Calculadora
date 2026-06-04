using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Calculadora.Models;

namespace PAC5_Calculadora.Tests
{
    public class CalculadoraCoreTests
    {
        private readonly CalculadoraCore _calculadora;
        public CalculadoraCoreTests()
        {
            _calculadora = new CalculadoraCore();
        }
        [Fact]
        public void TestSuma_Success()
        {
            _calculadora.Operacions("5");
            _calculadora.Operacions("+");
            _calculadora.Operacions("3");
            var resultat = _calculadora.Resultat();
            Assert.Equal("8", resultat);
        }
        [Fact]
        public void TestResta_Success()
        {
            _calculadora.Operacions("5");
            _calculadora.Operacions("-");
            _calculadora.Operacions("3");
            var resultat = _calculadora.Resultat();
            Assert.Equal("2", resultat);
        }
        [Fact]
        public void TestMultiplicacio_Success()
        {
            _calculadora.Operacions("5");
            _calculadora.Operacions("*");
            _calculadora.Operacions("3");
            var resultat = _calculadora.Resultat();
            Assert.Equal("15", resultat);
        }

        [Fact]
        public void TestDivisio_Success()
        {
            _calculadora.Operacions("5");
            _calculadora.Operacions("/");
            _calculadora.Operacions("2");
            var resultat = _calculadora.Resultat();
            Assert.Equal("2.5", resultat);
        }
        [Fact]
        public void TestDivisio_PerZero_ThrowsException()
        {
            _calculadora.Operacio = "5/0";
            Assert.Throws<DivideByZeroException>(() => _calculadora.Resultat());
        }

        //01. [SUCCESS] 5 + 3 * 2 - 4 / 2 = 9

        [Fact]
        public void TestPrioritatOperadors_Success()
        {
            _calculadora.Operacio = "5+3*2-4/2";
            var resultat = _calculadora.Resultat();
            Assert.Equal("9", resultat);
        }

        //02. [SUCCESS] 2 ^ 3 = 8

        [Fact]
        public void TestPotencia_Success()
        {
            _calculadora.Operacio = "2**3";
            var resultat = _calculadora.Resultat();
            Assert.Equal("8", resultat);
        }

        //03. [SUCCESS] 0 ^ 5 = 0

        [Fact]
        public void TestPotenciaBaseZero_Success()
        {
            _calculadora.Operacio = "0**5";
            var resultat = _calculadora.Resultat();
            Assert.Equal("0", resultat);
        }

        //04. [SUCCESS] √ 16 = 4

        [Fact]
        public void TestArrel_Success()
        {
            _calculadora.Operacio = "√(16)";
            var resultat = _calculadora.Resultat();
            Assert.Equal("4", resultat);
        }

        //05. [SUCCESS] √ 0 = 0

        [Fact]
        public void TestArrelBaseZero_Success()
        {
            _calculadora.Operacio = "√(0)";
            var resultat = _calculadora.Resultat();
            Assert.Equal("0", resultat);
        }

        //06. [FORMATEXCEPTION] 3 √ 6 = FAIL

        [Fact]
        public void TestArrelAmbOperandEsquerra_ThrowsFormatException()
        {
            _calculadora.Operacio = "3√6";
            Assert.Throws<FormatException>(() => _calculadora.Resultat());
        }

        //07. [SUCCESS] 3 * √ 9 = 9

        [Fact]
        public void TestMultiplicacioAmbArrel_Success()
        {
            _calculadora.Operacio = "3*√(9)";
            var resultat = _calculadora.Resultat();
            Assert.Equal("9", resultat);
        }

        //08. [INVALIDOPERATIONEXCEPTION] √ -4 = FAIL

        [Fact]
        public void TestArrelNegatiu_ThrowsInvalidOperationException()
        {
            _calculadora.Operacio = "√(-4)";
            Assert.Throws<InvalidOperationException>(() => _calculadora.Resultat());
        }

        //09. [SUCCESS] ( 5 + 3 ) * 2 = 16

        [Fact]
        public void TestParentesis_Success()
        {
            _calculadora.Operacio = "(5+3)*2";
            var resultat = _calculadora.Resultat();
            Assert.Equal("16", resultat);
        }

        //10. [SUCCESS] ( ( 2 + 3 ) * 2 ) = 10

        [Fact]
        public void TestParentesisDobles_Success()
        {
            _calculadora.Operacio = "((2+3)*2)";
            var resultat = _calculadora.Resultat();
            Assert.Equal("10", resultat);
        }

        //11. [FORMATEXCEPTION] 5 + ( 3 * 2 = FAIL

        [Fact]
        public void TestParentesisObert_ThrowsFormatException()
        {
            _calculadora.Operacio = "5+(3*2";
            Assert.Throws<FormatException>(() => _calculadora.Resultat());
        }

        //12. [FORMATEXCEPTION] 5 + 3 ) * 2 = FAIL

        [Fact]
        public void TestParentesisTancat_ThrowsFormatException()
        {
            _calculadora.Operacio = "5+3)*2";
            Assert.Throws<FormatException>(() => _calculadora.Resultat());
        }

        //13. [SUCCESS] 3 + 5 * ( 2 ^ 3 ) - √ 16 = 39
        //16. [SUCCESS] 3 + 5 * ( 2 ^ 3 ) - √ 16 = 39

        [Fact]
        public void TestExpressioCompleta_Success()
        {
            _calculadora.Operacio = "3+5*(2**3)-√(16)";
            var resultat = _calculadora.Resultat();
            Assert.Equal("39", resultat);
        }

        //14. [FORMATEXCEPTION] 2(8 + 2) = FAIL

        [Fact]
        public void TestMultiplicacioImplicita_ThrowsFormatException()
        {
            _calculadora.Operacio = "2(8+2)";
            Assert.Throws<FormatException>(() => _calculadora.Resultat());
        }

        //15. [FORMATEXCEPTION] (8 + 2)2 = FAIL

        [Fact]
        public void TestMultiplicacioImplicitaDreta_ThrowsFormatException()
        {
            _calculadora.Operacio = "(8+2)2";
            Assert.Throws<FormatException>(() => _calculadora.Resultat());
        }

        //17. [FORMATEXCEPTION] (5)3 + 2 = FAIL

        [Fact]
        public void TestParentesisSeguitDeNumero_ThrowsFormatException()
        {
            _calculadora.Operacio = "(5)3+2";
            Assert.Throws<FormatException>(() => _calculadora.Resultat());
        }

        //18. [SUCCESS] 5 * 3 + 2 = 17

        [Fact]
        public void TestMultiplicacioISuma_Success()
        {
            _calculadora.Operacio = "5*3+2";
            var resultat = _calculadora.Resultat();
            Assert.Equal("17", resultat);
        }

    }
}
