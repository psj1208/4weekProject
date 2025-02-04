﻿using System.ComponentModel.Design;
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
    public enum SceneType
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
    
    class Program
    {
        static void Main(string[] args)
        {
            GameStart();
        }

        static void GameStart()
        {
            SceneType curSceneType = SceneType.Lobby;
            Player player = new Player();
            Stage stage = new Stage();
            Shop shop = new Shop();
            bool gameexit = false;
            int Curstage = 0;
            //게임 종료 활성화 시 탈출
            while (gameexit == false)
            {
                //로비(시작 시)
                if (curSceneType == SceneType.Lobby)
                {
                    Console.Clear();
                    string name = Text.GetInput("이름을 정해주세요.");
                    player = new Player(name);
                    shop = new Shop(player);
                    //디버깅 코드
                    player.inven.AddItem(ItemDataBase.ShopDataBase[0][0].DeepCopy());
                    player.inven.AddItem(ItemDataBase.ShopDataBase[0][1].DeepCopy());
                    player.inven.AddItem(new HealthPotion().DeepCopy());
                    player.inven.AddItem(new HealthPotion().DeepCopy());
                    player.inven.AddItem(new HealthPotion().DeepCopy());
                    Text.TextingLine("마을로 입장하겠습니다. 조금만 기다려주세요");
                    Counting(3);
                    //씬 이동 역할
                    curSceneType = SceneType.Home;
                }
                //게임 시작 후 마을로
                else if (curSceneType == SceneType.Home)
                {
                    Console.Clear();
                    while (true)
                    {
                        Text.TextingLine("마을에 오신 것을 환영합니다.");
                        Text.TextingLine("원하시는 활동을 선택해주세요.\n");
                        Text.TextingLine("1 . 상태 보기\n", ConsoleColor.Green, false);
                        Text.TextingLine("2 . 인벤토리\n", ConsoleColor.Green, false);
                        Text.TextingLine("3 . 상점\n", ConsoleColor.Green, false);
                        Text.TextingLine("4 . 던전으로\n", ConsoleColor.Green, false);
                        int input = Text.GetInput(null, 1, 2, 3, 4);
                        switch (input)
                        {
                            case 1:
                                Console.Clear();
                                Text.TextingLine("플레이어의 현재 스탯입니다.", ConsoleColor.Green);
                                Thread.Sleep(Waitingtime);
                                player.ShowStat();
                                input = Text.GetInput("마을로 돌아가시려면 0을 입력해주세요.", 0);
                                Console.Clear();
                                break;
                            case 2:
                                Console.Clear();
                                player.inven.ShowInventory();
                                Console.Clear();
                                break;
                            case 3:
                                Console.Clear();
                                shop.ShowShop();
                                Console.Clear();
                                break;
                            case 4:
                                Console.Clear();
                                Text.TextingLine(" 스테이지 목록 !");
                                Thread.Sleep(Waitingtime);
                                Text.TextingLine("-------------------------------------------------------\n", ConsoleColor.Magenta, false);
                                for (int i = 0; i < StageDB.stageList.Count; i++)
                                {
                                    Text.Texting($"스테이지 {i + 1}. 출현 몬스터 : ", ConsoleColor.Green, false);
                                    for(int a = 0; a < StageDB.stageList[i].monsters.Count; a++)
                                    {
                                        Text.Texting(StageDB.stageList[i].monsters[a].monster.Name + " , ", ConsoleColor.Red, false);
                                    }
                                    Console.WriteLine("\n");
                                }
                                Text.TextingLine("\n-------------------------------------------------------", ConsoleColor.Magenta, false);
                                input = Text.GetInput("해당 스테이지 번호를 입력해주세요", Number.Make(1,StageDB.stageList.Count));
                                stage = StageDB.stageList[input-1];
                                Text.TextingLine($"스테이지 {input}으로 이동합니다.");
                                Curstage = input - 1;
                                Counting(3);
                                curSceneType = SceneType.Explore;
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
                else if (curSceneType == SceneType.Explore)
                {
                    Console.Clear();
                    Random rand = new Random();
                    bool isPlayerturn = true;
                    int clear = 0; 
                    for (int i = 0; i < stage.length; i++)
                    {
                        if (player.IsDead == true)
                            break;
                        Monster monster = stage.MakeMonster().GetCopy();
                        Text.Texting("탐험 중 ", ConsoleColor.Blue, false);
                        for (int a = 0; a < 5; a++)
                        {
                            Text.Texting(" . ", ConsoleColor.Blue, false);
                            Thread.Sleep(500);
                        }
                        Console.WriteLine();
                        Text.TextingLine($"당신은 {monster.Name}을 만났다!" , ConsoleColor.Magenta);
                        Text.SaveStartPos();
                        Text.TextingLine("0 . 싸운다.", ConsoleColor.Green);
                        Text.TextingLine("1 . 도망간다.", ConsoleColor.Green);
                        Text.SaveEndPos();
                        int input = Text.GetInput(null, 0, 1);
                        if(input == 0)
                        {
                            Text.Texting(" 전 ", ConsoleColor.Red);
                            Thread.Sleep(500);
                            Text.Texting(" 투 ", ConsoleColor.Red);
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Text.Texting(" 귀 ", ConsoleColor.Green);
                            Thread.Sleep(500);
                            Text.Texting(" 환 ", ConsoleColor.Green);
                            Thread.Sleep(1000);
                        }
                        Console.Clear();
                        if (input == 0)
                        {
                            while (true)
                            {
                                if (player.IsDead == true || monster.IsDead == true)
                                {
                                    break;
                                }
                                if (isPlayerturn == true)
                                {
                                    player.AttackEnemy(monster);
                                    isPlayerturn = !isPlayerturn;
                                    Thread.Sleep(1000);
                                }
                                else if (isPlayerturn == false)
                                {
                                    monster.AttackEnemy(player);
                                    isPlayerturn = !isPlayerturn;
                                    Thread.Sleep(1000);
                                }
                            }
                            if (player.IsDead == true)
                            {
                                Text.TextingLine("패배하였습니다.", ConsoleColor.Red);
                                Text.GetInput("다음 (0 입력)", 0);
                                gameexit = true;
                            }
                            else
                            {
                                Text.TextingLine("승리하였습니다.", ConsoleColor.Green);
                                clear++;
                                player.Getexp(monster.Exp);
                                player.Gold += monster.gold;
                                Text.TextingLine($"{monster.gold} 골드를 흭득하셨습니다.", ConsoleColor.Yellow);
                                Text.TextingLine($"{player.Name} 의 남은 체력 : {player.Health}");
                                if (clear == stage.length)
                                {
                                    Text.GetInput(" 0. 다음 ", 0);
                                    Console.Clear();
                                    Text.TextingLine($"{Curstage} stage 클리어!", ConsoleColor.Yellow);
                                    Console.WriteLine("\n");
                                    Text.TextingLine(" 보 상 ", ConsoleColor.Yellow);
                                    Console.WriteLine("\n");
                                    StageClear.ClearMethod[Curstage](player);
                                }
                                Text.TextingLine(" 0 . 다음 ", ConsoleColor.Green);
                                Text.TextingLine(" 1 . 인벤토리 열기 ", ConsoleColor.Green);
                                input = Text.GetInput(null, 0, 1);
                                if (input == 1) 
                                {
                                    Console.Clear();
                                    player.inven.ShowInventory();
                                    input = Text.GetInput(" 0. 계속 진행하기 \n 1 . 마을로 돌아가기 ", 0, 1);
                                }
                            }
                            Console.Clear();
                            if (input == 1)
                                break;
                        }
                        else 
                            break;
                    }
                    curSceneType = SceneType.Home;
                }
            }
            Console.Clear();
            Console.WriteLine("게임 끝!");
            player.ShowStat();
        }
        static  void Counting(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Console.Write(num - i + "   ");
                Thread.Sleep(1000);
            }
        }
    }
}
