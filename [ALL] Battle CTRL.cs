﻿using Dragoon_Modifier;
using System;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using System.Globalization;
using System.Reflection;

public class BattleCTRL
{
    public static void Run(Emulator emulator)
    {
        if (Globals.FIRST_RUN == true)
        {
            Globals.DICTIONARY = new LoDDict();
            Globals.FIRST_RUN = false;
        }
        int encounterValue = emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE"));
        if (Globals.IN_BATTLE && !Globals.STATS_CHANGED && encounterValue == 41215)
        {
            Constants.WriteOutput("Battle detected. Loading...");
            Globals.BATTLE = new Battle(emulator);
            Constants.WriteDebug("M_Point:        " + Convert.ToString(Globals.BATTLE.m_point, 16).ToUpper());
            Constants.WriteDebug("C_Point:        " + Convert.ToString(Globals.BATTLE.c_point, 16).ToUpper());
            Constants.WriteDebug("Monster Size:        " + Globals.BATTLE.monster_size);
            Constants.WriteDebug("Monster IDs:        " + String.Join(", ", Globals.BATTLE.monster_ID_list.ToArray()));
            Constants.WriteDebug("Unique Monster Size:        " + Globals.BATTLE.unique_monster_size);
            Constants.WriteDebug("Unique Monster IDs:        " + String.Join(", ", Globals.BATTLE.monster_unique_ID_list.ToArray()));
            Globals.STATS_CHANGED = true;
            Constants.WriteOutput("Addresses loaded, Changing stats...");
            for (int monster = 0; monster < Globals.BATTLE.monster_size; monster++)
            {
                int ID = Globals.BATTLE.monster_ID_list[monster];
                Globals.BATTLE.monster_address_list[monster].WriteAddress("HP", Globals.DICTIONARY.StatList[ID].HP);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("Max_HP", Globals.DICTIONARY.StatList[ID].HP);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("ATK", Globals.DICTIONARY.StatList[ID].ATK);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("OG_ATK", Globals.DICTIONARY.StatList[ID].ATK);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("MAT", Globals.DICTIONARY.StatList[ID].MAT);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("OG_MAT", Globals.DICTIONARY.StatList[ID].MAT);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("DEF", Globals.DICTIONARY.StatList[ID].DEF);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("OG_DEF", Globals.DICTIONARY.StatList[ID].DEF);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("MDEF", Globals.DICTIONARY.StatList[ID].MDEF);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("OG_MDEF", Globals.DICTIONARY.StatList[ID].MDEF);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("SPD", Globals.DICTIONARY.StatList[ID].SPD);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("OG_SPD", Globals.DICTIONARY.StatList[ID].SPD);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("A_AV", Globals.DICTIONARY.StatList[ID].A_AV);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("M_AV", Globals.DICTIONARY.StatList[ID].M_AV);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("P_Immune", Globals.DICTIONARY.StatList[ID].P_Immune);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("M_Immune", Globals.DICTIONARY.StatList[ID].M_Immune);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("P_Half", Globals.DICTIONARY.StatList[ID].P_Half);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("M_Half", Globals.DICTIONARY.StatList[ID].M_Half);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("E_Immune", Globals.DICTIONARY.StatList[ID].E_Immune);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("E_Half", Globals.DICTIONARY.StatList[ID].E_Half);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("Stat_Res", Globals.DICTIONARY.StatList[ID].Stat_Res);
                Globals.BATTLE.monster_address_list[monster].WriteAddress("Death_Res", Globals.DICTIONARY.StatList[ID].Death_Res);
            }
            for (int monster = 0; monster < Globals.BATTLE.unique_monster_size; monster++)
            {
                int ID = Globals.BATTLE.monster_unique_ID_list[monster];
                emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + (int)Constants.OFFSET + monster * 0x8, (ushort)Globals.DICTIONARY.StatList[ID].EXP);
                emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + 0x2 + (int)Constants.OFFSET + monster * 0x8, (ushort)Globals.DICTIONARY.StatList[ID].Gold);
                emulator.WriteByteU(Constants.GetAddress("MONSTER_REWARDS") + 0x4 + (int)Constants.OFFSET + monster * 0x8, (byte)Globals.DICTIONARY.StatList[ID].Drop_Chance);
                emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + 0x5 + (int)Constants.OFFSET + monster * 0x8, (ushort)Globals.DICTIONARY.StatList[ID].Drop_Item);
            }
            Constants.WriteOutput("Finished loading.");
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
        Globals.DICTIONARY = new LoDDict();
    }
    public static void Close(Emulator emulator) { }
    public static void Click(Emulator emulator)
    {
        Globals.DICTIONARY = new LoDDict();
    }
}

public class Battle
{
    public int encounter_ID = 0;
    public int m_point = 0x0;
    public int c_point = 0x0;
    public int monster_size = 1;
    public int unique_monster_size = 1;
    public List<int> monster_ID_list = new List<int>();
    public List<int> monster_unique_ID_list = new List<int>();
    public List<dynamic> monster_address_list = new List<dynamic>();

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
        Thread.Sleep(1000);
        unique_monster_size = emulator.ReadByte(Constants.GetAddress("UNIQUE_MONSTERS"));
        foreach (int monster in Enumerable.Range(0, unique_monster_size))
        {
            monster_unique_ID_list.Add(emulator.ReadShort(Constants.GetAddress("UNIQUE_SLOT") + (monster * 0x1A8)));
        }
        monster_size = emulator.ReadByte(Constants.GetAddress("MONSTER_SIZE"));
        foreach (int monster in Enumerable.Range(0, monster_size))
        {
            monster_ID_list.Add(emulator.ReadShort(Constants.GetAddress("MONSTER_ID") + GetOffset() + (monster * 0x8)));
            monster_address_list.Add(new MonsterAddress(m_point, monster, monster_ID_list[monster], monster_unique_ID_list, emulator));
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
        int[] spd = { 0, 2 };
        int[] og_spd = { 0, 2 };
        int[] turn = { 0, 2 };
        int[] a_av = { 0, 1 };
        int[] m_av = { 0, 1 };
        int[] p_immune = { 0, 1 };
        int[] m_immune = { 0, 1 };
        int[] p_half = { 0, 1 };
        int[] m_half = { 0, 1 };
        int[] e_immune = { 0, 1 };
        int[] e_half = { 0, 1 };
        int[] stat_res = { 0, 1 };
        int[] death_res = { 0, 1 };
        int[] unique_index = { 0, 1 };
        int[] exp = { 0, 2 };
        int[] gold = { 0, 2 };
        int[] drop_chance = { 0, 1 };
        int[] drop_item = { 0, 2 };
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
        public int[] SPD { get { return spd; } }
        public int[] OG_SPD { get { return og_spd; } }
        public int[] Turn { get { return turn; } }
        public int[] A_AV { get { return a_av; } }
        public int[] M_AV { get { return m_av; } }
        public int[] P_Immune { get { return p_immune; } }
        public int[] M_Immune { get { return m_immune; } }
        public int[] P_Half { get { return p_half; } }
        public int[] M_Half { get { return m_half; } }
        public int[] E_Immune { get { return e_immune; } }
        public int[] E_Half { get { return e_half; } }
        public int[] Stat_Res { get { return stat_res; } }
        public int[] Death_Res { get { return death_res; } }
        public int[] Unique_Index { get { return unique_index; } }
        public int[] EXP { get { return exp; } }
        public int[] Gold { get { return gold; } }
        public int[] Drop_Chance { get { return drop_chance; } }
        public int[] Drop_Item { get { return drop_item; } }

        public MonsterAddress(int m_point, int monster, int ID, List<int> monster_unique_ID_list, Emulator emu)
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
            spd[0] = m_point + 0x2A - monster * 0x388;
            og_spd[0] = m_point + 0x5C - monster * 0x388;
            turn[0] = m_point + 0x44 - monster * 0x388;
            a_av[0] = m_point + 0x38 - monster * 0x388;
            m_av[0] = m_point + 0x3A - monster * 0x388;
            p_immune[0] = m_point + 0x10 - monster * 0x388;
            m_immune[0] = m_point + 0x10 - monster * 0x388;
            p_half[0] = m_point + 0x10 - monster * 0x388;
            m_half[0] = m_point + 0x10 - monster * 0x388;
            e_immune[0] = m_point + 0x1A - monster * 0x388;
            e_half[0] = m_point + 0x18 - monster * 0x388;
            stat_res[0] = m_point + 0x1C - monster * 0x388;
            death_res[0] = m_point + 0x0C - monster * 0x388;
            unique_index[0] = m_point + 0x264 - monster * 0x388;
            exp[0] = Constants.GetAddress("MONSTER_REWARDS") + (int)Constants.OFFSET + monster_unique_ID_list.IndexOf(ID) * 0x8;
            gold[0] = Constants.GetAddress("MONSTER_REWARDS") + 0x2 + (int)Constants.OFFSET + monster_unique_ID_list.IndexOf(ID) * 0x8;
            drop_chance[0] = Constants.GetAddress("MONSTER_REWARDS") + 0x4 + (int)Constants.OFFSET + monster_unique_ID_list.IndexOf(ID) * 0x8;
            drop_item[0] = Constants.GetAddress("MONSTER_REWARDS") + 0x5 + (int)Constants.OFFSET + monster_unique_ID_list.IndexOf(ID) * 0x8;
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
        string cwd = AppDomain.CurrentDomain.BaseDirectory;
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
    int atk = 0;
    int mat = 0;
    int def = 0;
    int mdef = 0;
    int spd = 0;
    int a_av = 0;
    int m_av = 0;
    int p_immune = 0;
    int m_immune = 0;
    int p_half = 0;
    int m_half = 0;
    int e_immune = 0;
    int e_half = 0;
    int stat_res = 0;
    int death_res = 0;
    int exp = 0;
    int gold = 0;
    int drop_item = 255;
    int drop_chance = 0;

    public string Name { get { return name; } }
    public int Element { get { return element; } }
    public int HP { get { return hp; } }
    public int ATK { get { return atk; } }
    public int MAT { get { return mat; } }
    public int DEF { get { return def; } }
    public int MDEF { get { return mdef; } }
    public int SPD { get { return spd; } }
    public int A_AV { get { return a_av; } }
    public int M_AV { get { return m_av; } }
    public int P_Immune { get { return p_immune; } }
    public int M_Immune { get { return m_immune; } }
    public int P_Half { get { return p_half; } }
    public int M_Half { get { return m_half; } }
    public int E_Immune { get { return e_immune; } }
    public int E_Half { get { return e_half; } }
    public int Stat_Res { get { return stat_res; } }
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
        atk = Int32.Parse(monster[4]);
        mat = Int32.Parse(monster[5]);
        def = Int32.Parse(monster[6]);
        mdef = Int32.Parse(monster[7]);
        spd = Int32.Parse(monster[8]);
        a_av = Int32.Parse(monster[9]);
        m_av = Int32.Parse(monster[10]);
        p_immune = Int32.Parse(monster[11]);
        m_immune = Int32.Parse(monster[12]);
        p_half = Int32.Parse(monster[13]);
        m_half = Int32.Parse(monster[14]);
        e_immune = element2num[monster[15]];
        e_half = element2num[monster[16]];
        stat_res = Int32.Parse(monster[17]);
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
    int ddef = 0;
    int dmdef = 0;

    public int DAT { get { return dat; } }
    public int DMAT { get { return dmat; } }
    public int DDEF { get { return ddef; } }
    public int DMDEF { get { return dmdef; } }

    public DragoonStats(int ndat, int ndmat, int nddef, int ndmdef)
    {
        dat = ndat;
        dmat = ndmat;
        ddef = nddef;
        dmdef = ndmdef;
    }
}