/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class MadExternalTools {

    private const string ExternalPath = @"Assets\Mad Level Manager\Scripts\Mad2D\Editor\External";



    private static string Run(string scriptName, string arguments) {
        var start = new ProcessStartInfo();
        start.Arguments = "-I " + ExternalPath + " " + ExternalPath + "\\" + scriptName + " " + arguments;
        start.FileName = "ruby";
        start.WindowStyle = ProcessWindowStyle.Hidden;
        start.CreateNoWindow = true;

        using (Process proc = Process.Start(start)) {
            proc.WaitForExit();
            var exitCode = proc.ExitCode;
            UnityEngine.Debug.Log(exitCode);
        }

        return "";
    }

}