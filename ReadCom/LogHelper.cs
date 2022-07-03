namespace ReadCom
{
    public class LogHelper
    {
        private static SimpleLogger _logger = new SimpleLogger(@"/var/www/ReadComPHP/var/log/csharp.log");

        public static void Debug(string text)
        {
            _logger.Debug(text);
        }
        public static void Error(string text)
        {
            _logger.Error(text);
        }
        
        public static void Fatal(string text)
        {
            _logger.Fatal( text);
        }

        public static void Info(string text)
        {
            _logger.Info(text);
        }

        public static void Trace(string text)
        {
            _logger.Trace(text);
        }
        
        public static void Warning(string text)
        {
            _logger.Warning(text);
        }
    }
}