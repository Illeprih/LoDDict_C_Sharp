using System;

namespace LoDDict_csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string cwd = System.IO.Directory.GetCurrentDirectory();
            var LoDDict = new LoDDict();
            Console.WriteLine(LoDDict.StatList[134].Name);          //Name of ID 134 (Hellena Warden)
            Console.WriteLine(LoDDict.Num2Item[202]);               //Name of item ID 202 (Healing Potion)
            Console.WriteLine(LoDDict.Item2Num["Healing Potion"]);  //ID of Healing Potion (202)
            Console.WriteLine(LoDDict.Element2Num["Dark"]);         //Value of element Dark (4)
            Console.WriteLine(LoDDict.Num2Element[4]);              //Name of element 4 (Dark)
            Console.WriteLine(LoDDict.DragoonStats[0][1].DAT);      //Dragoon Attack of Dart on D'lvl 1
        }
    }
}
