using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static GlobalData;

namespace _4weekProject
{
    public interface IItem
    {
        string Name { get; set; }
        ItemType type { get; set; }
        double buyPrice { get; set; }
        double sellPrice { get; set; }
        int amt { get; set; }
        bool Use(Player player);
        void UnUse(Player player);
        string description();
    }

    public class Consumable : IItem
    {
        public string Name { get; set; }
        public double buyPrice { get; set; }
        public double sellPrice { get; set; }
        public int amt { get; set; }
        public ItemType type { get; set; }

        public string description_ { get; set; }
        public virtual bool Use(Player player)
        {
            return false;
        }

        public void UnUse(Player player)
        {
        }
        public string description()
        {
            return $"{Name} : {description_} , 수량 : {amt}";
        }
    }
    public class HealthPotion : Consumable
    {
        public HealthPotion(int num = 1)
        {
            Name = "힐링포션";
            type = ItemType.NonConsumable;
            amt = num;
            buyPrice = 20;
            sellPrice = Math.Round(buyPrice * sellingratio);
            description_ = "체력 20 회복";
        }

        public override bool Use(Player player)
        {
            Text.TextingLine($"{Name} 사용 ! , {description_}", ConsoleColor.Red);
            amt--;
            player.Health += 20;
            return true;
        }
    }

    public class StrengthPotion : Consumable
    {
        public StrengthPotion(int num = 1)
        {
            Name = "힘영약";
            type = ItemType.NonConsumable;
            amt = num;
            buyPrice = 300;
            sellPrice = Math.Round(buyPrice * sellingratio);
            description_ = "영구적으로 공격력을 1 상승";
        }
        public override bool Use(Player player)
        {
            Text.TextingLine($"{Name} 사용 ! , {description_}", ConsoleColor.Red);
            amt--;
            player.Attack += 1;
            return true;
        }
    }
    public class Equipment : IItem
    {
        //이름,구매가격,판매가격,공격력,방어력
        public string Name { get; set; }
        public double buyPrice { get; set; }
        public double sellPrice { get; set; }
        public int attackBonus;
        public int defenseBonus;
        public int amt { get; set; }
        public ItemType type { get; set; }
        public void Plusamt(int i) { }
        public EquipmentType equipType;

        public Equipment(string name, double buyPrice, int attackBonus, int defenseBonus, EquipmentType equipType)
        {
            Name = name;
            this.buyPrice = buyPrice;
            this.sellPrice = Math.Round(buyPrice * sellingratio);
            this.attackBonus = attackBonus;
            this.defenseBonus = defenseBonus;
            type = ItemType.NonConsumable;
            this.equipType = equipType;
        }

        public bool Use(Player player)
        {
            if (player.Equip(this))
            {
                player.Attack += attackBonus;
                player.defense += defenseBonus;
                Text.TextingLine($"{this.Name} 장착 ! ");
                return true;
            }
            else
            {
                Text.TextingLine("장착에 실패했습니다.", ConsoleColor.Green);
                return false;
            }
        }

        public void UnUse(Player player)
        {
            player.Attack -= attackBonus;
            player.defense -= defenseBonus;
        }

        public string description()
        {
            return $"{Name} : 구매 가격: {buyPrice} , 판매 가격: {sellPrice} , 공격력: {attackBonus} , 방어력: {defenseBonus}";
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
}
