using System;

namespace LoDDict_csharp
{
    public class StatList
    {
        string name = "Monster";
        string element = "Fire";
        int hp = 0;
        int at = 0;
        int mat = 0;
        int df = 0;
        int mdf = 0;
        int spd = 0;

        public string Name
        {
            get { return name; }
        }

        public string Element
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

        public StatList()
        {

        }
        public StatList(string[] monster)
        {
            name = monster[1];
            element = monster[2];
            hp = Int32.Parse(monster[3]);
            at = Int32.Parse(monster[4]);
            mat = Int32.Parse(monster[5]);
            df = Int32.Parse(monster[6]);
            mdf = Int32.Parse(monster[7]);
            spd = Int32.Parse(monster[8]);
        }
    }
}
