using System;

namespace x_hotel_server.net.Misc
{
    public static class EasyUtils
    {
        public static void ColorDebug(ConsoleColor color, string mensagem)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(mensagem);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}