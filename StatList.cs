﻿using System;
using System.Collections.Generic;

namespace LoDDict_csharp
{
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


        public string Name
        {
            get { return name; }
        }

        public int Element
        {
            get { return element; }
        }

        public int HP
        {
            get { return hp; }
        }

        public int AT
        {
            get { return at; }
        }

        public int MAT
        {
            get { return mat; }
        }

        public int DF
        {
            get { return df; }
        }

        public int MDF
        {
            get { return mdf; }
        }

        public int SPD
        {
            get { return spd; }
        }

        public int A_AV
        {
            get { return a_av; }
        }

        public int M_AV
        {
            get { return m_av; }
        }

        public int P_Immune
        {
            get { return p_immune; }
        }

        public int M_Immune
        {
            get { return m_immune; }
        }

        public int P_Half
        {
            get { return p_half; }
        }

        public int M_Half
        {
            get { return m_half; }
        }

        public int E_Immune
        {
            get { return e_immune; }
        }

        public int E_Half
        {
            get { return e_half; }
        }

        public int Status_Res
        {
            get { return status_res; }
        }

        public int Death_Res
        {
            get { return death_res; }
        }

        public int EXP
        {
            get { return exp; }
        }

        public int Gold
        {
            get { return gold; }
        }

        public int Drop_Item
        {
            get { return drop_item; }
        }

        public int Drop_Chance
        {
            get { return drop_chance; }
        }

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
}