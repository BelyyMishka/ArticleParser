using System.IO;

namespace ArticleParser
{
    static class Writer
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        private const string FILENAME = "Scopus.txt";

        /// <summary>
        /// Метод для записи в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="text">Текст</param>
        public static void WriteToFile(string path, string text, int currentPosition)
        {
            StreamWriter streamWriter = new StreamWriter(string.Format(@"{0}\{1}", path, FILENAME), true);
            streamWriter.WriteLine(string.Format(@"{0} {1}", currentPosition, text));
            streamWriter.Close();
        }
    }
}
