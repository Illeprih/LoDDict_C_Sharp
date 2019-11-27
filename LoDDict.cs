using System;
using System.Collections.Generic;
using System.IO;

namespace LoDDict_csharp
{


    public class LoDDict
    {
        IDictionary<int, dynamic> statList = new Dictionary<int, dynamic>();
        IDictionary<int, Dictionary<int, dynamic>> dragoonStats = new Dictionary<int, Dictionary<int, dynamic>>();
        IDictionary<int, string> num2item = new Dictionary<int, string>();
        IDictionary<string, int> item2num = new Dictionary<string, int>();
        IDictionary<int, string> num2element = new Dictionary<int, string>()
        {
            {0, "None" },
            {1, "Water" },
            {2, "Earth" },
            {4, "Dark" },
            {8, "Non-Elemental" },
            {16, "Thunder" },
            {32, "Light" },
            {64, "Wind" },
            {128, "Fire" }
        };
        IDictionary<string, int> element2num = new Dictionary<string, int>()
        {
            {"None", 0 },
            {"Water", 1 },
            {"Earth", 2 },
            {"Dark", 4 },
            {"Non-Elemental", 8 },
            {"Thunder", 16 },
            {"Light", 32 },
            {"Wind", 64 },
            {"Fire", 128 }
        };

        public IDictionary<int, dynamic> StatList
        {
            get { return statList; }
        }

        public IDictionary<int, string> Num2Item
        {
            get { return num2item; }
        }

        public IDictionary<string, int> Item2Num
        {
            get { return item2num; }
        }

        public IDictionary<int, string> Num2Element
        {
            get { return num2element; }
        }

        public IDictionary<string, int> Element2Num
        {
            get { return element2num; }
        }

        public IDictionary<int, Dictionary<int, dynamic>> DragoonStats
        {
            get { return dragoonStats; }
        }

        public LoDDict()
        {
            string cwd = System.AppDomain.CurrentDomain.BaseDirectory;
            using var monsterData = new StreamReader(cwd + "/Mods/Base/Monster_Data.csv");
            bool firstline = true;
            while (!monsterData.EndOfStream)
            {
                var line = monsterData.ReadLine();
                if (firstline == false)
                {
                    var values = line.Split(",");
                    statList.Add(Int32.Parse(values[0]), new StatList(values));
                }
                else
                {
                    firstline = false;
                }
            }
            using var dragoon = new StreamReader(cwd + "/Mods/Base/Dragoon_Stats.csv");
            firstline = true;
            var i = 0;
            while (!dragoon.EndOfStream)
            {
                var line = dragoon.ReadLine();
                if (firstline == false)
                {
                    var values = line.Split(",");
                    Dictionary<int, dynamic> level = new Dictionary<int, dynamic>();
                    level.Add(1, new DragoonStats(Int32.Parse(values[1]), Int32.Parse(values[2]), Int32.Parse(values[3]), Int32.Parse(values[4])));
                    level.Add(2, new DragoonStats(Int32.Parse(values[5]), Int32.Parse(values[6]), Int32.Parse(values[7]), Int32.Parse(values[8])));
                    level.Add(3, new DragoonStats(Int32.Parse(values[9]), Int32.Parse(values[10]), Int32.Parse(values[11]), Int32.Parse(values[12])));
                    level.Add(4, new DragoonStats(Int32.Parse(values[13]), Int32.Parse(values[14]), Int32.Parse(values[15]), Int32.Parse(values[16])));
                    level.Add(5, new DragoonStats(Int32.Parse(values[17]), Int32.Parse(values[18]), Int32.Parse(values[19]), Int32.Parse(values[20])));
                    dragoonStats.Add(i - 1, level);
                }
                else
                {
                    firstline = false;
                }
                i++;
            }
            string[] lines = File.ReadAllLines(cwd + "/Mods/Base/Item_List.txt");
            i = 0;
            foreach (string row in lines)
            {
                if (i > 0)
                {
                    if (row != "")
                    {
                        item2num.Add(row, i - 1);
                        num2item.Add(i - 1, row);
                    }
                }
                i++;
            }
        }

        public LoDDict(string path)
        {
            using var monsterData = new StreamReader(path + "/Monster_Data.csv");
            bool firstline = true;
            while (!monsterData.EndOfStream)
            {
                var line = monsterData.ReadLine();
                if (firstline == false)
                {
                    var values = line.Split(",");
                    statList.Add(Int32.Parse(values[0]), new StatList(values));
                }
                else
                {
                    firstline = false;
                }
            }
            using var dragoon = new StreamReader(path + "/Dragoon_Stats.csv");
            firstline = true;
            var i = 0;
            while (!dragoon.EndOfStream)
            {
                var line = dragoon.ReadLine();
                if (firstline == false)
                {
                    var values = line.Split(",");
                    Dictionary<int, dynamic> level = new Dictionary<int, dynamic>();
                    level.Add(1, new DragoonStats(Int32.Parse(values[1]), Int32.Parse(values[2]), Int32.Parse(values[3]), Int32.Parse(values[4])));
                    level.Add(2, new DragoonStats(Int32.Parse(values[5]), Int32.Parse(values[6]), Int32.Parse(values[7]), Int32.Parse(values[8])));
                    level.Add(3, new DragoonStats(Int32.Parse(values[9]), Int32.Parse(values[10]), Int32.Parse(values[11]), Int32.Parse(values[12])));
                    level.Add(4, new DragoonStats(Int32.Parse(values[13]), Int32.Parse(values[14]), Int32.Parse(values[15]), Int32.Parse(values[16])));
                    level.Add(5, new DragoonStats(Int32.Parse(values[17]), Int32.Parse(values[18]), Int32.Parse(values[19]), Int32.Parse(values[20])));
                    dragoonStats.Add(i - 1, level);
                }
                else
                {
                    firstline = false;
                }
                i++;
            }
            string[] lines = File.ReadAllLines(path + "/Item_List.txt");
            i = 0;
            foreach (string row in lines)
            {
                if (i > 0)
                {
                    if (row != "")
                    {
                        item2num.Add(row, i - 1);
                        num2item.Add(i - 1, row);
                    }
                }
                i++;
            }
        }
    }
}
