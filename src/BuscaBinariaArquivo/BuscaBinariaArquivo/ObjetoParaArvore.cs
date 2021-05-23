using System;
using System.Collections.Generic;
using System.Text;

namespace BuscaBinariaArquivo
{
    class ObjetoParaArvore : Tabela
    {
        public ObjetoParaArvore(string continente)
        {
            this.continente = continente;
            this.enderecos = new List<long>();
        }

        public string continente;
        public List<long> enderecos;

        public override string IdParaBusca()
        {
            return continente;
        }
    }
}
