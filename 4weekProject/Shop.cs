using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4weekProject
{
    public class Shop
    {
        Player player;
        ItemDataBase itemData;

        public Shop() { }

        public Shop(Player p, ItemDataBase idb)
        {
            player = p;
            itemData = idb;
        }

        public void ShowShop()
        {
            while (true)
            {
                int totallength = 0;
                int count = itemData.ShopDataBase.Count;
                (int left, int top) point;
                Text.SaveStartPos();
                Text.TextingLine(" 상점 !");
                Text.TextingLine("-------------------------------------------------------\n", ConsoleColor.Red, false);
                for (int i = 0; i < itemData.ShopDataBase.Count; i++)
                {
                    switch(i)
                    {
                        case 0:
                            Text.TextingLine(" 무 기 류 ", ConsoleColor.Blue, false);
                            break;
                        case 1:
                            Text.TextingLine(" 방 어 구 류 ", ConsoleColor.Blue, false);
                            break;
                        case 2:
                            Text.TextingLine(" 소 모 품 류 ", ConsoleColor.Blue, false);
                            break;
                        default:
                            break;

                    };
                    for (int a = 0; a < itemData.ShopDataBase[i].Count; a++)
                    {
                        Text.TextingLine($"{count * i + a + 1} . {itemData.ShopDataBase[i][a].description()}", ConsoleColor.Green, false);
                        totallength++;
                    }
                }
                Text.TextingLine($"{totallength + 1} . 휴식 (50원) : 체력을 50% 회복시켜준다.", ConsoleColor.Green);
                Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Red, false);
                Text.TextingLine("아이템 구입은 해당 번호를, 나가시려면 0을 입력해주세요.", ConsoleColor.Green);
                int input = Text.GetInput(null, Number.Make(0, totallength + 1));
                input--;
                Text.SaveEndPos();
                if (input == -1)
                    break;
                else if (input == totallength) 
                {
                    if (player.Gold >= 50)
                    {
                        Text.TextingLine("휴식하셨습니다. 체력 50% 회복. ", ConsoleColor.Green);
                        player.Health += player.maxHealth / 2;
                        if(player.Health > player.maxHealth)
                            player.Health = player.maxHealth;
                    }
                    else
                    {
                        Text.TextingLine("금액이 부족합니다. ", ConsoleColor.Red);
                    }
                    Thread.Sleep(1000);
                    Console.Clear();
                }
                else
                {
                    IItem selectitem = itemData.ShopDataBase[input / count][input % count];
                    if (player.Gold >= selectitem.buyPrice)
                    {
                        player.Gold -= (int)selectitem.buyPrice;
                        player.inven.AddItem(selectitem);
                        Text.TextingLine($"{selectitem.Name} 을 구매하셨습니다. ", ConsoleColor.Yellow);
                    }
                    else
                    {
                        Text.TextingLine("금액이 부족합니다. ", ConsoleColor.Red);
                    }
                    Thread.Sleep(1000);
                    Console.Clear();
                }
            }
        }
    }
}
