using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GlobalData;

namespace _4weekProject
{
    public interface ICharacter
    {
        string Name { get; set; }
        int Health { get; set; }
        int Attack { get; set; }
        bool IsDead { get; set; }
        CharacterType type { get; set; }
        void TakeDamage(int damage);

        bool Dead();

        void AttackEnemy(ICharacter character);
    }

    public class Monster : ICharacter
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public bool IsDead { get; set; }
        public int Exp;
        public CharacterType type { get; set; } = CharacterType.Monster;

        public Monster(string name, int hp, int atk, int exp)
        {
            Name = name;
            Health = hp;
            Attack = atk;
            IsDead = false;
            Exp = exp;
        }
        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Dead())
            {
                IsDead = true;
                Text.TextingLine($"{Name}이 사망했습니다.", ConsoleColor.Red, false);
            }
            else
                Text.TextingLine($"{Name}이 {damage}피해를 받았습니다!", ConsoleColor.Magenta, false);
        }

        public bool Dead()
        {
            return Health < 0 ? true : false;
        }

        public void AttackEnemy(ICharacter cha)
        {
            Text.TextingLine($"{Name}이 {cha.Name}을 공격했습니다!", ConsoleColor.Red, false);
            cha.TakeDamage(Attack);
            Console.WriteLine();
        }

        public Monster GetCopy()
        {
            return new Monster(this.Name, this.Health, this.Attack, this.Exp);
        }
    }
    //Warrior 클래스를 대체합니다.
    public class Player : ICharacter
    {
        public int level;
        public string Name { get; set; }

        public int maxHealth { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int defense { get; set; }
        public int Gold { get; set; }

        private int exp;

        private IItem curWeapon;
        private IItem curArmour;

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
            maxHealth = 100;
            Health = 100;
            Attack = 10;
            defense = 0;
            Gold = 50;
            IsDead = false;
            inven = new Inventory(this);
            Text.TextingLine($"환영합니다. {Name} 님");
            Thread.Sleep(1000);
        }
        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Dead())
            {
                Text.TextingLine($"{Name}이 사망했습니다.", ConsoleColor.Red, false);
                IsDead = true;
            }
            else
                Text.TextingLine($"{Name}이 {damage}피해를 받았습니다!", ConsoleColor.Magenta, false);
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
            Text.TextingLine($"경험치. {exp}", ConsoleColor.Blue);
            Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Red, false);
            Text.SaveEndPos();
            Text.Texting("\n\n");
        }

        public bool Dead()
        {
            return Health <= 0 ? true : false;
        }

        public void AttackEnemy(ICharacter cha)
        {
            Text.TextingLine($"{Name}이 {cha.Name}을 공격했습니다!", ConsoleColor.Red, false);
            cha.TakeDamage(Attack);
            Console.WriteLine();
        }
        public void Getexp(int num)
        {
            exp += num;
            Text.TextingLine($"{num} 경험치를 흭득했습니다! ");
            if (exp >= 100 * level)
            {
                level++;
                maxHealth = HealthPerLevel;
                Attack += AtkPerLevel;
                Health = maxHealth;
                exp = exp - 100 * level;
                Text.TextingLine("레벨업!", ConsoleColor.Yellow);
            }
        }

        public bool Equip(IItem item)
        {
            if (item is Weapon)
            {
                if (curWeapon == null)
                {
                    curWeapon = item;
                    return true;
                }
                else
                {
                    inven.AddItem(curWeapon);
                    curWeapon.UnUse(this);
                    curWeapon = item;
                    return true;
                }
            }
            else if (item is Armour)
            {
                if (curArmour == null)
                {
                    curArmour = item;
                    return true;
                }
                else
                {
                    inven.AddItem(curArmour);
                    curArmour.UnUse(this);
                    curArmour = item;
                    return true;
                }
            }
            return false;
        }
    }

    public class Inventory
    {
        public List<IItem> items;
        public Player player;

        public Inventory(Player p)
        {
            items = new List<IItem>();
            player = p;
        }
        public void ShowInventory()
        {
            while (true)
            {
                (int left, int top) point;
                Text.SaveStartPos();
                Text.TextingLine("플레이어 인벤토리");
                Text.TextingLine("-------------------------------------------------------\n", ConsoleColor.Red, false);
                for (int i = 0; i < items.Count; i++)
                {
                    Text.TextingLine($"{i + 1} . {items[i].description()}", ConsoleColor.Green, false);
                }
                Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Red, false);
                Text.TextingLine("아이템 사용은 해당 번호를, 나가시려면 0을 입력해주세요.", ConsoleColor.Green);
                int input = Text.GetInput(null, Number.Make(0, items.Count));
                Text.SaveEndPos();
                if (input == 0)
                    break;
                else
                {
                    UseItem(input);
                    Thread.Sleep(1000);
                    Console.Clear();
                }
            }
        }

        int SearchItem(string name)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (name == items[i].Name)
                    return i;
            }
            return -1;
        }
        public void AddItem(IItem item)
        {
            if (item is Consumable)
            {
                int find = SearchItem(item.Name);
                if (find == -1)
                {
                    items.Add(item);
                }
                else
                {
                    items[find].amt++;
                }
            }
            else
            {
                items.Add(item);
            }

        }
        public void UseItem(int num)
        {
            num--;
            if (items[num] is Consumable)
            {
                items[num].Use(player);
                if (items[num].amt <= 0)
                {
                    items.Remove(items[num]);
                }
            }
            else
            {
                if (items[num].Use(player))
                {
                    items.Remove(items[num]);
                }
            }
        }
    }
}
