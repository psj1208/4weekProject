using System.ComponentModel.Design;
using System.Drawing;
using System.Net.Security;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using static GlobalData;

namespace _4weekProject
{
    //Text클래스를 활용하기 위한 int형 배열 생성용 static 클래스(스크립트에서 활용할 도구용 메서드들은 static으로 대부분 선언)
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
    //Text클래스를 활용하기 위한 x,y 좌표를 가진 클래스
    public struct Point_
    {
        int x;
        int y;
    }
    //유니티의 Scene의 역할을 하는 열거형.
    public enum SceneType
    {
        Lobby,
        Home,
        Explore,
        Store
    }
    //아이템이 소모품인지 아닌지 구분하기 위함이였는데. 포션들을 따로따로 클래스로 구현해놓아서 필요가 없어짐. 혹시 모르니 남겨둠.
    public enum ItemType
    {
        Consumable,
        NonConsumable
    }
    //장비가 무기인지 방어구인지(중복 착용을 막기 위함)
    public enum EquipmentType
    {
        weapon,
        armour
    }
    //플레이어인지 아닌지 구분
    public enum CharacterType
    {
        Player, //Warrior 타입을 대체합니다.
        Monster
    }
    
    class Program
    {
        public static string path = AppDomain.CurrentDomain.BaseDirectory;
        static SceneType curSceneType = SceneType.Lobby;
        static Player player = new Player();
        static Stage stage = new Stage();
        static Shop shop;
        static void Main(string[] args)
        {
            GameStart();
        }

        static void GameStart()
        {
            //게임 종료 구분 여부
            bool gameexit = false;
            //진행할 스테이지의 단계를 담아둘 변수
            int Curstage = 0;
            //게임 종료 활성화 시 탈출
            while (gameexit == false)
            {
                //로비(시작 시)
                if (curSceneType == SceneType.Lobby)
                {
                    int input_start = Text.GetInput("1 . 새로하기\n2 . 불러오기", 1, 2);
                    Console.Clear();
                    if (input_start == 2)
                    {
                        player = new Player();
                        Load();
                    }
                    else
                    {
                        string name = Text.GetInput("이름을 정해주세요.");
                        player = new Player(name);
                        //디버깅 코드
                        player.inven.AddItem(ItemDataBase.ShopDataBase[0][0].DeepCopy());
                        player.inven.AddItem(ItemDataBase.ShopDataBase[0][1].DeepCopy());
                        player.inven.AddItem(new HealthPotion().DeepCopy());
                        player.inven.AddItem(new HealthPotion().DeepCopy());
                        player.inven.AddItem(new HealthPotion().DeepCopy());
                    }
                    shop = new Shop(player);
                    Text.TextingLine("마을로 입장하겠습니다. 조금만 기다려주세요");
                    Counting(3);
                    //씬 이동. 타입을 마을로.
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
                        Text.TextingLine("5. 세이브\n", ConsoleColor.Green, false);
                        int input = Text.GetInput(null, 1, 2, 3, 4, 5);
                        switch (input)
                        {
                            //상태 보기
                            case 1:
                                Console.Clear();
                                Text.TextingLine("플레이어의 현재 스탯입니다.", ConsoleColor.Green);
                                Thread.Sleep(Waitingtime);
                                player.ShowStat();
                                input = Text.GetInput("마을로 돌아가시려면 0을 입력해주세요.", 0);
                                Console.Clear();
                                break;
                            //인벤토리
                            case 2:
                                Console.Clear();
                                player.inven.ShowInventory();
                                Console.Clear();
                                break;
                            //상점
                            case 3:
                                Console.Clear();
                                input = Text.GetInput("\n\n1. 구매를 원합니다.\n\n2. 판매를 원합니다.", 1, 2);
                                Console.Clear();
                                if(input == 1)
                                    shop.ShowShop();
                                if (input == 2)
                                    shop.ShowSelling();
                                Console.Clear();
                                break;
                            //스테이지 선택
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
                                //스테이지 설정을 불러오기 위한 데이터베이스 접근
                                stage = StageDB.stageList[input-1];
                                Text.TextingLine($"스테이지 {input}으로 이동합니다.", ConsoleColor.Green);
                                //스테이지 단계 저장(클리어 보수를 구분하기 위함)
                                Curstage = input - 1;
                                Counting(3);
                                //씬타입을 모험으로
                                curSceneType = SceneType.Explore;
                                break;
                            case 5:
                                Save();
                                Text.TextingLine("저장되었습니다!", ConsoleColor.Green);
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
                //모험(스테이지)
                else if (curSceneType == SceneType.Explore)
                {
                    Console.Clear();
                    Random rand = new Random();
                    //공격 턴을 구분하기 위함
                    bool isPlayerturn = true;
                    int clear = 0; 
                    for (int i = 0; i < stage.length; i++)
                    {
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
                        //전투 시작
                        if (input == 0)
                        {
                            while (true)
                            {
                                //몬스터나 플레이어가 죽을 시 전투 종료
                                if (player.IsDead == true || monster.IsDead == true)
                                {
                                    break;
                                }
                                //플레이어 턴
                                if (isPlayerturn == true)
                                {
                                    player.AttackEnemy(monster);
                                    isPlayerturn = !isPlayerturn;
                                    Thread.Sleep(1000);
                                }
                                //적 턴
                                else if (isPlayerturn == false)
                                {
                                    monster.AttackEnemy(player);
                                    isPlayerturn = !isPlayerturn;
                                    Thread.Sleep(1000);
                                }
                            }
                            //플레이어가 패배했을 시.
                            if (player.IsDead == true)
                            {
                                isPlayerturn = true;
                                Text.TextingLine("패배하였습니다.", ConsoleColor.Red);
                                Text.GetInput("다음 (0 입력)", 0);
                            }
                            //플레이어가 승리했을 시
                            else
                            {
                                isPlayerturn = true;
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
                            //마을 귀환을 택할 경우 반복문(모험) 탈출
                            //0이면 break를 하지 않기에 계속 반복문(모험) 진행
                            if (input == 1)
                                break;
                        }
                        //도망가기 선택 시 반복문(모험) 탈출
                        else 
                            break;
                        //플레이어가 죽엇을 시 반복문을 탈출(while문 조건을 채운다)
                        if (player.IsDead == true)
                        {
                            gameexit = true;
                            break;
                        }
                    }
                    //씬 타입을 마을로
                    curSceneType = SceneType.Home;
                }
            }
            //while문 탈출(게임 종료 활성화 시 = 사망) 스탯창을 띄우고 게임 종료
            Console.Clear();
            Console.WriteLine("게임 끝!");
            player.ShowStat();
        }
        //대기 시간 구현 메서드
        static  void Counting(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Console.Write(num - i + "   ");
                Thread.Sleep(1000);
            }
        }

        //세이브 기능. 상속받은 정보까지 가져가기 위해 All로 지정.
        static void Save()
        {
            string userdata = JsonConvert.SerializeObject(player, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(path + "\\UserData.json", userdata);

            string inventorydata = JsonConvert.SerializeObject(player.inven.items, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(path + "\\InvenData.json", inventorydata);
        }

        //로드 기능.
        static void Load()
        {
            if (!File.Exists(path + "\\UserData.json"))
            {
                player.inven.items = new List<IItem>();
                return;
            }
            else
            {
                string userLData = File.ReadAllText(path + "\\UserData.json");
                Player? userLoadData = JsonConvert.DeserializeObject<Player>(userLData, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
                player = userLoadData;

                string invenLData = File.ReadAllText(path + "\\InvenData.json");
                List<IItem>? invenLoadData = JsonConvert.DeserializeObject<List<IItem>>(invenLData, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
                if (player.inven == null)
                {
                    player.inven = new Inventory(player);
                }
                foreach (IItem data in invenLoadData)
                {
                    player.inven.AddItem(data);
                }
                Text.TextingLine("로드 성공.");
                Thread.Sleep(Waitingtime);
                Console.Clear();
            }
        }
    }
}
