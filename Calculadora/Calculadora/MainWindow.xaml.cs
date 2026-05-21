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

        /// <summary>
        /// Inicialitza els components de la finestra principal.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// S'executa quan el TextBox es carrega. Posa el focus al camp d'entrada.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de l'event.</param>
        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            in_out.Focus();
        }

        /// <summary>
        /// Valida l'entrada de text abans d'inserir-la. Només permet dígits i operadors bàsics.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de composició de text.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            if (in_out.Text == "Error") in_out.Text = "";

            // Només permet dígits i operadors
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9\+\-\*\/]$");
        }

        /// <summary>
        /// Detecta la tecla Enter per executar el càlcul.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de la tecla premuda.</param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // Crida el mateix mètode que el botó "="
                IgualClick(sender, e);
            }
        }

        /// <summary>
        /// Manté el cursor sempre al final del text quan canvia el contingut.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments del canvi de text.</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Cursor sempre al final
            in_out.CaretIndex = in_out.Text.Length;
        }

        /// <summary>
        /// Ajusta la mida de la font dinàmicament segons la longitud del text.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments del canvi de text.</param>
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

        /// <summary>
        /// Afegeix el dígit del botó premut al camp d'entrada.
        /// </summary>
        /// <param name="sender">El botó numèric premut.</param>
        /// <param name="e">Arguments de l'event.</param>
        private void NumeroClick(object sender, RoutedEventArgs e)
        {
            if (in_out.Text == "Error") in_out.Text = "";
            if (in_out.Text == "0") in_out.Text = "";

            Button btn = (Button)sender;
            int num = int.Parse(btn.Tag.ToString() ?? "0");
            in_out.Text += num.ToString();
        }

        /// <summary>
        /// Afegeix l'operador del botó premut al camp d'entrada.
        /// </summary>
        /// <param name="sender">El botó d'operació premut.</param>
        /// <param name="e">Arguments de l'event.</param>
        private void OperacioClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (in_out.Text == "Error") in_out.Text = "";

            string operador = btn.Tag.ToString() ?? "0";
            in_out.Text += operador.ToString();
        }

        /// <summary>
        /// Esborra tot el contingut del camp d'entrada i el text d'operació.
        /// </summary>
        /// <param name="sender">El botó d'esborrar premut.</param>
        /// <param name="e">Arguments de l'event.</param>
        private void WipeOut(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            in_out.Text = "";
            operacioText.Text = "";
        }

        /// <summary>
        /// Avalua l'expressió matemàtica del camp d'entrada i mostra el resultat.
        /// Mostra "Error" si l'expressió és invàlida o el resultat és infinit o NaN.
        /// </summary>
        /// <param name="sender">El botó "=" o la tecla Enter.</param>
        /// <param name="e">Arguments de l'event.</param>
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