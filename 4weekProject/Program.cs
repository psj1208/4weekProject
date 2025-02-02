using System.ComponentModel.Design;
using System.Drawing;
using System.Net.Security;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static GlobalData;

namespace _4weekProject
{
    public static class Number
    {
        public static int[] Make(int start,int end)
        {
            int[] ints = new int[end - start + 1];
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = start + i;
            }
            return ints;
        }
    }
    public struct Point_
    {
        int x;
        int y;
    }
    public enum CurSceneType
    {
        Lobby,
        Home,
        Explore,
        Store
    }

    public enum ItemType
    {
        Consumable,
        NonConsumable
    }
    public enum EquipmentType
    {
        weapon,
        armour
    }
    public enum CharacterType
    {
        Player, //Warrior 타입을 대체합니다.
        Monster
    }
    public interface ICharacter
    {
        string Name { get; set; }
        int Health { get; set; }
        int Attack { get; set; }
        bool IsDead { get; set; }
        CharacterType type { get; set; }
        void TakeDamage(int damage);
    }

    public class Monster : ICharacter
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public bool IsDead { get; set; }
        public CharacterType type { get; set; } = CharacterType.Monster;

        public Monster(string name, int hp, int atk)
        {
            Name = name;
            Health = hp;
            Attack = atk;
            IsDead = false;
        }
        public void TakeDamage(int damage)
        {
            Health -= damage;
            Text.TextingLine($"{Name}이 {damage}피해를 받았습니다!");
        }
    }
    //Warrior 클래스를 대체합니다.
    public class Player : ICharacter
    {
        public int level;
        public string Name { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int defense { get; set; }
        public int Gold;

        public Inventory inven;
        public bool IsDead { get; set; }
        public CharacterType type { get; set; } = CharacterType.Player;

        public Player()
        {
        }
        public Player(string name)
        {
            level = 1;
            Name = name;
            Health = 100;
            Attack = 10;
            defense = 0;
            Gold = 100;
            IsDead = false;
            inven = new Inventory();
            Text.TextingLine($"환영합니다. {Name} 님");
            Thread.Sleep(1000);
        }
        public void TakeDamage(int damage)
        {
            Text.TextingLine($"{Name}이 {damage}피해를 받았습니다!");
        }

        public void AddInven(IItem item)
        {
            if (inven != null)
            {
                inven.AddItem(item);
            }
        }
        public void ShowStat()
        {
            (int left, int top) point;
            Text.SaveStartPos();
            Text.TextingLine("-------------------------------------------------------\n", ConsoleColor.Red, false);
            Text.TextingLine($"Lv. {level}", ConsoleColor.Green);
            Text.TextingLine($"이름. {Name}", ConsoleColor.Green);
            Text.TextingLine($"체력. {Health}", ConsoleColor.Green);
            Text.TextingLine($"공격력. {Attack}", ConsoleColor.Red);
            Text.TextingLine($"방어력. {defense}", ConsoleColor.Magenta);
            Text.TextingLine($"돈. {Gold}", ConsoleColor.Yellow);
            Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Red, false);
            Text.SaveEndPos();
            Text.Texting("\n\n");
        }
    }

    public class Inventory
    {
        public List<IItem> items;

        public Inventory()
        {
            items = new List<IItem>();
        }
        public void ShowInventory()
        {
            (int left, int top) point;
            Text.SaveStartPos();
            Text.TextingLine("플레이어 인벤토리");
            Text.TextingLine("-------------------------------------------------------\n", ConsoleColor.Red, false);
            for (int i = 0; i < items.Count; i++)
            {
                Text.TextingLine($"{i + 1} . {items[i].Name} : {items[i].description()}",ConsoleColor.Green);
            }
            Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Red, false);
            Text.TextingLine("아이템 사용은 해당 번호를, 나가시려면 0을 입력해주세요.",ConsoleColor.Green);
            Console.WriteLine(Number.Make(0, items.Count));
            Text.GetInput(null,Number.Make(0, items.Count));
            Text.SaveEndPos();
        }

        public void AddItem(IItem item)
        {
            items.Add(item);
        }
        public void UseItem(int num)
        {

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            GameStart();
        }

        static void GameStart()
        {
            CurSceneType curSceneType = CurSceneType.Lobby;
            Player player = new Player();
            ItemDataBase IDB = new ItemDataBase();
            bool gameexit = false;
            //게임 종료 활성화 시 탈출
            while (gameexit == false)
            {
                //로비(시작 시)
                if (curSceneType == CurSceneType.Lobby)
                {
                    Console.Clear();
                    string name = Text.GetInput("이름을 정해주세요.");
                    player = new Player(name);
                    player.AddInven(IDB.returnItem(0));
                    player.AddInven(IDB.returnItem(1));
                    Text.TextingLine("마을로 입장하겠습니다. 조금만 기다려주세요");
                    for (int i = 0; i < 5; i++)
                    {
                        Console.Write(5 - i + "   ");
                        Thread.Sleep(1000);
                    }
                    //씬 이동 역할
                    curSceneType = CurSceneType.Home;
                }
                //게임 시작 후 마을로
                else if (curSceneType == CurSceneType.Home)
                {
                    Console.Clear();
                    while (true)
                    {
                        Text.TextingLine("마을에 오신 것을 환영합니다.");
                        Text.TextingLine("원하시는 활동을 선택해주세요.\n");
                        Text.TextingLine("1. 상태 보기", ConsoleColor.Green);
                        Text.TextingLine("2. 인벤토리", ConsoleColor.Green);
                        Text.TextingLine("3. 상점", ConsoleColor.Green);
                        Text.TextingLine("4. 던전으로", ConsoleColor.Green);
                        string input = Text.GetInput(null, 1, 2, 3, 4);
                        switch (input)
                        {
                            case "1":
                                Console.Clear();
                                Text.TextingLine("플레이어의 현재 스탯입니다.",ConsoleColor.Green);
                                player.ShowStat();
                                input = Text.GetInput("마을로 돌아가시려면 0을 입력해주세요.", 0);
                                Console.Clear();
                                break;
                            case "2":
                                Console.Clear();
                                player.inven.ShowInventory();
                                Console.Clear();
                                break;
                            case "3":
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    //아이템 데이터 베이스 관리
    class ItemDataBase
    {
        public List<Equipment> equipDataBase;

        public ItemDataBase()
        {
            equipDataBase = new List<Equipment>();
            PlusDataBase(new Weapon("부러진 직검", 100, 5, 0, EquipmentType.weapon));
            PlusDataBase(new Armour("찌그러진 방패", 100, 0, 3, EquipmentType.armour));
        }

        public void PlusDataBase(Equipment equip)
        {
            equipDataBase.Add(equip);
        }

        public IItem returnItem(int i)
        {
            if(i < equipDataBase.Count)
            {
                return equipDataBase[i];
            }
            return null;
        }
    }
}
