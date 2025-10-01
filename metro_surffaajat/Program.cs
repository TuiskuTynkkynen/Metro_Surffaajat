#region Using Statements

using System;

#endregion

namespace Metro_Surffaajat
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using var game = new MetroSurffaajat();
            game.Run();
        }
    }
}

