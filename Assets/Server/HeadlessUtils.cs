using System;
using System.Linq;

namespace Server {
    public static class HeadlessUtils {

        /// <summary>
        /// Signal if the executable was started in Headless mode by using the "-batchmode -nographics" command-line arguments
        /// <see cref="https://docs.unity3d.com/Manual/PlayerCommandLineArguments.html"/>
        /// </summary>
        /// <returns>True if in "Headless Mode", false otherwise</returns>
        public static bool IsHeadlessMode() {
            return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
        }

        /// <summary>
        /// Get the value of a specific command-line argument passed when starting the executable
        /// </summary>
        /// <example>
        /// Starting the binary with: "./my-game.exe -map street -type hide-and-seek"
        /// and calling `var mapValue = HeadlessUtils.GetArg("-map", "-m")` will return the string "street"
        /// </example>
        /// <param name="keys">List of possible keys for the argument</param>
        /// <returns>The string value of the argument if the at least 1 key was found, null otherwise</returns>
        public static string GetArg(params string[] keys) {
            var args = Environment.GetCommandLineArgs();

            for ( var i = 0; i < args.Length; i++ ) {
                if ( keys.Any(name => args[i] == name && args.Length > i + 1) ) {
                    return args[i + 1];
                }
            }

            return null;
        }
    }
}