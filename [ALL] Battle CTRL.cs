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

    public static void Open(Emulator emulator) { }
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
            var address = (int[]) property.GetValue(this, null);
            if (address[1] == 2)
            {
                return this.emulator.ReadShortU(address[0]);
            }
            else
            {
                return (int) this.emulator.ReadByteU(address[0]);
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