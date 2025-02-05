using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalData;

namespace _4weekProject
{
    //상점 관련
    public class Shop
    {
        Player player;

        public Shop() { }

        public Shop(Player p)
        {
            player = p;
        }

        public void ShowShop()
        {
            while (true)
            {
                //길이 측정 변수 선언
                int totallength = 0;
                //아이템 종류(무기,방어구,소모품)
                int count = ItemDataBase.ShopDataBase.Count;
                Text.SaveStartPos();
                Text.TextingLine(" 상점 !");
                Thread.Sleep(Waitingtime);
                Text.TextingLine("-------------------------------------------------------\n", ConsoleColor.Red, false);
                for (int i = 0; i < ItemDataBase.ShopDataBase.Count; i++)
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
                    for (int a = 0; a < ItemDataBase.ShopDataBase[i].Count; a++)
                    {
                        Text.TextingLine($"{count * i + a + 1} . {ItemDataBase.ShopDataBase[i][a].description()}", ConsoleColor.Green, false);
                        //길이 측정용
                        totallength++;
                    }
                }
                Text.TextingLine(" 서 비 스  ", ConsoleColor.Blue, false);
                Text.TextingLine($"{totallength + 1} . 휴식 (50원) : 체력을 50% 회복시켜준다.", ConsoleColor.Green, false);
                Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Red, false);
                Text.TextingLine("아이템 구입은 해당 번호를, 나가시려면 0을 입력해주세요.", ConsoleColor.Green);
                int input = Text.GetInput(null, Number.Make(0, totallength + 1));
                //아이템의 번호는 1부터 시작해서 인덱스보다 1 높으니 1을 빼준다.
                input--;
                Text.SaveEndPos();
                //0을 입력하게되면 -- 연산때문에 -1이 되기 때문.
                if (input == -1)
                    break;
                //휴식 구매 시(totallength는 결국 휴식의 번호와 같다)
                else if (input == totallength) 
                {
                    if (player.Gold >= 50)
                    {
                        Text.TextingLine("휴식하셨습니다. 체력 50% 회복. ", ConsoleColor.Green);
                        //최대 체력의 절반을 회복
                        player.Health += player.maxHealth / 2;
                        //최대 체력 초과 시 돌려놓기
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
                //휴식 제외 다른 물품 구매
                else
                {
                    IItem selectitem = ItemDataBase.ShopDataBase[input / count][input % count].DeepCopy();
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
        //판매 메서드.
        public void ShowSelling()
        {
            while (true)
            {
                player.inven.InvenInfo();
                int input = Text.GetInput("나가시려면 0을 입력 \n아니면 판매할 아이템의 번호를 입력해주세요.", Number.Make(0, player.inven.ReturnLength()));
                input--;
                if (input != -1)
                {
                    player.inven.SellItem(input);
                    Thread.Sleep(Waitingtime);
                    Console.Clear();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
