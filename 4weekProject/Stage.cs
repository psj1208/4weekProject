using _4weekProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace _4weekProject
{
    //데미지의 랜덤성을 위한 구조체
    public struct RandomBetween
    {
        public int start;
        public int end;

        public RandomBetween(int x,int y)
        {
            start = x;
            end = y;
        }
    }
    //랜덤22
    public static class Randomize
    {
        //데미지 범위 내를 랜덤으로 반환
        public static int Makenum(RandomBetween random)
        {
            Random rand = new Random();
            return rand.Next(random.start, random.end + 1);
        }

        //per확률로 player에게 item 부여
        public static void RandomGain(Player player,IItem item,int per)
        {
            Random rand = new Random();
            if (rand.Next(0, 101) <= per) 
                player.GainItem(item);
        }
    }
    //몬스터와 몬스터 조우 확률
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

    //클리어 시 보상
    public static class StageClear
    {
        public static List<Action<Player>> ClearMethod = new List<Action<Player>>
        {
            Clear1,
            Clear2,
            Clear3
        };
         static void Clear1(Player player)
        {
            player.GainGold(Randomize.Makenum(new RandomBetween(20, 50)));
            Randomize.RandomGain(player, ItemDataBase.WeaponList[0], 20);
            Randomize.RandomGain(player, ItemDataBase.ArmourList[0], 20);
            Randomize.RandomGain(player, ItemDataBase.ConsumeList[0], 40);
            Randomize.RandomGain(player, ItemDataBase.ConsumeList[1], 5);
        }
        static void Clear2(Player player)
        {
            player.GainGold(Randomize.Makenum(new RandomBetween(40, 70)));
            Randomize.RandomGain(player, ItemDataBase.WeaponList[1], 20);
            Randomize.RandomGain(player, ItemDataBase.ArmourList[1], 20);
            Randomize.RandomGain(player, ItemDataBase.ConsumeList[0], 50);
            Randomize.RandomGain(player, ItemDataBase.ConsumeList[0], 50);
            Randomize.RandomGain(player, ItemDataBase.ConsumeList[1], 8);
        }

        static void Clear3(Player player)
        {
            player.GainGold(Randomize.Makenum(new RandomBetween(60, 90)));
            Randomize.RandomGain(player, ItemDataBase.WeaponList[2], 20);
            Randomize.RandomGain(player, ItemDataBase.ArmourList[2], 20);
            Randomize.RandomGain(player, ItemDataBase.ConsumeList[0], 50);
            Randomize.RandomGain(player, ItemDataBase.ConsumeList[0], 50);
            Randomize.RandomGain(player, ItemDataBase.ConsumeList[1], 10);
        }
    }

    //스테이지 데이터베이스
    public class StageDB
    {
        public static List<Stage> stageList = new List<Stage>
        {
            new Stage(3,new StageSpawn(MonsterDB.goblin,10),
                new StageSpawn(MonsterDB.wolf,10),
                new StageSpawn(MonsterDB.bat,10)),
            new Stage(5,new StageSpawn(MonsterDB.goblin,10),
                new StageSpawn(MonsterDB.wolf,10),
                new StageSpawn(MonsterDB.slime, 10),
                new StageSpawn(MonsterDB.Rare_1, 1)),
            new Stage(7, new StageSpawn(MonsterDB.goblin, 10),
                new StageSpawn(MonsterDB.slime, 10),
                new StageSpawn(MonsterDB.wolf, 10),
                new StageSpawn(MonsterDB.Cow, 5),
                new StageSpawn(MonsterDB.Rare_1,2)),
            new Stage(9, new StageSpawn(MonsterDB.wolf,10),
                new StageSpawn(MonsterDB.slime,10),
                new StageSpawn(MonsterDB.wolf,5),
                new StageSpawn(MonsterDB.Cow,2),
                new StageSpawn(MonsterDB.Rare_1,3),
                new StageSpawn(MonsterDB.dragon,5))
        };
    }

    //몬스터 데이터베이스(static으로 선언해서 접근하기 편하게)
    public static class MonsterDB
    {
        public static Monster goblin = new Monster("고블린", 30, new RandomBetween(4, 6), 6, new RandomBetween(5, 15));
        public static Monster bat = new Monster("박쥐", 10, new RandomBetween(3,4), 3, new RandomBetween(3,8));
        public static Monster wolf = new Monster("늑대", 20, new RandomBetween(6,9), 8, new RandomBetween(8,17));
        public static Monster slime = new Monster("슬라임", 35, new RandomBetween(5,7), 10, new RandomBetween(10, 14));
        public static Monster Cow = new Monster("카우 킹", 50, new RandomBetween(10, 14), 20, new RandomBetween(20, 30));
        public static Monster Rare_1 = new Monster("황금 고블린", 1, new RandomBetween(1, 1), 30, new RandomBetween(1, 300));
        public static Monster dragon = new Monster("드래곤", 100, new RandomBetween(15, 25), 100, new RandomBetween(50, 100));
    }
    //스테이지 클래스. 등장 몬스터와 던전 길이.
    public class Stage
    {
        public List<StageSpawn> monsters = new List<StageSpawn>();
        public int length { get; set; }
        int totalProb;

        public Stage(int leng = 0, params StageSpawn[] spawn)
        {
            length = leng;
            foreach(var s in spawn)
            {
                monsters.Add(s);
            }
            SetTotalProb();
        }

        //가중치 합계 및 설정
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


        //가중치에 따라 랜덤으로 몬스터 하나 반환
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
