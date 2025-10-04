#region Using Statements

using System;

#endregion

namespace Metro_Surffaajat
{
    /// @author Tuisku Tynkkynen
    /// @version 01.10.2025
    /// <summary>
    /// The main class.
    /// Only used for initialization of game class
    /// and as application entry point. 
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

