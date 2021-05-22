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
            Encoding ascii = Encoding.ASCII;

            string line;
            long bytesCounter = 0;
            string id;
            File.Delete(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIDFile.txt")));

            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DataFile.csv")))
            using (StreamWriter sw = new StreamWriter(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "IndexIDFile.txt")))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    id = line.Substring(0, line.IndexOf(';'));

                    sw.WriteLine(id.PadLeft(5, '0') + ';' + (bytesCounter.ToString().PadLeft(sr.BaseStream.Length.ToString().Length, '0')));

                    bytesCounter += ASCIIEncoding.Unicode.GetByteCount(line);
                }
            }
            
            //item 
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
            //File.AppendAllLines(Path.Combine(docPath, "WriteFile.txt"), registros);

            //using (FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DataFile.txt"), FileMode.Open, FileAccess.Read),
            //             fsIndex = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "IndexIDFile.txt"), FileMode.Open, FileAccess.Write))
            //{
            //    string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //    File.Delete(Path.Combine(docPath, "IndexIDFile.txt"));

            //    byte[] caracter = new byte[1];
            //    char[] encodedCaracter = new char[1];
            //    long startLineIndex = 0;

            //    string id = string.Empty;

            //    for (long bytesCounter = 0; bytesCounter <= fs.Length; bytesCounter++)
            //    {
            //        fs.Seek(bytesCounter, SeekOrigin.Begin);
            //        fs.Read(caracter, 0, 1);
            //        ascii.GetChars(caracter, 0, 1, encodedCaracter, 0);


            //        if (bytesCounter == 0)
            //        {
            //            //guarda in
            //        }
            //        else if (encodedCaracter[0] == ';' && string.IsNullOrEmpty(id))
            //        {

            //        }
            //        else if (encodedCaracter[0] == '\n')
            //        {
            //            startLineIndex = bytesCounter + 1;
            //        }
            //        else
            //        {
            //            caracter[0] = new byte();
            //            encodedCaracter[0] = new char();
            //        }
            //        //File.AppendAllLines(Path.Combine(docPath, "WriteFile.txt"), registros);
            //    }

            //    BuscaRegistroRecursivo(numeroRegistro, 0, fs.Length, fs, ascii, null);
            //}


            //

        }
    }
}