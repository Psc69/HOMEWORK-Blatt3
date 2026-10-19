using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading;

public static class Settings
{
    public static readonly Action[] ListeAufgaben =
    [
        Aufgaben.Aufgabe1,
        Aufgaben.Aufgabe2,
        Aufgaben.Aufgabe3,
        Aufgaben.Aufgabe4
    ];
    public const char QUITTING_BUTTON = 'q';
    public const ConsoleColor BACKGROUND_COLOR = ConsoleColor.Black;
}
public class Logic
{
    public static void throwError(string errorType = "")
    {
        if (errorType != "")
        {
            Console.Write($"Error Type: {errorType}.");
            for (int i = 0; i<2; i++)
            {
                Thread.Sleep(750);
                Console.Write('.');
            }
        }

        //fühl mich heute lustig :p
        else
        {
            Console.Write($"Error!");
            for (int i = 0; i < 2; i++)
            {
                Thread.Sleep(1000);
                Console.Write('!');
            }
        }

        Home.InputToHome();
    }
    public static string MakeFirstCharBig(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty; // or throw / call Logic.throwError() if empty input should be treated as error

        return char.ToUpper(name[0]) + name[1..];
    }
    public static void MakeStringCool(string name, char casingType = '#')
    {
        Console.WriteLine("\n");

        int n = name.Length + 1;
        for (int i = 0;i <= n; i++) { Console.Write(casingType); }
        Console.Write($"\n{casingType + name + casingType}\n");
        for (int i = 0;i <= n; i++) { Console.Write(casingType); }
            
    }
    public static float ParseToFloat(string input)
    {
        //parsing
        if (float.TryParse(input, out float output))
        {
            //return
            return output;
        } 
        else
        {
            throwError("Not valid num!"); //could be dangerous cuz throwerror sends to home?
            return 0f;
        }
    }
}
public class Calc //short for calculator 🤓
{
    public static List<string> tokenizeStr(string input)
    {
        //var
        List<string> tokens = new List<string>();
        int i = 0;

        while (i < input.Length)
        {
            char c = input[i];

            //whitespaces ignorieren
            if (char.IsWhiteSpace(c)) { i++; continue; }

            //Nummer Logic (komma- / ganze Zahl B) )
            if (char.IsDigit(c) || c == '.' || c == ',')
            {
                string numberStr = "";
                while (i < input.Length && (char.IsDigit(c) || c == '.' || c == ','))
                {
                    numberStr += c;
                    i++;
                }
                tokens.Add(numberStr);
                continue;
            }

            //Operatoren Logic :/
            if (c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')')
            {
                tokens.Add(c.ToString());
                i++;
                continue;
            }

            //error handling :o
            throw new Exception("Invalid char gefunden D:");
        }
        return tokens;
    }
    public static double EvaluateRPN(List<string> tokens)
    {
        var stack = new Stack<double>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out double result)) { stack.Push(result); }
            else
            {
                double b = stack.Pop();
                double a = stack.Pop();

                double ans = token switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => a / b,
                    _ => throw new InvalidOperationException($"Unknown Operator: {token}")
                };

                stack.Push(ans);
            }
        }

        return stack.Pop();
    }
}
public class Aufgaben
{
    public static void Aufgabe1()
    {
        //var
        string vorname;
        string nachname;
        {
            //process
            Console.Write("Bitte gib deinen Vorname ein: ");
            vorname = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(vorname)) { Logic.throwError(); return; }
            vorname = Logic.MakeFirstCharBig(vorname);

            Console.Write("Bitte gib deinen Nachname ein: ");
            nachname = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(nachname)) { Logic.throwError(); return; }
            nachname = Logic.MakeFirstCharBig(nachname);
        }
        string vollerName = $"{vorname} {nachname}";

        //return
        Console.Clear();
        Console.Write($"Ihr Name ist");
        for (int i = 0; i<3; i++)
        {
            Thread.Sleep(500);
            Console.Write('.');
        }

        Logic.MakeStringCool($"{vollerName} !!");


        //exit
        Home.InputToHome();
    }
    public static void Aufgabe2()
    {
        //process
        try
        {
            //input
            Console.WriteLine("Bitte gib dein Alter ein: ");

            //var
            string line = Console.ReadLine() ?? "";
            int alter = Convert.ToInt32(line);

            //return
            Console.WriteLine($"Ihr Alter ist: {alter}");
            Home.InputToHome();
        }
        //error handling :p
        catch (FormatException)
        {
            Logic.throwError("Input = keine Zahl!");
        }
        catch (OverflowException)
        {
            Logic.throwError("Input > 2^32!!");
        }
    }
    public static void Aufgabe3()
    {
        //print
        Console.WriteLine("Fläche von Rechteck = Länge * Breite");
        float ans;
        
        //input
        Console.Write("Länge = ");
        string lengthInput = Console.ReadLine() ?? "";
        Console.Write("Breite = ");
        string widthInput = Console.ReadLine() ?? "";

        //var
        float length = Logic.ParseToFloat(lengthInput); 
        float width = Logic.ParseToFloat(widthInput);

        //magie :o
        ans = length * width;

        //output
        Console.WriteLine($"\n{ans.ToString("F2")} = {length} * {width}");

        //exit
        Home.InputToHome();


    }
    public static void Aufgabe4()
    {
        //var
        bool debug = false;
        List<string> tokens = new();
        double result = 0f;

        //input
        Console.Clear();
        Console.Write("Bitte gib deine rechnung zu: ");
        string input = Console.ReadLine() ?? "";


        //Shunting Yard Problem D:
        try         
        {
            //tokeniziation
            tokens = Calc.tokenizeStr(input);
            if (debug)
            {
                Console.WriteLine($"Tokens: {string.Join(" ", tokens)}");
            }
        }
        catch { Logic.throwError("Error during tokenizing :( "); return; }

        //Evaluate RPN (Reverse Polish Notation lol)
        try 
        {
            //magie
            result = Calc.EvaluateRPN(tokens);

            //make it look cool lol
            Console.Write("\nCalculating");
            for (int i = 0; i < 3; i++)
            {
                Console.Write('.');
                Thread.Sleep(750);
            }
            Console.Clear();

            //return
            Console.Write($"{input} = {result} (wahrscheinlich falsch :P)");

            //exit
            Home.InputToHome();
        }
        catch { Logic.throwError("Error during RPN Evaluation D: "); }

        //exit
        Home.InputToHome();
    }
    public static void Aufgabe5()
    {
        Console.Clear();
        Home.InputToHome();
    }
}

class Home
{
    public static void Run()
    {
        Print();
        Input();
    }
    public static void Print()
    {
        Console.Clear();
        Console.Write("Wähl deine Aufgabe aus:\n");

        int num = 1;
        foreach (Action aufgabe in Settings.ListeAufgaben)
        {
            string methodeName = aufgabe.Method.Name;
            Console.WriteLine($" {num}. {methodeName}");
            num++;
        }
    }

    public static void Input()
    {
        Console.Write($"\nDrücke die Taste '{char.ToUpper(Settings.QUITTING_BUTTON)}' um zu Verlassen.");
        Console.Write("\nBitte gebe eine Valide Nummer ein: ");

        ConsoleKeyInfo input = Console.ReadKey(true);
        char key = char.ToLower(input.KeyChar);

        if (key >= '1' && key <= '0' + Settings.ListeAufgaben.Length)
        {
            int choice = key - '0';
            Console.Clear();
            Settings.ListeAufgaben[choice - 1]();
        }
        else if (key == Settings.QUITTING_BUTTON)
        {
            Program.Quit();
            return;
        }
        else
        {
            Console.WriteLine("Invalide Auswahl.");
            Console.Clear();
            Run();
        }
    }

    public static void InputToHome()
    {
        Console.WriteLine("\nDrücken Sie eine beliebige Taste, um zurückzugehen.");
        Console.ReadKey(true);
        Run();
    }
}
public class Program
{
    private static void Start()
    {
        Console.Clear();
        Console.BackgroundColor = Settings.BACKGROUND_COLOR;
        Home.Run();
    }
    public static void Quit()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
    }
    static void Main()
    {
        Start(); 
    }
}
