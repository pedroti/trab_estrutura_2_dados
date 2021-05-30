using System.Collections.Generic;

namespace BuscaBinariaArquivo
{
    public class ObjetoParaArvore : Tabela
    {
        public ObjetoParaArvore(string continente)
        {
            this.continente = continente;
            this.enderecos = new List<Endereco>();
        }

        public string continente;
        public List<Endereco> enderecos;

        public override string IdParaBusca()
        {
            return continente;
        }
    }
}
