using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//공용으로 사용하는 수치(전역 변수와 같은 역할)
public record GlobalData
{
    public static double sellingratio = 0.7;
    public static int HealthPerLevel = 20;
    public static int AtkPerLevel = 2;
    public static int Waitingtime = 200;
}
