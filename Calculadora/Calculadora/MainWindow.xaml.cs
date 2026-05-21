using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculadora
{

    public partial class MainWindow : Window
    {
        private double result;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            in_out.Focus();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            if (in_out.Text == "Error") in_out.Text = "";

            // Només permet dígits i operadors
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9\+\-\*\/]$");
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // Crida el mateix mètode que el botó "="
                IgualClick(sender, e);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Cursor sempre al final
            in_out.CaretIndex = in_out.Text.Length;
        }

        private void in_out_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (in_out.Text.Length < 6)
                in_out.FontSize = 50;
            else if (in_out.Text.Length < 10)
                in_out.FontSize = 35;
            else if (in_out.Text.Length < 20)
                in_out.FontSize = 25;
            else
                in_out.FontSize = 18;
        }

        private void NumeroClick(object sender, RoutedEventArgs e)
        {
            if (in_out.Text == "Error") in_out.Text = "";
            if (in_out.Text == "0") in_out.Text = "";

            Button btn = (Button)sender;
            int num = int.Parse(btn.Tag.ToString() ?? "0");
            in_out.Text += num.ToString();
        }

        private void WipeOut(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            in_out.Text = "";
            operacioText.Text = "";
        }

        private void OperacioClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (in_out.Text == "Error") in_out.Text = "";

            string operador = btn.Tag.ToString() ?? "0";
            in_out.Text += operador.ToString();
        }

        private void IgualClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string operacio = in_out.Text;

                double result = Convert.ToDouble(
                    new DataTable().Compute(operacio, null)
                );

                operacioText.Text = operacio;

                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    in_out.Text = "Error";
                    return;
                }

                // Força el punt com a separador decimal
                in_out.Text = result.ToString(System.Globalization.CultureInfo.InvariantCulture);
                in_out.CaretIndex = in_out.Text.Length;
            }
            catch
            {
                in_out.Text = "Error";
                in_out.CaretIndex = in_out.Text.Length;
            }
        }

    }
}