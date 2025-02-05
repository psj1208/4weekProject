using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static GlobalData;

namespace _4weekProject
{
    //아이템 클래스 관련
    public interface IItem_base
    {
        string Name { get; set; }
        ItemType type { get; set; }
        double buyPrice { get; set; }
        double sellPrice { get; set; }
        int amt { get; set; }
        //사용 시 or 장착 시
        bool Use(Player player);
        //장착 해제 시
        void UnUse(Player player);
        //아이템 설명 출력용.
        string description();

        //상점에 있는 아이템을 매개변수로 주게 되면 클래스는 주소값이 넘어가기 때문에 카피본이 필요함.
    }

    //virtual-virtual-override 순서대로 상속받고 2번째 클래스에서 호출하면 2번째  함수가 호출된다.
    //이는 2번째 virtual에서 마무리를 위해 override가 먼저 선언되고 virtual 형태로 선언되기 때문.
    //풀어 설명하면 virtual-(overide,virtual)-overide 형태로 선언된다.
    //virtual-override-override 형식을 선언하면 형태에 맞게(올바르게) 메서드가 호출됨.
    public class IItem : IItem_base
    {
        public virtual string Name { get; set; }
        public virtual ItemType type { get; set; }
        public virtual double buyPrice { get; set; }
        public virtual double sellPrice { get; set; }
        public virtual int amt { get; set; }

        //사용 시 or 장착 시
        public virtual bool Use(Player player) { return true; ; }
        //장착 해제 시
        public virtual void UnUse(Player player) { }
        //아이템 설명 출력용.(상점이나 인벤토리에 아이템의 설명을 보이기 위함)
        public virtual string description() { return null; }

        //상점에 있는 아이템을 매개변수로 주게 되면 클래스는 주소값이 넘어가기 때문에 카피본이 필요함.
        public virtual IItem DeepCopy() { return null; }
    }

    //소모품
    public class Consumable : IItem
    {
        public override string Name { get; set; }
        public override double buyPrice { get; set; }
        public override double sellPrice { get; set; }
        public override int amt { get; set; }
        public override ItemType type { get; set; }

        public string description_ { get; set; }
        public override bool Use(Player player)
        {
            return false;
        }

        //소모품은 이거 사용안함.
        public override void UnUse(Player player)
        {
        }
        public override string description()
        {
            return $"{Name} : {description_} , 구매 가격 : {buyPrice} , 판매 가격 : {sellPrice} , 수량 : {amt}";
        }
    }
    //원래 따로 클래스로 안 만들 생각이였는데, 주어진 베이스가 이런 식이라 그대로 함.
    public class HealthPotion : Consumable
    {
        public HealthPotion(int num = 1)
        {
            Name = "힐링포션";
            type = ItemType.Consumable;
            amt = num;
            buyPrice = 20;
            sellPrice = Math.Round(buyPrice * sellingratio);
            description_ = "체력 20 회복";
        }

        //체력포션 사용 효과
        public override bool Use(Player player)
        {
            Text.TextingLine($"{Name} 사용 ! , {description_}", ConsoleColor.Red);
            amt--;
            player.Health += 20;
            if (player.Health > player.maxHealth)
                player.Health = player.maxHealth;
            return true;
        }

        //클래스 호출 시. 복사하기 위함. 미리 설명해놨으니 이제부턴 생략.
        public override IItem DeepCopy()
        {
            return new HealthPotion();
        }
    }
    //원래 따로 클래스로 안 만들 생각이였는데, 주어진 베이스가 이런 식이라 그대로 함.
    public class StrengthPotion : Consumable
    {
        public StrengthPotion(int num = 1)
        {
            Name = "힘영약";
            type = ItemType.Consumable;
            amt = num;
            buyPrice = 300;
            sellPrice = Math.Round(buyPrice * sellingratio);
            description_ = "영구적으로 공격력을 1 상승";
        }
        //사용 효과
        public override bool Use(Player player)
        {
            Text.TextingLine($"{Name} 사용 ! , {description_}", ConsoleColor.Red);
            amt--;
            player.Attack += 1;
            return true;
        }

        public override IItem DeepCopy()
        {
            return new StrengthPotion();
        }
    }
    public class Equipment : IItem
    {
        //이름,구매가격,판매가격,공격력,방어력
        public override string Name { get; set; }
        public override double buyPrice { get; set; }
        public override double sellPrice { get; set; }
        public int attackBonus;
        public int defenseBonus;
        public override int amt { get; set; }
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

        //장착(Equip)함수에서 호출하여 장착이 될 시 무기와 방어구의 스탯만큼 플레이어 스탯 증가.
        public override bool Use(Player player)
        {
            //장착 실패시 false를 반환하는 메서드다. 위에 있음.
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

        //장착 해제 시
        public override void UnUse(Player player)
        {
            player.Attack -= attackBonus;
            player.defense -= defenseBonus;
        }

        //출력용
        public override string description()
        {
            return $"{Name} : 구매 가격: {buyPrice} , 판매 가격: {sellPrice} , 공격력: {attackBonus} , 방어력: {defenseBonus}";
        }

        //하위 클래스마다 다르게 설정하기에 재정의 가능하게 코드 기술.
        public override IItem DeepCopy()
        {
            return null;
        }
    }
    //무기
    public class Weapon : Equipment
    {
        //부모(Equipment) 생성자 호출
        public Weapon(string name, double buy, int atk, int def, EquipmentType equipType) : base(name, buy, atk, def, equipType)
        {
        }

        public override IItem DeepCopy()
        {
            return new Weapon(this.Name, this.buyPrice, this.attackBonus, this.defenseBonus, this.equipType);
        }
    }
    //방어구
    public class Armour : Equipment
    {
        //부모(Equipment) 생성자 호출
        public Armour(string name, double buy, int atk, int def, EquipmentType equipType) : base(name, buy, atk, def, equipType)
        {
        }

        public override IItem DeepCopy()
        {
            return new Armour(this.Name, this.buyPrice, this.attackBonus, this.defenseBonus, this.equipType);
        }
    }
}
