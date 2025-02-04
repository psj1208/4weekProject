using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4weekProject
{
    public class ItemDataBase
    {
        static public List<IItem> DataBase;
        static public List<List<IItem>> ShopDataBase;
        static public List<IItem> WeaponList;
        static public List<IItem> ArmourList;
        static public List<IItem> ConsumeList;
        //무기류
        static IItem weapon_1 = new Weapon("부러진 직검", 100, 5, 0, EquipmentType.weapon);
        static IItem weapon_2 = new Weapon("직검", 200, 10, 0, EquipmentType.weapon);
        static IItem weapon_3 = new Weapon("장검", 300, 15, 0, EquipmentType.weapon);
        //방어구류
        static IItem armour_1 = new Armour("찌그러진 방패", 100, 0, 3, EquipmentType.armour);
        static IItem armour_2 = new Armour("낡은 방패", 200, 0, 5, EquipmentType.armour);
        static IItem armour_3 = new Armour("제대로 된 방패", 300, 0, 8, EquipmentType.armour);
        //소모품류
        static IItem healthpotion = new HealthPotion();
        static IItem strengthpotion = new StrengthPotion();

        static ItemDataBase()
        {
            DataBase = new List<IItem>();
            ShopDataBase = new List<List<IItem>>();
            WeaponList = new List<IItem>();
            ArmourList = new List<IItem>();
            ConsumeList = new List<IItem>();
            CreateShopBase();
        }

        static void CreateShopBase()
        {
            WeaponList.Add(weapon_1);
            WeaponList.Add(weapon_2);
            WeaponList.Add(weapon_3);
            ArmourList.Add(armour_1);
            ArmourList.Add(armour_2);
            ArmourList.Add(armour_3);
            ConsumeList.Add(healthpotion);
            ConsumeList.Add(strengthpotion);
            ShopDataBase.Add(WeaponList);
            ShopDataBase.Add(ArmourList);
            ShopDataBase.Add(ConsumeList);
        }
    }
}
