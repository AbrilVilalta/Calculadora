using System;
using System.Data;
using System.Text.RegularExpressions;

namespace Calculadora.Models
{
    /// <summary>
    /// Nucli lògic de la calculadora.
    /// Gestiona l'entrada d'operacions i el càlcul de resultats,
    /// incloent suport per a potències (<c>**</c>) i arrels quadrades (<c>√</c>).
    /// </summary>
    public class CalculadoraCore
    {
        /// <summary>
        /// Expressió matemàtica actual que s'està construint o que s'ha calculat.
        /// Es pot llegir i modificar des de fora de la classe.
        /// </summary>
        public string Operacio { get; set; }

        /// <summary>
        /// Valor numèric del darrer resultat calculat.
        /// </summary>
        private double resultat;

        /// <summary>
        /// Inicialitza una nova instància de <see cref="CalculadoraCore"/>
        /// amb el valor per defecte "0".
        /// </summary>
        public CalculadoraCore()
        {
            Operacio = "0";
        }

        /// <summary>
        /// Processa l'entrada de l'usuari i actualitza l'expressió actual.
        /// </summary>
        /// <param name="input">
        /// Caràcter o operació introduïts per l'usuari.
        /// Valors especials:
        /// <list type="bullet">
        ///   <item><c>"C"</c> — reinicia l'expressió a "0".</item>
        ///   <item><c>"√"</c> — aplica l'arrel quadrada a l'últim número o parèntesi.</item>
        /// </list>
        /// </param>
        /// <returns>L'expressió actualitzada com a <see cref="string"/>.</returns>
        public string Operacions(string input)
        {
            if (input == "C")
            {
                Operacio = "0";
                return Operacio;
            }

            if (input == "√")
            {
                Operacio = AplicarArrel(Operacio);
                return Operacio;
            }

            if (Operacio == "0" || Operacio == "Error")
            {
                Operacio = input;
                return Operacio;
            }

            Operacio += input;
            return Operacio;
        }

        /// <summary>
        /// Transforma l'expressió actual afegint l'operació d'arrel quadrada
        /// al darrer número o al darrer grup entre parèntesis.
        /// </summary>
        /// <remarks>
        /// Exemples de transformació:
        /// <list type="bullet">
        ///   <item><c>"9"</c> → <c>"√(9)"</c></item>
        ///   <item><c>"2+9"</c> → <c>"2+√(9)"</c></item>
        ///   <item><c>"(2+7)"</c> → <c>"√(2+7)"</c></item>
        /// </list>
        /// </remarks>
        /// <param name="expressio">L'expressió actual sobre la qual s'aplica l'arrel.</param>
        /// <returns>L'expressió modificada amb la notació d'arrel quadrada.</returns>
        private string AplicarArrel(string expressio)
        {
            if (Regex.IsMatch(expressio, @"^\d+\.?\d*$"))
                return $"√({expressio})";

            var matchNumero = Regex.Match(expressio, @"(.*[+\-*/])(\d+\.?\d*)$");
            if (matchNumero.Success)
                return $"{matchNumero.Groups[1].Value}√({matchNumero.Groups[2].Value})";

            if (expressio.EndsWith(")"))
            {
                int depth = 0;
                for (int i = expressio.Length - 1; i >= 0; i--)
                {
                    if (expressio[i] == ')') depth++;
                    else if (expressio[i] == '(') depth--;

                    if (depth == 0)
                    {
                        string abans = expressio.Substring(0, i);
                        string parentesi = expressio.Substring(i);
                        return $"{abans}√{parentesi}";
                    }
                }
            }

            return $"√({expressio})";
        }

        private void ValidarExpressio(string expressio)
        {
            // Arrel sense parèntesis
            if (Regex.IsMatch(expressio, @"√(?!\()"))
                throw new FormatException("L'arrel quadrada ha d'anar seguida de parèntesi.");

            // Parèntesis desbalancejats
            int depth = 0;
            foreach (char c in expressio)
            {
                if (c == '(') depth++;
                else if (c == ')') depth--;
                if (depth < 0)
                    throw new FormatException("Parèntesi de tancament sense obertura.");
            }
            if (depth != 0)
                throw new FormatException("Parèntesi d'obertura sense tancament.");

            // Multiplicació implícita
            if (Regex.IsMatch(expressio, @"\d\s*\("))
                throw new FormatException("Multiplicació implícita no permesa: número seguit de '('.");
            if (Regex.IsMatch(expressio, @"\)\s*\d"))
                throw new FormatException("Multiplicació implícita no permesa: ')' seguit de número.");
        }

        /// <summary>
        /// Avalua l'expressió matemàtica actual i retorna el resultat.
        /// </summary>
        /// <remarks>
        /// El procés d'avaluació segueix aquests passos:
        /// <list type="number">
        ///   <item>Resol les potències (<c>base**exponent</c>) mitjançant <see cref="Math.Pow"/>.</item>
        ///   <item>Resol les arrels quadrades (<c>√(...)</c>) avaluant primer el contingut del parèntesi.</item>
        ///   <item>Passa l'expressió resultant a <see cref="DataTable.Compute"/> per calcular la resta.</item>
        /// </list>
        /// Retorna <c>"Error"</c> si l'expressió és invàlida, produeix infinit o NaN,
        /// o si es produeix qualsevol excepció durant el càlcul.
        /// </remarks>
        /// <returns>
        /// El resultat com a <see cref="string"/>, o <c>"Error"</c> si el càlcul falla.
        /// </returns>
        public string Resultat()
        {
            try
            {
                string expressioTraduida = Operacio;

                ValidarExpressio(expressioTraduida);

                while (expressioTraduida.Contains("**") || expressioTraduida.Contains("√"))
                {
                    expressioTraduida = Regex.Replace(
                        expressioTraduida,
                        @"(\d+\.?\d*)\*\*(\d+\.?\d*)",
                        m => Math.Pow(
                            double.Parse(m.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture)
                        ).ToString(System.Globalization.CultureInfo.InvariantCulture)
                    );

                    expressioTraduida = Regex.Replace(
                        expressioTraduida,
                        @"√\(([^)]+)\)",
                        m => {
                            double valorInterior = Convert.ToDouble(
                                new DataTable().Compute(m.Groups[1].Value, null)
                            );
                            if (valorInterior < 0)
                                throw new InvalidOperationException("No es pot calcular l'arrel d'un negatiu.");
                            return Math.Sqrt(valorInterior)
                                       .ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    );
                }

                resultat = Convert.ToDouble(new DataTable().Compute(expressioTraduida, null));

                if (double.IsInfinity(resultat) || double.IsNaN(resultat))
                    throw new DivideByZeroException("No es pot dividir per zero.");

                Operacio = resultat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                return Operacio;
            }
            catch (DivideByZeroException)
            {
                throw;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch
            {
                return "Error";
            }
        }
    }
}