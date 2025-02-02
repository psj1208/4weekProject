using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalData;

namespace _4weekProject
{
    internal class ItemControl
    {
    }

    public interface IItem
    {
        string Name { get; set; }
        ItemType type { get; set; }
        void Use(Player player);

        string description();
    }

    public class Consumable : IItem
    {
        public string Name { get; set; }
        public double buyPrice { get; set; }
        public double sellPrice { get; set; }
        public ItemType type { get; set; }

        public string description_ { get; set; }
        public void Use(Player player)
        {
            Text.TextingLine($"{Name} 사용 !", ConsoleColor.Red);
        }

        public string description()
        {
            return description_;
        }
    }
    public class HealthPotion : Consumable
    {
        public HealthPotion()
        {
            Name = "힐링포션";
            type = ItemType.NonConsumable;
            buyPrice = 20;
            sellPrice = Math.Round(buyPrice * sellingratio);
            description_ = "체력을 10 회복시켜줍니다.";
        }
    }

    public class StrengthPotion : Consumable
    {
        public StrengthPotion()
        {
            Name = "힘포션";
            type = ItemType.NonConsumable;
            buyPrice = 20;
            sellPrice = Math.Round(buyPrice * sellingratio);
            description_ = "한번의 대전동안 공격력을 5 상승시켜줍니다.";
        }
    }
    public class Equipment : IItem
    {
        //이름,구매가격,판매가격,공격력,방어력
        public string Name { get; set; }
        public double buyPrice;
        public double sellPrice;
        public int attackBonus;
        public int defenseBonus;
        public ItemType type { get; set; }
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

        public void Use(Player player)
        {
            player.Attack += attackBonus;
            player.defense += defenseBonus;
        }

        public string description()
        {
            return $"구매 가격: {buyPrice} , 판매 가격: {sellPrice} , 공격력: {attackBonus} , 방어력: {defenseBonus}";
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
