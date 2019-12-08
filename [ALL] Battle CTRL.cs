using Dragoon_Modifier;
using System;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using System.Globalization;
using System.Reflection;
using System.Xml;

public class BattleCTRL
{
    public static void Run(Emulator emulator)
    {
        int encounterValue = emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE"));
        if (Globals.IN_BATTLE && !Globals.STATS_CHANGED && encounterValue == 41215)
        {
            Constants.WriteOutput("Battle detected. Loading...");
            Globals.C_POINT = 0;
            for (int i = 0; i < 5; i++)
            {
                Globals.MONSTER_IDS[i] = 32767;
            }
            Thread.Sleep(3000);
            Globals.MONSTER_SIZE = emulator.ReadByte(Constants.GetAddress("MONSTER_SIZE"));
            Globals.UNIQUE_MONSTERS = emulator.ReadByte(Constants.GetAddress("UNIQUE_MONSTERS"));

            if (Constants.REGION == Region.USA)
            {
                Globals.M_POINT = 0x1A439C + emulator.ReadShort(Constants.GetAddress("M_POINT"));
            }
            else
            {
                Globals.M_POINT = 0x1A43B4 + emulator.ReadShort(Constants.GetAddress("M_POINT"));
            }

            Globals.C_POINT = (int)(emulator.ReadInteger(Constants.GetAddress("C_POINT")) - 0x7F5A8558 - (uint)Constants.OFFSET);
            for (int i = 0; i < Globals.MONSTER_SIZE; i++)
            {
                Globals.MONSTER_IDS[i] = emulator.ReadShort(Constants.GetAddress("MONSTER_ID") + GetOffset() + (i * 0x8));
            }
            Globals.STATS_CHANGED = true;

            Constants.WriteDebug("Monster Size:      " + Globals.MONSTER_SIZE);
            Constants.WriteDebug("Unique Monsters:   " + Globals.UNIQUE_MONSTERS);
            Constants.WriteDebug("Monster Point:     " + Convert.ToString(Globals.M_POINT + Constants.OFFSET, 16).ToUpper());
            Constants.WriteDebug("Character Point:   " + Convert.ToString(Globals.C_POINT + Constants.OFFSET, 16).ToUpper());
            Constants.WriteDebug("Monster HP:        " + emulator.ReadShort(Globals.M_POINT));
            Constants.WriteDebug("Character HP:      " + emulator.ReadShort(Globals.C_POINT));
            for (int i = 0; i < Globals.MONSTER_SIZE; i++)
            {
                Constants.WriteDebug("Monster ID Slot " + (i + 1) + ": " + Globals.MONSTER_IDS[i]);
            }
            Constants.WriteOutput("Finished loading.");
            var battle = new Battle(emulator);
            Constants.WriteDebug("M_Point:        " + Convert.ToString(battle.m_point, 16).ToUpper());
            Constants.WriteDebug("Monster 1 HP:        " + battle.monster_address_list[0].ReadAddress("HP"));
            battle.monster_address_list[0].WriteAddress("HP", 10);
            Constants.WriteDebug("Monster 1 HP:        " + battle.monster_address_list[0].ReadAddress("HP"));
            /* when BATTLE is in globals "public static dynamic BATTLE = new System.Dynamic.ExpandoObject();"
            Globals.BATTLE = new Battle(emulator);
            Constants.WriteDebug("M_Point:        " + Convert.ToString(Globals.BATTLE.m_point, 16).ToUpper());
            Constants.WriteDebug("Monster 1 HP:        " + Globals.BATTLE.monster_address_list[0].ReadAddress("HP"));
            Globals.BATTLE.monster_address_list[0].WriteAddress("HP", 10);
            Constants.WriteDebug("Monster 1 HP:        " + Globals.BATTLE.monster_address_list[0].ReadAddress("HP"));
             */
        }
        else
        {
            if (Globals.STATS_CHANGED && encounterValue < 9999)
            {
                Globals.STATS_CHANGED = false;
                Constants.WriteOutput("Exiting out of battle.");
            }
        }
    }

    public static int GetOffset()
    {
        int[] discOffset = { 0xDB0, 0x0, 0x1458, 0x1B0 };
        int[] charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
        int partyOffset = 0;
        if (Globals.PARTY_SLOT[0] < 9 && Globals.PARTY_SLOT[1] < 9 && Globals.PARTY_SLOT[2] < 9)
        {
            partyOffset = charOffset[Globals.PARTY_SLOT[1]] + charOffset[Globals.PARTY_SLOT[2]];
        }
        return discOffset[Globals.DISC - 1] - partyOffset;
    }

    public static void Open(Emulator emulator) 
    {
        var dictionary = new LoDDict();
    }
    public static void Close(Emulator emulator) { }
    public static void Click(Emulator emulator) { }
}

public class Battle
{
    public int encounter_ID = 0;
    public int m_point = 0x0;
    public int c_point = 0x0;
    public int monster_size = 1;
    public int unique_monster_size = 1;
    public int[] monster_ID_list = new int[5];
    public int[] monster_unique_ID_list = new int[3];
    public dynamic[] monster_address_list = new dynamic[5];

    public Battle(Emulator emulator)
    {
        if (Constants.REGION == Region.USA)
        {
            m_point = 0x1A439C + emulator.ReadShort(Constants.GetAddress("M_POINT")) + (int)Constants.OFFSET;
        }
        else
        {
            m_point = 0x1A43B4 + emulator.ReadShort(Constants.GetAddress("M_POINT")) + (int)Constants.OFFSET;
        }
        c_point = (int)(emulator.ReadInteger(Constants.GetAddress("C_POINT")) - 0x7F5A8558);
        monster_size = emulator.ReadByte(Constants.GetAddress("MONSTER_SIZE"));
        foreach (int monster in Enumerable.Range(0, monster_size))
        {
            monster_ID_list[monster] = emulator.ReadShort(Constants.GetAddress("MONSTER_ID") + GetOffset() + (monster * 0x8));
            monster_address_list[monster] = new MonsterAddress(m_point, monster, emulator);
        }
    }

    public static int GetOffset()
    {
        int[] discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
        int[] charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
        int partyOffset = 0;
        if (Globals.PARTY_SLOT[0] < 9 && Globals.PARTY_SLOT[1] < 9 && Globals.PARTY_SLOT[2] < 9)
        {
            partyOffset = charOffset[Globals.PARTY_SLOT[1]] + charOffset[Globals.PARTY_SLOT[2]];
        }
        return discOffset[Globals.DISC - 1] - partyOffset;
    }

    public class MonsterAddress
    {
        int[] hp = { 0, 2 };
        int[] max_hp = { 0, 2 };
        int[] element = { 0, 2 };
        int[] display_element = { 0, 2 };
        int[] atk = { 0, 2 };
        int[] og_atk = { 0, 2 };
        int[] mat = { 0, 2 };
        int[] og_mat = { 0, 2 };
        int[] def = { 0, 2 };
        int[] og_def = { 0, 2 };
        int[] mdef = { 0, 2 };
        int[] og_mdef = { 0, 2 };
        public Emulator emulator = null;

        public int[] HP { get { return hp; } }
        public int[] Max_HP { get { return max_hp; } }
        public int[] Element { get { return element; } }
        public int[] Display_Element { get { return display_element; } }
        public int[] ATK { get { return atk; } }
        public int[] OG_ATK { get { return og_atk; } }
        public int[] MAT { get { return mat; } }
        public int[] OG_MAT { get { return og_mat; } }
        public int[] DEF { get { return def; } }
        public int[] OG_DEF { get { return og_def; } }
        public int[] MDEF { get { return mdef; } }
        public int[] OG_MDEF { get { return og_mdef; } }

        public MonsterAddress(int m_point, int monster, Emulator emu)
        {
            emulator = emu;
            hp[0] = m_point - monster * 0x388;
            max_hp[0] = m_point + 0x8 - monster * 0x388;
            element[0] = m_point + 0x6a - monster * 0x388;
            display_element[0] = m_point + 0x14 - monster * 0x388;
            atk[0] = m_point + 0x2c - monster * 0x388;
            og_atk[0] = m_point + 0x58 - monster * 0x388;
            mat[0] = m_point + 0x2E - monster * 0x388;
            og_mat[0] = m_point + 0x5A - monster * 0x388;
            def[0] = m_point + 0x30 - monster * 0x388;
            og_def[0] = m_point + 0x5E - monster * 0x388;
            mdef[0] = m_point + 0x32 - monster * 0x388;
            og_mdef[0] = m_point + 0x60 - monster * 0x388;
        }

        public int ReadAddress(string attribute)
        {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (int[])property.GetValue(this, null);
            if (address[1] == 2)
            {
                return this.emulator.ReadShortU(address[0]);
            }
            else
            {
                return (int)this.emulator.ReadByteU(address[0]);
            }
        }

        public void WriteAddress(string attribute, int value)
        {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (int[])property.GetValue(this, null);
            if (address[1] == 2)
            {
                this.emulator.WriteShortU(address[0], (ushort)value);
            }
            else
            {
                this.emulator.WriteByteU(address[0], (byte)value);
            }
        }
    }
}

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

    public IDictionary<int, dynamic> StatList { get { return statList; } }
    public IDictionary<int, string> Num2Item { get { return num2item; } }
    public IDictionary<string, int> Item2Num { get { return item2num; } }
    public IDictionary<int, string> Num2Element { get { return num2element; } }
    public IDictionary<string, int> Element2Num { get { return element2num; } }
    public IDictionary<int, Dictionary<int, dynamic>> DragoonStats { get { return dragoonStats; } }

    public LoDDict()
    {
        string cwd = AppDomain.CurrentDomain.BaseDirectory;
        string[] lines = File.ReadAllLines(cwd + "Mods/Base/Item_List.txt");
        var i = 0;
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
        using (var monsterData = new StreamReader(cwd + "Mods/Base/Monster_Data.csv"))
        {
            bool firstline = true;
            while (!monsterData.EndOfStream)
            {
                var line = monsterData.ReadLine();
                if (firstline == false)
                {
                    var values = line.Split(',').ToArray();
                    statList.Add(Int32.Parse(values[0]), new StatList(values, element2num, item2num));
                }
                else
                {
                    firstline = false;
                }
            }
        }
        using (var dragoon = new StreamReader(cwd + "/Mods/Base/Dragoon_Stats.csv"))
        {
            bool firstline = true;
            i = 0;
            while (!dragoon.EndOfStream)
            {
                var line = dragoon.ReadLine();
                if (firstline == false)
                {
                    var values = line.Split(',').ToArray();
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
        }
    }
    public LoDDict(string path)
    {
        string[] lines = File.ReadAllLines(path + "/Item_List.txt");
        var i = 0;
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
        using (var monsterData = new StreamReader(path + "/Monster_Data.csv"))
        {
            bool firstline = true;
            while (!monsterData.EndOfStream)
            {
                var line = monsterData.ReadLine();
                if (firstline == false)
                {
                    var values = line.Split(',').ToArray();
                    statList.Add(Int32.Parse(values[0]), new StatList(values, element2num, item2num));
                }
                else
                {
                    firstline = false;
                }
            }
        }
        using (var dragoon = new StreamReader(path + "/Dragoon_Stats.csv"))
        {
            bool firstline = true;
            i = 0;
            while (!dragoon.EndOfStream)
            {
                var line = dragoon.ReadLine();
                if (firstline == false)
                {
                    var values = line.Split(',').ToArray();
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
        }
    }
}

public class StatList
{
    string name = "Monster";
    int element = 128;
    int hp = 0;
    int at = 0;
    int mat = 0;
    int df = 0;
    int mdf = 0;
    int spd = 0;
    int a_av = 0;
    int m_av = 0;
    int p_immune = 0;
    int m_immune = 0;
    int p_half = 0;
    int m_half = 0;
    int e_immune = 0;
    int e_half = 0;
    int status_res = 0;
    int death_res = 0;
    int exp = 0;
    int gold = 0;
    int drop_item = 255;
    int drop_chance = 0;

    public string Name { get { return name; } }
    public int Element { get { return element; } }
    public int HP { get { return hp; } }
    public int AT { get { return at; } }
    public int MAT { get { return mat; } }
    public int DF { get { return df; } }
    public int MDF { get { return mdf; } }
    public int SPD { get { return spd; } }
    public int A_AV { get { return a_av; } }
    public int M_AV { get { return m_av; } }
    public int P_Immune { get { return p_immune; } }
    public int M_Immune { get { return m_immune; } }
    public int P_Half { get { return p_half; } }
    public int M_Half { get { return m_half; } }
    public int E_Immune { get { return e_immune; } }
    public int E_Half { get { return e_half; } }
    public int Status_Res { get { return status_res; } }
    public int Death_Res { get { return death_res; } }
    public int EXP { get { return exp; } }
    public int Gold { get { return gold; } }
    public int Drop_Item { get { return drop_item; } }
    public int Drop_Chance { get { return drop_chance; } }

    public StatList(string[] monster, IDictionary<string, int> element2num, IDictionary<string, int> item2num)
    {
        name = monster[1];
        element = element2num[monster[2]];
        hp = Int32.Parse(monster[3]);
        at = Int32.Parse(monster[4]);
        mat = Int32.Parse(monster[5]);
        df = Int32.Parse(monster[6]);
        mdf = Int32.Parse(monster[7]);
        spd = Int32.Parse(monster[8]);
        a_av = Int32.Parse(monster[9]);
        m_av = Int32.Parse(monster[10]);
        p_immune = Int32.Parse(monster[11]);
        m_immune = Int32.Parse(monster[12]);
        p_half = Int32.Parse(monster[13]);
        m_half = Int32.Parse(monster[14]);
        e_immune = element2num[monster[15]];
        e_half = element2num[monster[16]];
        status_res = Int32.Parse(monster[17]);
        death_res = Int32.Parse(monster[18]);
        exp = Int32.Parse(monster[19]);
        gold = Int32.Parse(monster[20]);
        drop_item = item2num[monster[21]];
        drop_chance = Int32.Parse(monster[22]);
    }
}

public class DragoonStats
{
    int dat = 0;
    int dmat = 0;
    int ddf = 0;
    int dmdf = 0;

    public int DAT { get { return dat; } }
    public int DMAT { get { return dmat; } }
    public int DDF { get { return ddf; } }
    public int DMDF { get { return dmdf; } }

    public DragoonStats(int ndat, int ndmat, int nddf, int ndmdf)
    {
        dat = ndat;
        dmat = ndmat;
        ddf = nddf;
        dmdf = ndmdf;
    }
}