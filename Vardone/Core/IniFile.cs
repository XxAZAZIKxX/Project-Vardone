using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


//██████╗░░█████╗░███╗░░░███╗░█████╗░███╗░░██╗  ░█████╗░██████╗░██╗░░░██╗███╗░░██╗███████╗████████╗░██████╗
//██╔══██╗██╔══██╗████╗░████║██╔══██╗████╗░██║  ██╔══██╗██╔══██╗╚██╗░██╔╝████╗░██║██╔════╝╚══██╔══╝██╔════╝
//██████╔╝██║░░██║██╔████╔██║███████║██╔██╗██║  ██║░░██║██║░░██║░╚████╔╝░██╔██╗██║█████╗░░░░░██║░░░╚█████╗░
//██╔══██╗██║░░██║██║╚██╔╝██║██╔══██║██║╚████║  ██║░░██║██║░░██║░░╚██╔╝░░██║╚████║██╔══╝░░░░░██║░░░░╚═══██╗
//██║░░██║╚█████╔╝██║░╚═╝░██║██║░░██║██║░╚███║  ╚█████╔╝██████╔╝░░░██║░░░██║░╚███║███████╗░░░██║░░░██████╔╝
//╚═╝░░╚═╝░╚════╝░╚═╝░░░░░╚═╝╚═╝░░╚═╝╚═╝░░╚══╝  ░╚════╝░╚═════╝░░░░╚═╝░░░╚═╝░░╚══╝╚══════╝░░░╚═╝░░░╚═════╝░

namespace Vardone.Core
{
    /// <summary>
    /// Отвечает за работу с файлом конфигурации
    /// </summary>
    internal class IniFile
    {
        private readonly string _path;
        private readonly string _exe = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        /// <summary>
        /// Конструктор конфигурационного файла
        /// </summary>
        /// <param name="IniPath">Путь к файлу</param>
        public IniFile(string IniPath = null) => _path = new FileInfo(IniPath ?? _exe + ".ini").FullName.ToString();

        /// <summary>
        /// Чтение записи с конфигурации
        /// </summary>
        /// <param name="Key">Ключ записи</param>
        /// <param name="Section">Секция записи</param>
        /// <returns>Значение записи</returns>
        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? _exe, Key, "", RetVal, 255, _path);
            return RetVal.ToString();
        }

        /// <summary>
        /// Запись в конфигурацию
        /// </summary>
        /// <param name="Key">Ключ записи</param>
        /// <param name="Value">Значение записи</param>
        /// <param name="Section">Секция записи</param>
        public void Write(string Key, string Value, string Section = null) => WritePrivateProfileString(Section ?? _exe, Key, Value, _path);

        /// <summary>
        /// Удаление ключа с конфигурации
        /// </summary>
        /// <param name="Key">Ключ записи</param>
        /// <param name="Section">Секция записи</param>
        public void DeleteKey(string Key, string Section = null) => Write(Key, null, Section ?? _exe);

        /// <summary>
        /// Удаление целой секции с конфигурации
        /// </summary>
        /// <param name="Section">Секция</param>
        public void DeleteSection(string Section = null) => Write(null, null, Section ?? _exe);

        /// <summary>
        /// Проверка на существование ключа в конфигурации
        /// </summary>
        /// <param name="Key">Ключ записи</param>
        /// <param name="Section">Секция записи</param>
        /// <returns>Булевый результат</returns>
        public bool KeyExists(string Key, string Section = null) => Read(Key, Section).Length > 0;
    }
}