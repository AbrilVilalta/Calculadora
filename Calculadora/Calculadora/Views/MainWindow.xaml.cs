using Calculadora.Models;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculadora
{
    /// <summary>
    /// Finestra principal de l'aplicació Calculadora.
    /// Gestiona la interfície d'usuari i la comunicació amb el nucli lògic <see cref="CalculadoraCore"/>.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Instància del nucli lògic de la calculadora.
        /// </summary>
        private CalculadoraCore calculadora;

        /// <summary>
        /// Inicialitza els components de la finestra principal
        /// i crea una nova instància de <see cref="CalculadoraCore"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            calculadora = new CalculadoraCore();
        }

        /// <summary>
        /// S'executa quan el <see cref="System.Windows.Controls.TextBox"/> es carrega.
        /// Posa el focus al camp d'entrada i situa el cursor al final del text.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de l'event.</param>
        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            in_out.Focus();
            in_out.CaretIndex = in_out.Text.Length;
        }

        /// <summary>
        /// Valida l'entrada de text abans d'inserir-la al camp.
        /// Només permet dígits, operadors bàsics i símbols matemàtics admesos.
        /// Si el camp conté "Error" o "0", el neteja abans d'inserir el nou caràcter.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de composició de text.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (in_out.Text == "Error" || in_out.Text == "0") in_out.Text = "";

            // Només permet dígits i operadors: 0-9, +, -, *, /, espai, (, ), √
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9\+\-\*\/ \(\)√]$");
        }

        /// <summary>
        /// Detecta la tecla <c>Enter</c> per executar el càlcul,
        /// equivalent a prémer el botó "=".
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de la tecla premuda.</param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                IgualClick(sender, e);
                in_out.CaretIndex = in_out.Text.Length;
            }
        }

        /// <summary>
        /// S'executa cada vegada que el text del camp canvia.
        /// Manté el cursor al final del text en tot moment.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments del canvi de text.</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            in_out.CaretIndex = in_out.Text.Length;
        }

        /// <summary>
        /// Ajusta dinàmicament la mida de la font del camp d'entrada
        /// segons la longitud del text, per evitar desbordaments visuals.
        /// </summary>
        /// <remarks>
        /// Escala de mides aplicada:
        /// <list type="bullet">
        ///   <item>Menys de 6 caràcters → 50px</item>
        ///   <item>Menys de 10 caràcters → 35px</item>
        ///   <item>Menys de 20 caràcters → 25px</item>
        ///   <item>20 o més caràcters → 18px</item>
        /// </list>
        /// </remarks>
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
        /// Gestiona el clic de qualsevol botó numèric o d'operació.
        /// Llegeix el valor del <c>Tag</c> del botó, l'envia al nucli lògic
        /// i actualitza la pantalla amb el resultat.
        /// Si es prem "C", també neteja el text secundari de l'operació.
        /// </summary>
        /// <param name="sender">El botó que ha estat premut.</param>
        /// <param name="e">Arguments de l'event de clic.</param>
        private void BotoClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string valor = btn.Tag.ToString() ?? "0";

            in_out.Text = calculadora.Operacions(valor);

            if (valor == "C")
            {
                operacioText.Text = "0";
            }

            in_out.CaretIndex = in_out.Text.Length;
        }

        /// <summary>
        /// Gestiona el clic del botó "=" o la tecla <c>Enter</c>.
        /// Copia l'expressió actual al text secundari, l'envia al nucli lògic
        /// per calcular-la i mostra el resultat al camp principal.
        /// </summary>
        /// <param name="sender">L'objecte que ha llançat l'event.</param>
        /// <param name="e">Arguments de l'event.</param>
        private void IgualClick(object sender, RoutedEventArgs e)
        {
            operacioText.Text = in_out.Text;
            calculadora.Operacio = in_out.Text;
            try
            {
                in_out.Text = calculadora.Resultat();
            }
            catch (DivideByZeroException)
            {
                in_out.Text = "No es pot dividir per 0";
            }
            catch (FormatException)
            {
                in_out.Text = "Error de format";
            }
            catch (InvalidOperationException)
            {
                in_out.Text = "Operacio invalida";
            }
            in_out.CaretIndex = in_out.Text.Length;
        }
    }
}