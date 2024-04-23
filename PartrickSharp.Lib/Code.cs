namespace PartrickSharp;

public class CodeConverter
{
    private const string chr1 = "0123456789BCDFGHJKLMNPQRSTVWXY";
    private const string chr2 = "00010110100000001110000001111100";
    private const string chr3 = "00000000000000000000000000000000";

    public static string Num2Code(long N, bool IsLvl)
    {
        var A = "1000";
        var D = IsLvl ? "0" : "1";
        var E = "1";

        long H = N ^ Convert.ToInt64(chr2, 2);
        var R = $"H={H}\n";

        var B = Strings.Right(chr3 + Convert.ToString((N - 31) % 64, 2), 6);
        var FC = Strings.Right(chr3 + Convert.ToString(H, 2), 32);
        var F = Strings.Left(FC, 12);
        var C = Strings.Right(FC, 20);

        var R = $"编号：{N}\n\n";
        R += $"A={A}\n";
        R += $"B={B}\n";
        R += $"C={C}\n";
        R += $"D={D}{(D == "0" ? "(L)" : "(M)")}\n";
        R += $"E={E}\n";
        R += $"F={F}\n";
        long num = Convert.ToInt64(A + B + C + D + E + F, 2);
        R += $"N2={A + B + C + D + E + F}\n";
        R += $"N1={num}\n";

        var code = "";
        while (num != 0)
        {
            code = chr1[(int)(num % 30)] + code;
            num /= 30;
        }

        return code;
    }

    public static int Code2Num(string code, ref string INFO)
    {
        long num = 0;
        for (int i = 9; i >= 1; i--)
        {
            num = num * 30 + chr1.IndexOf(code[i - 1]);
        }

        var N = Convert.ToString(num, 2);
        var A = Strings.Mid(N, 1, 4); // A=1000
        var B = Strings.Mid(N, 5, 6);
        var C = Strings.Mid(N, 11, 20);
        var D = Strings.Mid(N, 31, 1); // D=0 Level  D=1 Maker
        var E = Strings.Mid(N, 32, 1); // E=1
        var F = Strings.Right(N, 12);

        long H = (Convert.ToInt64(F + C, 2) ^ Convert.ToInt64(chr2, 2));
        long M = (H - 31) % 64;

        var INFO = $"{(D == "0" ? "    关卡ID：" : "    工匠ID：")}{code}\n";
        INFO += $"    N1={num}\n";
        INFO += $"    N2={N}\n";
        INFO += $"    A ={A}\n";
        INFO += $"    B ={B} ({Convert.ToInt64(B, 2)})\n";
        INFO += $"    C ={C}\n";
        INFO += $"    D ={D}{(D == "0" ? " (Level)" : " (Maker)")}\n";
        INFO += $"    E ={E}\n";
        INFO += $"    F ={F}\n";
        INFO += $"    G ={Convert.ToInt64(F + C, 2)}\n";
        INFO += $"    H ={H}\n";
        INFO += $"    M ={M}\n";
        INFO += $"    K ={Convert.ToString(H, 2)}\n";
        INFO += $"{(D == "0" ? "    关卡序号：" : "    工匠序号：")}{H}";

        if (M.ToString() == Convert.ToInt64(B, 2).ToString())
        {
            return int.Parse(D);
        }
        else
        {
            return -1;
        }
    }
}
