using System.ComponentModel.Design;
using System.Reflection.Metadata;

namespace _4weekProject
{
    public enum CurSceneType
    {
        Lobby,
        Home,
        Store
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
        public string Name { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public bool IsDead { get; set; }
        public CharacterType type { get; set; } = CharacterType.Player;

        public Player(string name)
        {
            Name = name;
            Health = 100;
            Attack = 10;
            IsDead = false;
            Text.TextingLine($"환영합니다. {Name} 님");
            Thread.Sleep(2000);
        }
        public void TakeDamage(int damage)
        {
            Text.TextingLine($"{Name}이 {damage}피해를 받았습니다!");
        }
    }

    public interface IItem
    {
        string Name { get; set; }
        void Use(Player player);
    }

    public class HealthPotion : IItem
    {
        public string Name { get; set; }
        public void Use(Player player)
        {
        }
    }

    public class StrengthPotion
    {
        public string Name { get; set; }
        public void Use(Player player)
        {
        }
    }
    public class Equipment
    {
        //이름,구매가격,판매가격,공격력,방어력
        public string Name;
        public double buyPrice;
        public double sellPrice;
        public int attackBonus;
        public int defenseBonus;
        public EquipmentType equipType;

        public Equipment(string name, double buyPrice, int attackBonus, int defenseBonus, EquipmentType equipType)
        {
            Name = name;
            this.buyPrice = buyPrice;
            this.sellPrice = Math.Round(buyPrice * 0.7f);
            this.attackBonus = attackBonus;
            this.defenseBonus = defenseBonus;
            this.equipType = equipType;
        }
    }
    public class Weapon : Equipment
    {
        //부모(Equipment) 생성자 호출
        public Weapon(string name, int buy, int atk, int def, EquipmentType equipType) : base(name, buy, atk, def, equipType)
        {
        }
    }

    public class Armour : Equipment
    {
        //부모(Equipment) 생성자 호출
        public Armour(string name, int buy, int atk, int def, EquipmentType equipType) : base(name, buy, atk, def, equipType)
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
            Player player;
            bool gameexit = false;
            while (gameexit == false)
            {
                if (curSceneType == CurSceneType.Lobby)
                {
                    while (true)
                    {
                        Text.TextingLine("이름을 정해주세요.");
                        String? name = Console.ReadLine();
                        if (!String.IsNullOrEmpty(name))
                        {
                            player = new Player(name);
                            break;
                        }
                    }
                    Text.TextingLine("마을로 입장하겠습니다. 조금만 기다려주세요");
                    for (int i = 0; i < 5; i++)
                    {
                        Console.Write(5 - i + "   ");
                        Thread.Sleep(1000);
                    }
                    curSceneType = CurSceneType.Home;
                }
                else if (curSceneType == CurSceneType.Home)
                {
                    while (true)
                    {

                    }
                }
            }
        }
    }

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
    }

    //텍스트 효과용 Static 클래스
    static class Text
    {
        public static void TextingLine(string text)
        {
            foreach (char ch in text)
            {
                Console.Write(ch);
                Thread.Sleep(20);
            }
            Console.WriteLine();

        }

        public static void Texting(string text)
        {
            foreach (char ch in text)
            {
                Console.Write(ch);
                Thread.Sleep(20);
            }
        }
    }
}
