using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace CodeCoverage
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string caminhoWorkspace = String.Empty;

        string caminhoCodeCovPaths = String.Empty;

        string caminhoCodeCovUnits = String.Empty;

        string caminhoExeAppTests = String.Empty;

        string caminhoMapAppTests = String.Empty;

        ArquivoJson arquivoJson;

        private void EscolherCaminho(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                Description = "Escolha a workspace",
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true
            };

            bool result = dialog.ShowDialog(this).GetValueOrDefault();

            if (result)
            {
                textCaminho.Text = dialog.SelectedPath.Trim().ToUpper();
            }
        }

        private void AbrirRelatorio(object sender, RoutedEventArgs e)
        {
            string caminhoRelatorio = textCaminho.Text.Trim() + @"\bin\CodeCoverage\Result_CodeCov\CodeCoverage_summary.html";

            if (!File.Exists(caminhoRelatorio))
            {
                MessageBox.Show("Arquivo não encontrado: " + caminhoRelatorio);
                return;
            }

            _ = Process.Start(@"cmd.exe ", @"/c " + caminhoRelatorio);
        }

        private void AtualizarComportamentosLista(object sender, RoutedEventArgs e)
        {
            textLista.IsEnabled = ((!checkListaPadrao.IsChecked) ?? false);
        }

        private void ExecutarCodeCoverage(object sender, RoutedEventArgs e)
        {
            if (!DefinirWorkspace())
                return;
            if (!ValidarListaUnits())
                return;
            if (!ValidarExeAppTests())
                return;
            if (!ValidarMapAppTests())
                return;
            if (!ValidarCodeCovPaths())
                return;
            if (!ValidarCodeCovUnits())
                return;

            string comando = caminhoWorkspace + @"\bin\CodeCoverage\CodeCoverage.exe";
            
            string parametros = "-v -e \"" + caminhoExeAppTests +
                "\" -m \"" + caminhoMapAppTests +
                "\" -uf \"" + caminhoCodeCovUnits +
                "\" -spf \"" + caminhoCodeCovPaths +
                "\" -od \"" + caminhoWorkspace +
                @"\bin\CodeCoverage\Result_CodeCov\"" -lt -html -xml -emma -lapi";

            System.Diagnostics.Process.Start(comando, parametros);           
        }

        private bool DefinirWorkspace()
        {
            caminhoWorkspace = textCaminho.Text.Trim();

            if (caminhoWorkspace.Length == 0)
            {
                MessageBox.Show("Por favor, preencha o caminho da workspace");
            }

            return (caminhoWorkspace.Length > 0);
        }

        private bool ValidarListaUnits()
        {
            if (checkListaPadrao.IsChecked ?? false)
                return true;

            if (textLista.Text.Trim().Length > 0)
                return true;

            MessageBox.Show("Por favor, preencha a lista de units");
            return false;
        }

        private bool ValidarMapAppTests()
        {
            caminhoMapAppTests = caminhoWorkspace + @"\bin\" + arquivoJson.NomeArquivoMapTests;

            if (File.Exists(caminhoMapAppTests))
                return true;

            MessageBox.Show("Arquivo inexistente: " + caminhoMapAppTests);
            return false;
        }

        private bool ValidarExeAppTests()
        {
            caminhoExeAppTests = caminhoWorkspace + @"\bin\" + arquivoJson.NomeArquivoExeTests;

            if (File.Exists(caminhoExeAppTests))
                return true;

            MessageBox.Show("Arquivo inexistente: " + caminhoExeAppTests);
            return false;
        }

        private bool ValidarCodeCovUnits()
        {
            if (checkListaPadrao.IsChecked ?? false)
            {
                caminhoCodeCovUnits = caminhoWorkspace + @"\bin\CodeCoverage\CodeCov_units.lst";

                if (File.Exists(caminhoCodeCovUnits))
                    return true;

                MessageBox.Show("Arquivo inexistente: " + caminhoCodeCovUnits);
                return false;
            }
            else
            {
                caminhoCodeCovUnits = caminhoWorkspace + @"\bin\CodeCoverage\CodeCov_units_manual.lst";

                File.WriteAllText(caminhoCodeCovUnits, textLista.Text);
                return true;
            }
        }

        private bool ValidarCodeCovPaths()
        {            
            caminhoCodeCovPaths = caminhoWorkspace + @"\bin\CodeCoverage\CodeCov_Paths.lst";

            if (File.Exists(caminhoCodeCovPaths))
                return true;

            MessageBox.Show("Arquivo inexistente: " + caminhoCodeCovPaths);
            return false;
        }

        private void InicializarSistema(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("config.json"))
            {
                CriarArquivoJson();
            }

            string jsonString = File.ReadAllText("config.json");

            arquivoJson = ArquivoJson.PegarInstancia();
            arquivoJson = JsonSerializer.Deserialize<ArquivoJson>(jsonString);

            textCaminho.Text = arquivoJson.CaminhoWorkspace;
        }

        private void CriarArquivoJson()
        {
            arquivoJson = ArquivoJson.PegarInstancia();
            arquivoJson.GravarUltimoWorkspace = true;
            arquivoJson.NomeArquivoExeTests = "pg5AppTests.exe";
            arquivoJson.NomeArquivoMapTests = "pg5AppTests.map";

            GravarJson();
        }

        private void GravarJson()
        {
            var opcoesJson = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(arquivoJson, opcoesJson);
            File.WriteAllText("config.json", jsonString);
        }

        private void EncerrarSistema(object sender, EventArgs e)
        {
            arquivoJson.CaminhoWorkspace = textCaminho.Text.Trim();
            GravarJson();
        }
    }
}
