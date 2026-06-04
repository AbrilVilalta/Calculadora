using System;
using System.Data;
using System.Text.RegularExpressions;

namespace Calculadora.Models
{
    internal class CalculadoraCore
    {
        public string Operacio { get; set; }
        private double resultat;

        public CalculadoraCore()
        {
            Operacio = "0";
        }

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

        public string Resultat()
        {
            try
            {
                string expressioTraduida = Operacio;

                while (expressioTraduida.Contains("**") || expressioTraduida.Contains("√"))
                {
                    // Potències
                    expressioTraduida = Regex.Replace(
                        expressioTraduida,
                        @"(\d+\.?\d*)\*\*(\d+\.?\d*)",
                        m => Math.Pow(
                            double.Parse(m.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture)
                        ).ToString(System.Globalization.CultureInfo.InvariantCulture)
                    );

                    // Arrels — avalua el contingut del parèntesi primer
                    expressioTraduida = Regex.Replace(
                        expressioTraduida,
                        @"√\(([^)]+)\)",
                        m => {
                            double valorInterior = Convert.ToDouble(
                                new DataTable().Compute(m.Groups[1].Value, null)
                            );
                            return Math.Sqrt(valorInterior)
                                       .ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    );
                }

                resultat = Convert.ToDouble(new DataTable().Compute(expressioTraduida, null));

                if (double.IsInfinity(resultat) || double.IsNaN(resultat))
                    return "Error";

                Operacio = resultat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                return Operacio;
            }
            catch
            {
                return "Error";
            }
        }
    }
}