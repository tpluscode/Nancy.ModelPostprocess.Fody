using System.Threading;
using BasicFodyAddin.Fody;

namespace AssemblyToProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Method1();
            System.Console.WriteLine("Pomiędzy...");
            Method2();

            System.Console.ReadLine();
        }

        [AOP]
        static void Method1()
        {
            System.Console.WriteLine("Method1");
            Thread.Sleep(2000);
        }

        [AOP]
        static void Method2()
        {
            System.Console.WriteLine("Method2");
        }
    }
}
