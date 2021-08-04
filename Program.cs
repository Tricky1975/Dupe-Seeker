// Lic:
// Dupe Seeker
// Seeks duplicate files
// 
// 
// 
// (c) Jeroen P. Broks, 2021
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 21.08.04
// EndLic

using System;
using System.Collections.Generic;
using TrickyUnits;

namespace DupeSeeker {
    class Program {
        FlagParse CLI_Config;
        Dictionary<string, List<string>> FileDict = new Dictionary<string, List<string>>();

        private Program(string[] args) {
            CLI_Config = new FlagParse(args);
            CLI_Config.Parse();
            Dirry.InitAltDrives();
        }

        void Run() {
            int Errors = 0;
            foreach(var d in CLI_Config.Args) {
                QCol.Doing("     Getting tree", d,"\r");
                try {
                    var Tree = FileList.GetTree(Dirry.AD(d), true, true);
                    QCol.Doing("    Reading files", d, "\r");
                    int i = 0;
                    foreach (var f in Tree) {
                        i = (i + 1) % 4;
                        if (i == 0) Console.Write("\r...\r"); else QCol.Red("*");
                        var file = ($"{d}/{f}").Replace("\\", "/");
                        var Size = QuickStream.FileSize(file);
                        var Hash = qstr.md5(QuickStream.LoadString(Dirry.AD(file)));
                        var Tag = $"{Size}.{Hash}";
                        if (!FileDict.ContainsKey(Tag)) FileDict[Tag] = new List<string>();
                        FileDict[Tag].Add(file);
                    }
                    QCol.Doing("        Completed", d);
                } catch (Exception E) {
                    QCol.Doing("           Failed", d, "\t");
                    QCol.Red($"{E.Message}\n");
                    Errors++;
                }
            }
            int Total = 0;
            QCol.Magenta("\n\n\nResults!\n\n");
            foreach(var hashgroup in FileDict) {
                if (hashgroup.Value.Count > 1) {
                    QCol.Yellow("\nGroup "); QCol.Cyan(hashgroup.Key); QCol.Yellow(" contains "); QCol.Cyan($"{hashgroup.Value.Count}"); QCol.Yellow(" possibly identical files\n");
                    Total += hashgroup.Value.Count;
                    foreach(var fl in hashgroup.Value) {
                        QCol.Red("=> ");
                        QCol.Cyan($"{fl}\n");
                    }
                }
            }
            QCol.Doing("\nTotal", $"{Total}");
            if (Errors > 0) QCol.Doing("Errors", $"{Errors}");
        }


        #region Init!
        static void Main(string[] args) {
            QCol.Yellow("Dupe Seeker\t");
            QCol.Cyan("Coded by: Tricky\n");
            QCol.Magenta("Copyright Jeroen P. Broks, 2021\n");
            QCol.Green("Released under the terms of the General Public License 3\n\n");
            new Program(args).Run();
            TrickyDebug.AttachWait();

        }
        #endregion
    }
}