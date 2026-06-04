using Calculadora.Models;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculadora
{

    public partial class MainWindow : Window
    {
        private CalculadoraCore calculadora;

        /// <summary>
        /// Inicialitza els components de la finestra principal.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            calculadora = new CalculadoraCore();
        }

        /// <summary>
        /// S'executa quan el TextBox es carrega. Posa el focus al camp d'entrada.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de l'event.</param>
        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            in_out.Focus();
            in_out.CaretIndex = in_out.Text.Length;
        }

        /// <summary>
        /// Valida l'entrada de text abans d'inserir-la. Només permet dígits i operadors bàsics.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de composició de text.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            if (in_out.Text == "Error" || in_out.Text == "0") in_out.Text = "";

            // Només permet dígits i operadors
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9\+\-\*\/ \(\)√]$");
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
                in_out.CaretIndex = in_out.Text.Length;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
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

        private void BotoClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string valor = btn.Tag.ToString() ?? "0";

            // Enviem el valor al core i actualitzem la pantalla amb el que ens respon
            in_out.Text = calculadora.Operacions(valor);

            // Si l'usuari ha premut esborrar, també netegem el text secundari de l'operació
            if (valor == "C")
            {
                operacioText.Text = "0";
            }

            in_out.CaretIndex = in_out.Text.Length;

        }

        private void IgualClick(object sender, RoutedEventArgs e)
        {
            operacioText.Text = in_out.Text;

            calculadora.Operacio = in_out.Text;

            in_out.Text = calculadora.Resultat();

            in_out.CaretIndex = in_out.Text.Length;
        }

    }
}