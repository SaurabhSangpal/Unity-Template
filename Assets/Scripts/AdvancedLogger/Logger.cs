using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedLogger
{
    public class Logger
    {
        private const int BufferSize = 512;
        public static Logger instance;
        private readonly string filePath;
        private readonly LoggerStream loggerStream;
        private FileStream fileStream;

        public Logger() {
            instance     ??= this;
            loggerStream =   new LoggerStream(BufferSize);
        }

        /// <summary>
        ///     Create a logger with a file to write to.
        ///     If a file exists, it will be replaced with new data.
        /// </summary>
        /// <param name="filePath">The path where the file will be created.</param>
        public Logger(string filePath) {
            this.filePath = filePath;
            if (File.Exists(filePath)) File.Delete(filePath);

            instance                    ??= this;
            loggerStream                =   new LoggerStream(BufferSize);
            loggerStream.OnLoggerFilled +=  WriteToDiskAsync;
        }

        public static void Log(Severity severity, string log) {
            if (severity == Severity.Trace) {
#if !UNITY_EDITOR
                return;
#endif
            }

            instance.loggerStream
                    .Add($"{GetPrefixBySeverity(severity)} {DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)} {log} {new StackTrace(new StackFrame(1))}");
        }

        public string[] GetStream() {
            return loggerStream.Stream;
        }

        public void WriteLogToDisk() {
            WriteToDisk(loggerStream.GetNonEmptyStream());
            loggerStream.ClearStream();
        }

        private async void WriteToDiskAsync(string[] dataToBeWritten) {
            await Task.Run(() => { WriteToDisk(dataToBeWritten); });
        }

        private void WriteToDisk(string[] dataToBeWritten) {
            fileStream = File.Open(filePath, FileMode.Append, FileAccess.Write);

            // Combine the strings into one large string
            var _text = "";
            for (var i = 0; i < dataToBeWritten.Length; i++) {
                _text += dataToBeWritten[i];
                _text += "\n";
            }

            var _bytes = Encoding.Default.GetBytes(_text);
            fileStream.Write(_bytes, 0, _bytes.Length);

            fileStream.Close();
        }

        private static string GetPrefixBySeverity(in Severity severity) {
            return severity switch
            {
                Severity.Trace => "Trace",
                Severity.Info => "Info",
                Severity.Warning => "Warning",
                Severity.Error => "Error",
                Severity.Fatal => "Fatal",
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };
        }
    }
}