using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BuscaBinariaArquivo
{
    public class Program
    {
        static void Main(string[] args)
        {
            var encoding = Encoding.ASCII;
            string dataFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DataFile.csv");
            string indexIdFilePath = Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIDFile.txt"));
            string indexIsoCodeFilePath = Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIsoCodeFile.txt"));

            GenerateIndexIDFile(indexIdFilePath, dataFilePath, encoding);
            BuscarPorId(dataFilePath, indexIdFilePath, encoding);
            GenerateIndexIsoCodeFile(indexIsoCodeFilePath, dataFilePath, encoding);
            BuscarResultadoHipotese(dataFilePath, indexIsoCodeFilePath, encoding);

            GenerateHashStructure(dataFilePath, encoding);
            GenerateBTree(dataFilePath, encoding);
            //string registroPelaData = BuscarPorData(hash);
            //string registroContinente = BuscarPorContinente(arvore, 18488);

            //MostrarTodos(dataFilePath);
        }

        #region 2.1
        public static void GenerateIndexIDFile(string indexIdFilePath, string dataFilePath, Encoding encoding)
        {
            string line;
            long bytesCounter = 0;
            string id;
            int dataFileSize = 0;

            File.Delete(indexIdFilePath);

            if (!File.Exists(indexIdFilePath))
            {
                using (StreamReader sr = new StreamReader(dataFilePath, encoding))
                using (StreamWriter sw = new StreamWriter(indexIdFilePath, true, encoding))
                {
                    var test = sr.GetType();
                    dataFileSize = sr.BaseStream.Length.ToString().Length;
                    while ((line = sr.ReadLine()) != null)
                    {
                        id = line.Substring(0, line.IndexOf(';'));

                        sw.WriteLine(id.PadLeft(5, '0') + ';' + (bytesCounter.ToString().PadLeft(dataFileSize, '0')));

                        bytesCounter += (encoding.GetByteCount(line) + 2);
                    }
                }
            }
        }

        public static void BuscarPorId(string dataFilePath, string indexIdFilePath, Encoding encoding)
        {
            var id = 60816;
            var endereco = BuscaRegistro(id, indexIdFilePath, encoding); 
            byte[] caracter = new byte[1];
            char[] encodedCaracter = new char[1];
            var registro = new StringBuilder();
            using (FileStream fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
            {
                for (int i = Convert.ToInt32(endereco); true; i++)
                {
                    fs.Seek(i, SeekOrigin.Begin);
                    fs.Read(caracter, 0, 1);
                    encoding.GetChars(caracter, 0, 1, encodedCaracter, 0);

                    if (encodedCaracter[0] == '\n')
                    {
                        break;
                    }
                    else
                    {
                        registro.Append(encodedCaracter);
                    }
                }
                Console.WriteLine("Registro Procurado: ");
                Console.WriteLine(registro.ToString());
            }
        }

        public static string BuscaRegistro(int numeroRegistro, string indexIdFilePath, Encoding encoding)
        {
            Console.WriteLine($"Buscando registro numero {numeroRegistro}:");
            var endereco = string.Empty;
            using (FileStream fs = new FileStream(indexIdFilePath, FileMode.Open, FileAccess.Read))
            {
                endereco = BuscaRegistroRecursivo(numeroRegistro, 0, fs.Length, fs, encoding, null);
            }
            return endereco;
        }

        #endregion

        #region 2.2

        public static void GenerateIndexIsoCodeFile(string indexIsoCodeFilePath, string dataFilePath, Encoding encoding)
        {
            string line;
            long bytesCounter = 0;
            long bytesCounterBegin = 0;
            string id;
            string currentIsoCode = string.Empty;
            int dataFileSize = 0;

            File.Delete(indexIsoCodeFilePath);

            if (!File.Exists(indexIsoCodeFilePath))
            {
                //2.2
                using (StreamReader sr = new StreamReader(dataFilePath, encoding))
                using (StreamWriter sw = new StreamWriter(indexIsoCodeFilePath, true, encoding))
                {
                    bytesCounter = 0;
                    dataFileSize = sr.BaseStream.Length.ToString().Length;
                    var newIsoCode = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        id = line.Substring(0, line.IndexOf(';'));

                        newIsoCode = line.Substring(id.Length + 1, (line.IndexOf(';', (id.Length + 1)) - id.Length - 1));

                        if (currentIsoCode.Equals(string.Empty))
                        {
                            currentIsoCode = newIsoCode;
                        }
                        else if (!currentIsoCode.Equals(newIsoCode))
                        {
                            //bytesCounter -= 2;
                            sw.WriteLine(currentIsoCode + ';' +
                                        ((bytesCounterBegin).ToString().PadLeft(dataFileSize, '0') + ";" +
                                        ((bytesCounter - 2).ToString().PadLeft(dataFileSize, '0'))));
                            currentIsoCode = newIsoCode;
                            bytesCounterBegin = bytesCounter;
                        }

                        bytesCounter += (encoding.GetByteCount(line) + 2);
                    }
                }
            }
        }

        public static void BuscarResultadoHipotese(string dataFilePath, string indexIsoCodeFilePath, Encoding encoding)
        {//utilizando arquivo com iso_codes e intervalo - 2.2

            //ler o arquivo de indices por isocode

            var list = new List<ItemFileHipotese>();

            using (StreamReader sr = new StreamReader(indexIsoCodeFilePath))
            {
                string line;
                string[] data;
                byte[] caracter = new byte[1];
                char[] encodedCaracter = new char[1];
                ItemFileHipotese item;
                StringBuilder dataLine;
                while ((line = sr.ReadLine()) != null)
                {
                    data = line.Split(';');
                    using (FileStream fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
                    {
                        item = new ItemFileHipotese();
                        dataLine = new StringBuilder();
                        long i;
                        for (i = Convert.ToInt64(data[2]) - 1; i >= Convert.ToInt64(data[1]); i--)
                        {
                            fs.Seek(i, SeekOrigin.Begin);
                            fs.Read(caracter, 0, 1);
                            encoding.GetChars(caracter, 0, 1, encodedCaracter, 0);

                            if (encodedCaracter[0] == '\n')
                            {
                                break;
                            }
                        }

                        int columns = 1;
                        for (long j = i + 1; j <= Convert.ToInt64(data[2]); j++)
                        {
                            fs.Seek(j, SeekOrigin.Begin);
                            fs.Read(caracter, 0, 1);
                            encoding.GetChars(caracter, 0, 1, encodedCaracter, 0);

                            if (encodedCaracter[0] == ';' || encodedCaracter[0] == '\r')
                            {

                                if (columns == 2 && dataLine.Length > 0)
                                {
                                    item.IsoCode = dataLine.ToString();
                                }
                                else if (columns == 14 && dataLine.Length > 0)
                                {
                                    item.PopulationDensity = Convert.ToDecimal(dataLine.ToString());
                                }
                                else if (columns == 15 && dataLine.Length > 0)
                                {
                                    item.LifeExpectancy = Convert.ToDecimal(dataLine.ToString());
                                }

                                columns++;
                                dataLine = new StringBuilder();
                            }
                            else
                            {
                                dataLine.Append(encodedCaracter);
                            }

                            caracter[0] = new byte();
                            encodedCaracter[0] = new char();
                        }
                        list.Add(item);
                    }
                }
            }

            var dadosValidos = list.Where(x => x.LifeExpectancy > 0 && x.PopulationDensity > 0).ToList();
            Console.WriteLine("ISO CODE - DENSIDADE POPULACIONAL - EXPECTATIVA DE VIDA");

            Console.WriteLine("ordenado pela densidade populacional decrescente");
            foreach (var item in dadosValidos.OrderByDescending(x => x.PopulationDensity).ToList().Take(10))
            {
                Console.WriteLine($"{item.IsoCode} - {item.PopulationDensity} - {item.LifeExpectancy}");
            }

            Console.WriteLine("ordenado pela espectativa de vida decrescente");
            foreach (var item in dadosValidos.OrderByDescending(x => x.LifeExpectancy).ToList().Take(10))
            {
                Console.WriteLine($"{item.IsoCode} - {item.PopulationDensity} - {item.LifeExpectancy}");
            }

            Console.WriteLine("ordenado pela densidade populacional crescente");
            foreach (var item in dadosValidos.OrderBy(x => x.PopulationDensity).ToList().Take(10))
            {
                Console.WriteLine($"{item.IsoCode} - {item.PopulationDensity} - {item.LifeExpectancy}");
            }

            Console.WriteLine("ordenado pela espectativa de vida crescente");
            foreach (var item in dadosValidos.OrderBy(x => x.LifeExpectancy).ToList().Take(10))
            {
                Console.WriteLine($"{item.IsoCode} - {item.PopulationDensity} - {item.LifeExpectancy}");
            }

            Console.WriteLine("ordenado pela densidade populacional decrescente e expectativa de vida decrescente");
            foreach (var item in dadosValidos.OrderByDescending(x => x.PopulationDensity).ThenByDescending(y => y.LifeExpectancy).ToList().Take(10))
            {
                Console.WriteLine($"{item.IsoCode} - {item.PopulationDensity} - {item.LifeExpectancy}");
            }

            //Console.WriteLine("ordenado pela densidade populacional decrescente e expectativa de vida crescente");
            //foreach (var item in dadosValidos.OrderByDescending(x => x.PopulationDensity).ThenBy(y => y.LifeExpectancy).ToList().Take(10))
            //{
            //    Console.WriteLine($"{item.IsoCode} - {item.PopulationDensity} - {item.LifeExpectancy}");
            //}
        }

        #endregion

        #region 2.3

        public static void GenerateHashStructure(string dataFilePath, Encoding encoding)
        {
            //item 2.3 (indice de memória com hash pela data)
            Dictionary<string, List<long>> hash = new Dictionary<string, List<long>>();
            long bytesCounter = 0;
            string line;
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    string date = line.Split(';')[2];
                    if (!hash.ContainsKey(date))
                    {
                        hash.Add(date, new List<long>());
                    }
                    hash.GetValueOrDefault(date).Add(bytesCounter);
                    bytesCounter += encoding.GetByteCount(line) + 2;
                }
            }
        }

        public static string BuscarPorData(Dictionary<string, List<long>> hash, Encoding encoding)
        {
            string data = "2020-12-12";
            byte[] caracter = new byte[1];
            char[] encodedCaracter = new char[1];
            string registro = "";
            string dataFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DataFile.csv");
            List<long> lista = hash.GetValueOrDefault(data);
            using (FileStream fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
            {
                for (long i = lista[0]; ; i++)
                {
                    fs.Seek(i, SeekOrigin.Begin);
                    fs.Read(caracter, 0, 1);
                    encoding.GetChars(caracter, 0, 1, encodedCaracter, 0);
                    if (encodedCaracter[0] == '\n')
                    {
                        break;
                    }
                    else
                    {
                        registro = registro + encodedCaracter[0];
                    }
                }
            }
            return registro;
        }

        #endregion

        #region 2.4

        public static void GenerateBTree(string dataFilePath, Encoding encoding)
        {
            //item 2.3 (indice de memória com árvore binária pelo continente)
            ArvoreBinaria arvore = new ArvoreBinaria();
            long bytesCounter = 0;
            string line;
            string continenteAtual = null;
            long bytesFinal = 0;
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    string continente = line.Split(';')[3];

                    if (continente != continenteAtual)
                    {
                        if (arvore.Buscar(continenteAtual) != null)
                        {
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
                    bytesCounter += encoding.GetByteCount(line) + 2;
                }
            }
        }

        public static string BuscarPorContinente(ArvoreBinaria arvore, int numeroRegistro, Encoding encoding)
        {
            string continente = "Europe";
            string registro = "";
            string dataFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DataFile.csv");
            NoArvore nodo = arvore.Buscar(continente);
            ObjetoParaArvore objetoParaArvore = (ObjetoParaArvore)nodo.Dados();
            using (FileStream fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
            {
                BuscaRegistroRecursivo(numeroRegistro, objetoParaArvore.enderecoInicial, objetoParaArvore.enderecoFinal, fs, encoding, null);
            }
            return registro;
        }

        #endregion

        #region Busca Recursiva

        public static string BuscaRegistroRecursivo(int numeroRegistro, long limiteSup, long limiteInf, FileStream fs, Encoding encoding, string ultimoCodigoEncontrado)
        {
            var endereco = string.Empty;
            long meio = limiteSup + ((limiteInf - limiteSup) / 2);
            byte[] caracter = new byte[1];
            char[] encodedCaracter = new char[1];
            string registro = string.Empty;
            string[] registroSplit;

            //esse laço volta ao início da linha após achar a linha do meio
            for (long i = meio; i >= 0 && i <= limiteInf; i--)
            {
                fs.Seek(i, SeekOrigin.Begin);
                fs.Read(caracter, 0, 1);
                encoding.GetChars(caracter, 0, 1, encodedCaracter, 0);
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

            //esse laço lê a partir da linha do meio
            for (long i = meio; i >= 0 && i <= limiteInf; i++)
            {
                fs.Seek(i, SeekOrigin.Begin);
                fs.Read(caracter, 0, 1);
                encoding.GetChars(caracter, 0, 1, encodedCaracter, 0);
                if (encodedCaracter[0] == '\n')
                {
                    break;
                }
                else
                {
                    registro = registro + encodedCaracter[0];
                }
            }
            registroSplit = registro.Split(';');

            if (ultimoCodigoEncontrado == registroSplit[0])
            {
                Console.WriteLine("Código não encontrado!");
                return string.Empty;
            }
            else if (Convert.ToInt32(registroSplit[0]) == numeroRegistro)
            {
                //ao invés de imprimir, retornar o endereço
                Console.WriteLine(registro);
                return registroSplit[1];
            }

            else if (Convert.ToInt64(registroSplit[0]) == numeroRegistro)
            {
                Environment.Exit(0);
            }
            else if (meio == 0)
            {
                Console.WriteLine("Código não encontrado!");
                return string.Empty;
            }
            else if (numeroRegistro > Convert.ToInt64(registroSplit[0]))
            {
                //parte de baixo
                endereco = BuscaRegistroRecursivo(numeroRegistro, meio, limiteInf, fs, encoding, registroSplit[0]);
            }
            else
            {
                //parte de cima
                endereco = BuscaRegistroRecursivo(numeroRegistro, limiteSup, meio, fs, encoding, registroSplit[0]);
            }
            return endereco;
        }

        #endregion

        #region Mostrar Todos

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

        #endregion

    }
}