using System;

namespace Parallax_Demo
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Footprint_Game game = new Footprint_Game())
            {
                game.Run();
            }
        }
    }
#endif
}

