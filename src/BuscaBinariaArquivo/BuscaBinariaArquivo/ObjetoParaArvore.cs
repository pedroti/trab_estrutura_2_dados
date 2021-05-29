namespace BuscaBinariaArquivo
{
    public class ObjetoParaArvore : Tabela
    {
        public ObjetoParaArvore(string continente, long enderecoInicial)
        {
            this.continente = continente;
            this.enderecoInicial = enderecoInicial;
        }

        public string continente;
        public long enderecoInicial;
        public long enderecoFinal;

        public override string IdParaBusca()
        {
            return continente;
        }
    }
}
