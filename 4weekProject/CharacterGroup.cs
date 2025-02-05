using Newtonsoft.Json;
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
    //캐릭터 관련(몬스터,플레이어,인벤토리)
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

        RandomBetween Attack_;//공격력을 랜덤한 값으로 돌려주기 위함. 같은 형식으로 플레이어에게도 적용은 할 수 있다.
        public bool IsDead { get; set; }
        public int Exp;
        RandomBetween goldrandom;//드롭하는 돈의 양을 랜덤으로 정하기 위함.
        public int gold;
        public CharacterType type { get; set; } = CharacterType.Monster;

        //생성자
        public Monster(string name, int hp, RandomBetween atk, int exp,RandomBetween drop)
        {
            Name = name;
            Health = hp;
            Attack_ = atk;
            IsDead = false;
            Exp = exp;
            goldrandom = drop;
            gold = Randomize.Makenum(goldrandom);
        }
        //데미지 받기.상대방의 Attack메서드에 넣어 코드를 간결화.
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

        //0이하(죽음) 일시 true를 반환 아니면 false를 반환
        public bool Dead()
        {
            return Health <= 0 ? true : false;
        }
        
        //적 공격
        public void AttackEnemy(ICharacter cha)
        {
            Text.TextingLine($"{Name}이 {cha.Name}을 공격했습니다!", ConsoleColor.Red, false);
            Attack = Randomize.Makenum(Attack_);
            //여기서 사용.TakeDamage
            cha.TakeDamage(Attack);
            Console.WriteLine();
        }
        //클래스는 주소값을 전달하기에 복사하여 새로운 객체를 추가하려면 따로 짜야된다.(ex.상점에서 구입하여 인벤에 들어가는 경우)
        public Monster GetCopy()
        {
            return new Monster(this.Name, this.Health, this.Attack_, this.Exp, this.goldrandom);
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

        public int exp;

        //현재 무기
        public Equipment curWeapon;
        //현재 방어구
        public Equipment curArmour;

        //inven이 세이브로드에서 오류를 일으켜 jsonignoret선언을 통해 세이브 방지. 플레이어의 인벤토리이다.
        [JsonIgnore]
        public Inventory inven;
        public bool IsDead { get; set; }
        public CharacterType type { get; set; } = CharacterType.Player;

        //생성자
        public Player()
        {

        }
        public Player(string name, int health = 200, int defense = 0, int attack = 10, int gold = 50)
        {
            level = 1;
            Name = name;
            maxHealth = 100 + HealthPerLevel * level;
            Health = health;
            Attack = attack;
            this.defense = defense;
            Gold = gold;
            //생성자와 동시에 인벤토리 생성 후 플레이어의 inven 변수에 할당.
            inven = new Inventory(this);
            IsDead = false;
            Text.TextingLine($"환영합니다. {Name} 님");
            Thread.Sleep(1000);
        }
        //골드 흭득 및 알람
        public void GainGold(int a)
        {
            Gold += a;
            Text.TextingLine($"{a} 골드를 얻었습니다.", ConsoleColor.Yellow);
        }
        //데미지 피격 및 사망 여부 확인
        public void TakeDamage(int damage)
        {
            damage -= defense;
            if (damage <= 0)
                damage = 0;
            Health -= damage;
            if (Dead())
            {
                Health = 0;
                Text.TextingLine($"{Name}이 사망했습니다.", ConsoleColor.Red, false);
                IsDead = true;
            }
            else
                Text.TextingLine($"{Name}이 {damage}피해를 받았습니다!", ConsoleColor.Magenta, false);
        }

        //아이템 흭득 및 알람
        public void GainItem(IItem item)
        {
            Text.TextingLine($"{item.Name} 을 흭득하였습니다.", ConsoleColor.Green);
            AddInven(item);
        }

        //인벤토리에 아이템 추가
        public void AddInven(IItem item)
        {
            if (inven != null)
            {
                inven.AddItem(item);
            }
        }

        //스탯 출력
        public void ShowStat()
        {
            (int left, int top) point;
            Text.SaveStartPos();
            Text.TextingLine("-------------------------------------------------------\n", ConsoleColor.Red, false);
            Text.TextingLine($"Lv. {level}", ConsoleColor.Green, false);
            Text.TextingLine($"이름. {Name}", ConsoleColor.Green, false);
            Text.TextingLine($"체력. {Health}", ConsoleColor.Green, false);
            Text.TextingLine($"공격력. {Attack}", ConsoleColor.Red, false);
            Text.TextingLine($"방어력. {defense}", ConsoleColor.Magenta, false);
            Text.TextingLine($"돈. {Gold}", ConsoleColor.Yellow, false);
            Text.TextingLine($"경험치. {exp}", ConsoleColor.Blue, false);
            Console.WriteLine();
            string weapon;
            if (curWeapon != null)
                weapon = curWeapon.Name;
            else
                weapon = "없음";
            Text.TextingLine($"현재 무기. {weapon} ");
            Console.WriteLine();
            string armour;
            if (curArmour != null)
                armour = curArmour.Name;
            else
                armour = "없음";
            Text.TextingLine($"현재 무기. {armour} ");
            Console.WriteLine();
            Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Red, false);
            Text.SaveEndPos();
            Text.Texting("\n\n");
        }

        //죽음 판정
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

        //경험치 흭득 및 레벨업 및 알람
        public void Getexp(int num)
        {
            exp += num;
            Text.TextingLine($"{num} 경험치를 흭득했습니다! ", ConsoleColor.Blue);
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

        //아이템 장착 및 알람
        public bool Equip(Equipment item)
        {
            //무기의 경우
            if (item is Weapon)
            {
                //비엇을 시 장착 후 true 반환
                if (curWeapon == null)
                {
                    curWeapon = item;
                    return true;
                }
                //안 비었을 시 현재 장비 장착 해제 후 장착
                else
                {
                    inven.AddItem(curWeapon);
                    curWeapon.UnUse(this);
                    curWeapon = item;
                    return true;
                }
            }
            //방어구의 경우
            else if (item is Armour)
            {
                //위와 같음
                if (curArmour == null)
                {
                    curArmour = item;
                    return true;
                }
                //위와 같음
                else
                {
                    inven.AddItem(curArmour);
                    curArmour.UnUse(this);
                    curArmour = item;
                    return true;
                }
            }
            //장착 실패 시 인데. 안 나올듯.
            return false;
        }
    }

    public class Inventory
    {
        public List<IItem> items;
        [JsonIgnore]
        public Player player;

        //플레이어를 매개변수로 받음으로써 플레이어에게 귀속.
        public Inventory(Player p)
        {
            items = new List<IItem>();
            player = p;
        }

        //외부 클래스에서 호출용. 인벤토리 아이템의 종류의 수
        public int ReturnLength()
        {
            return items.Count;
        }
        //인벤토리 조작
        public void ShowInventory()
        {
            while (true)
            {
                (int left, int top) point;
                Text.SaveStartPos();
                Text.TextingLine("플레이어 인벤토리");
                Thread.Sleep(Waitingtime);
                InvenInfo();
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

        //인벤토리 화면 출력
        public void InvenInfo()
        {
            Text.TextingLine("-------------------------------------------------------\n", ConsoleColor.Red, false);
            for (int i = 0; i < items.Count; i++)
            {
                Text.TextingLine($"{i + 1} . {items[i].description()}", ConsoleColor.Green, false);
            }
            Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Red, false);
        }

        //찾는 아이템의 이름을 대조해서 인벤토리의 아이템의 인덱스를 반환
        int SearchItem(string name)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (name == items[i].Name)
                    return i;
            }
            return -1;
        }
        //아이템 추가. Player의 AddInven에 사용되있음.
        public void AddItem(IItem item)
        {
            //소비품일 시
            if (item.type == ItemType.Consumable)
            {
                int find = SearchItem(item.Name);
                //같은 품목을 찾지 못함
                if (find == -1)
                {
                    items.Add((IItem)item);
                }
                //같은 품목을 찾을 시 수량을 증가
                else
                {
                    items[find].amt++;
                }
            }
            //장비품은 수량이 없으니 그냥 추가.
            else
            {
                items.Add(item);
            }
        }

        //장비 판매.Shop클래스에서 호출해서 사용함.
        public void SellItem(int num)
        {
            Text.TextingLine($"{items[num].Name} 을 판매하셨습니다.");
            player.GainGold((int)items[num].sellPrice);
            //소모품은 수량을 줄이고 0이하가 되면 제거.
            if (items[num] is Consumable) 
            {
                items[num].amt--;
                if (items[num].amt <= 0)
                {
                    items.Remove(items[num]);
                }
            }
            //장비품은 수량이 없으니 그냥 제거.
            else
            {
                items.Remove(items[num]);
            }
        }
        //아이템 사용
        public void UseItem(int num)
        {
            //인벤토리에서 품목은 1부터 시작하기에 1을 빼야함.
            num--;
            //소비품일 시
            if (items[num] is Consumable)
            {
                items[num].Use(player);
                //수량이 0이 되면 인벤토리에서 제거한다.
                if (items[num].amt <= 0)
                {
                    items.Remove(items[num]);
                }
            }
            //장비일 시
            else
            {
                //아이템이 장착되면 인벤토리에서 아이템을 제거한다.
                if (items[num].Use(player))
                {
                    items.Remove(items[num]);
                }
            }
        }
    }
}
