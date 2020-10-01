namespace CodeCoverage
{
    public class ArquivoJson
    {
        private static ArquivoJson instancia;

        public string CaminhoWorkspace { get; set; }

        public bool GravarUltimoWorkspace { get; set; }

        public string NomeArquivoExeTests { get; set; }

        public string NomeArquivoMapTests { get; set; }

        public static ArquivoJson PegarInstancia()
        {
            if (instancia == null)
            {
                instancia = new ArquivoJson();
            }
            return instancia;
        }
    }
}
