using System;

namespace AdvancedLogger
{
    internal class LoggerStream
    {
        private int index;

        public LoggerStream() {
            Stream = new string[512];
        }

        public LoggerStream(int initialLength) {
            Stream = new string[initialLength];
        }

        public string[] Stream { get; private set; }

        public event Action<string[]> OnLoggerFilled;

        ~LoggerStream() {
            Stream = null;
        }

        public void Add(string log) {
            if (index < Stream.Length) {
                Stream[index] = log;
                index++;
            }
            else {
                var _array = new string[Stream.Length];
                Array.Copy(Stream, _array, Stream.Length);
                OnLoggerFilled?.Invoke(_array);
                Array.Clear(Stream, 0, Stream.Length);

                // Put the element in the stream
                Stream[0] = log;
                index     = 1;
            }
        }

        /// <summary>
        ///     Only gets non-empty elements in the Stream.
        /// </summary>
        public string[] GetNonEmptyStream() {
            var _numberOfElements = -1;
            for (var i = 0; i < Stream.Length; i++) {
                if (!string.IsNullOrEmpty(Stream[i])) continue;

                _numberOfElements = i;
                break;
            }

            if (_numberOfElements == -1) _numberOfElements = Stream.Length;

            var _array = new string[_numberOfElements];
            Array.Copy(Stream, _array, _numberOfElements);

            return _array;
        }

        public void ClearStream() {
            Array.Clear(Stream, 0, Stream.Length);
        }
    }
}