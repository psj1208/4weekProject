using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace _4weekProject
{
    public struct Randomize
    {
        int start;
        int end;
    }
    public struct StageSpawn
    {
        public Monster monster { get; set; }
        public int prob { get; set; }

        public int curProb { get; set; }

        public StageSpawn(Monster mons, int prob_)
        {
            monster= mons;
            prob = prob_;
            curProb = 0;
        }
    }

    public class Stage
    {
        List<StageSpawn> monsters = new List<StageSpawn>();
        public int length { get; set; }
        Monster goblin = new Monster("고블린", 30, 5, 5);
        Monster bat = new Monster("박쥐", 10, 3, 3);
        Monster wolf = new Monster("늑대", 20, 8, 7);
        int totalProb;

        public Stage(int i = 0)
        {
            monsters.Clear();
            switch(i)
            {
                case 1:
                    monsters.Add(new StageSpawn(goblin,10));
                    monsters.Add(new StageSpawn(bat, 10));
                    monsters.Add(new StageSpawn(wolf, 10));
                    length = 3;
                    break;
                default:
                    break;
            }
            SetTotalProb();
        }

        public void SetTotalProb()
        {
            totalProb = 0;
            for (int i = 0; i < monsters.Count; i++)
            {
                totalProb += monsters[i].prob;
                StageSpawn spawn = monsters[i];
                spawn.curProb = totalProb;
                monsters[i] = spawn;
            }
        }
        public Monster MakeMonster()
        {
            Random rand = new Random();
            int randomseed = rand.Next(1, totalProb);
            for (int i = 0; i < monsters.Count; i++)
            {
                if (randomseed <= monsters[i].curProb)
                    return monsters[i].monster;
            }
            return null;
        }
    }
}
