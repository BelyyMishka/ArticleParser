using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticleParser
{
    static class Writer
    {
        private const string FILENAME = "Scopus.txt";
        public static void WriteToFile(string path, string text)
        {
            StreamWriter streamWriter = new StreamWriter(string.Format(@"{0}\{1}", path, FILENAME), true);
            streamWriter.WriteLine(text);
            streamWriter.Close();
        }
    }
}
