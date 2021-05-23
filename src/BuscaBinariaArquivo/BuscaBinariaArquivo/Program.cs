using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BuscaBinariaArquivo
{
    public class Program
    {
        static void Main(string[] args)
        {
            string line;
            long bytesCounter = 0;
            long bytesCounterBegin = 0;
            string id;
            string currentIsoCode = string.Empty;
            string newIsoCode = string.Empty;
            int dataFileSize = 0;
            string dataFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DataFile.csv");
            File.Delete(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIDFile.txt")));
            File.Delete(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIsoCodeFile.txt")));

            if (!File.Exists(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIDFile.txt"))) ||
                !File.Exists(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIsoCodeFile.txt"))))
            {
                File.Delete(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIDFile.txt")));
                File.Delete(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIsoCodeFile.txt")));

                //2.1 e 2.2
                using (StreamReader sr = new StreamReader(dataFilePath))
                using (StreamWriter sw = new StreamWriter(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIDFile.txt")))
                using (StreamWriter sw2 = new StreamWriter(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIsoCodeFile.txt")))
                {
                    dataFileSize = sr.BaseStream.Length.ToString().Length;
                    while ((line = sr.ReadLine()) != null)
                    {
                        id = line.Substring(0, line.IndexOf(';'));

                        sw.WriteLine(id.PadLeft(5, '0') + ';' + (bytesCounter.ToString().PadLeft(sr.BaseStream.Length.ToString().Length, '0')));

                        newIsoCode = line.Substring(id.Length + 1, (line.IndexOf(';', (id.Length + 1)) - id.Length - 1));

                        if (currentIsoCode.Equals(string.Empty))
                        {
                            currentIsoCode = newIsoCode;
                        }
                        else if (!currentIsoCode.Equals(newIsoCode))
                        {
                            sw2.WriteLine(currentIsoCode + ';' +
                                         (bytesCounterBegin.ToString().PadLeft(dataFileSize, '0') + ";" +
                                         ((bytesCounter - 1).ToString().PadLeft(dataFileSize, '0'))));
                            currentIsoCode = newIsoCode;
                            bytesCounterBegin = bytesCounter;
                        }

                        bytesCounter += ASCIIEncoding.Unicode.GetByteCount(line);
                    }
                }
            }

            //item 2.3 (indice de memória com hash pela data)
            Dictionary<String, List<long>> hash = new Dictionary<String, List<long>>();
            bytesCounter = 0;
            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DataFile.csv")))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    String date = line.Split(';')[2];
                    if (!hash.ContainsKey(date))
                    {
                        hash.Add(date, new List<long>());
                    }
                    hash.GetValueOrDefault(date).Add(bytesCounter);
                    bytesCounter += ASCIIEncoding.Unicode.GetByteCount(line);
                }
            }

            //item 2.3 (indice de memória com árvore binária pelo continente)
            ArvoreBinaria arvore = new ArvoreBinaria();
            bytesCounter = 0;
            String continenteAtual = null;
            long bytesFinal = 0;
            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DataFile.csv")))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    String continente = line.Split(';')[3];

                    if (continente != continenteAtual)
                    {
                        if (arvore.Buscar(continenteAtual) != null) {
                            NoArvore nodo = arvore.Buscar(continenteAtual);
                            ObjetoParaArvore objetoParaArvore = (ObjetoParaArvore)nodo.Dados();
                            objetoParaArvore.enderecoFinal = bytesFinal;
                        }
                    }

                    if (arvore.Buscar(continente) == null)
                    {
                        arvore.Inserir(new ObjetoParaArvore(continente, bytesCounter));
                        continenteAtual = continente;
                    }
                    bytesFinal = bytesCounter;
                    bytesCounter += ASCIIEncoding.Unicode.GetByteCount(line);
                }
            }
            //MostrarTodos(dataFilePath);
        }

        public static void MostrarTodos(string dataFilePath)
        {
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }

        public static void BuscarResultadoHipotese()
        {//utilizando arquivo com iso_codes e intervalo - 2.2

            //ler o arquivo de indices por isocode

            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIsoCodeFile.txt")))
            {

            }

            //para cada isocode procurar pela data mais antiga (sempre a última daquele isocode)

            //pegando a informação do final e lendo byte a byte e armazenando até chegar no \n, dai parar

            //pegar o dado de people_vaccinated e life_expectancy

            //armazenar isso em uma lista

            //imprimir ordenado por life_expectancy e people_vaccinated_per_hundred

            //imprimir uma só ordenado por life_expectancy

            //imprimir outra só ordenado por people_vaccinated_per_hundred

            //ou acumular em lista e exibir todos os registros para aquele país / iso_code
        }

        public static void BuscarPorData()
        {//utilizando estrutura hash - 2.3
            //pesquisa pela data (que tem um hash)
            //percorre a lista para aquele hash sobre a data
            //busca no arquivo de data a linha inteira começando do indice salvo na lista(até achar o \n)
            //ve se aquela é a linha que contém o que estamos querendo
            //poderia filtrar por um país, por exemplo
            //aquela data e dentre as datas, aquele país
            //ou acumular em lista e exibir todos os registros para aquela data
        }

        public static void BuscarPorId()
        {//utilizando arquivo de índices por id - 2.1
            //consultar por um ID
            //consultar no arquivo de indice por ID qual o índice daquele ID
            //ir no arquivo e buscar toda aquela linha (do inicio do índice até achar o \n)
        }

        public static void BuscarPorContinente()
        {//utilizando a árvore binária por continente - 2.4
            //árvore binária contendo o intervalo que possui os registros para aquele continenete
            //dito um continente, podemos saber o incio e o fim daquele continente
            //e realizar busca binária por outras informações dentro dele
        }


        public static void BuscaRegistro(int numeroRegistro, string path)
        {
            Console.WriteLine($"Buscando registro numero {numeroRegistro}:");
            Encoding ascii = Encoding.ASCII;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BuscaRegistroRecursivo(numeroRegistro, 0, fs.Length, fs, ascii, null);
            }
        }

        public static void BuscaRegistroRecursivo(int numeroRegistro, long limiteSup, long limiteInf, FileStream fs, Encoding ascii, string ultimoCodigoEncontrado)
        {
            long meio = limiteSup + ((limiteInf - limiteSup) / 2);
            byte[] caracter = new byte[1];
            char[] encodedCaracter = new char[1];
            string registro = string.Empty;
            string[] registroSplit;

            for (long i = meio; i >= 0 && i <= limiteInf; i--)
            {
                fs.Seek(i, SeekOrigin.Begin);
                fs.Read(caracter, 0, 1);
                ascii.GetChars(caracter, 0, 1, encodedCaracter, 0);
                if (encodedCaracter[0] == '\n')
                {
                    meio = i + 1;
                    break;
                }
                else if (i == 0)
                {
                    meio = i;
                    break;
                }
                else
                {
                    caracter[0] = new byte();
                    encodedCaracter[0] = new char();
                }
            }

            for (long i = meio; i >= 0 && i <= limiteInf; i++)
            {
                fs.Seek(i, SeekOrigin.Begin);
                fs.Read(caracter, 0, 1);
                ascii.GetChars(caracter, 0, 1, encodedCaracter, 0);
                if (encodedCaracter[0] == '\n')
                {
                    break;
                }
                else
                {
                    registro = registro + encodedCaracter[0];
                }
            }
            registroSplit = registro.Split(' ');

            if (ultimoCodigoEncontrado == registroSplit[0])
            {
                Console.WriteLine("Código não encontrado!");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine(registro);
            }

            if (Convert.ToInt64(registroSplit[0]) == numeroRegistro)
            {
                Environment.Exit(0);
            }
            else if (meio == 0)
            {
                Console.WriteLine("Código não encontrado!");
                Environment.Exit(0);
            }
            else if (numeroRegistro > Convert.ToInt64(registroSplit[0]))
            {
                //parte de baixo
                BuscaRegistroRecursivo(numeroRegistro, meio, limiteInf, fs, ascii, registroSplit[0]);
            }
            else
            {
                //parte de cima
                BuscaRegistroRecursivo(numeroRegistro, limiteSup, meio, fs, ascii, registroSplit[0]);
            }
        }
    }
}