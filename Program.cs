using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
using script;
using script.builder;
using script.variabel;

namespace test
{
    class Program
    {
        private static ArrayList test = new ArrayList();
        private static bool isError = false;
        private static int ticks = 5000;

        static void Main(string[] args)
        {
            Console.WriteLine("CowScript test program.");
            Console.WriteLine("This run series of test to control the lexer");
            Console.WriteLine("This set diffrence test there is desined to show if there is some error");
            

            if (!Directory.Exists("tests"))
            {
                Console.WriteLine("tests/ dont exists");
                Console.ReadLine();
                return;
            }

            Stopwatch clock = Stopwatch.StartNew();

            //run all files and dir to run the tests
            exuteDir("tests/");
            
            clock.Stop();
            Console.WriteLine("Time taken: {0}ms", clock.Elapsed.TotalMilliseconds);

            //wee wait to the user is hit a buttom
            Console.ReadLine();
        }

        private static CVar Print_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            Console.WriteLine(stack[0].toString(pos, data, db));
            return null;
        }

        private static void showErrro(string name, Energy error)
        {
            isError = true;
            Console.WriteLine("Error in file: " + name);
            if (error.getRunningStatus() != RunningState.Error)
            {
                Console.WriteLine("Unknown end status code: " + error.getRunningStatus().ToString());
                return;
            }
            
            Console.WriteLine("Message: " + error.getError().Message);
            if (error.getError().Posision != null)
            {
                Console.WriteLine("Line: " + error.getError().Posision.Line);
                Console.WriteLine("Row: " + error.getError().Posision.Row);
            }
            isError = true;
        }

        private static void exuteDir(string dir)
        {
            //run all dir first :)
            foreach (string d in Directory.GetDirectories(dir))
                exuteDir(d);

            //okay now we got all dir now is it files turn
            foreach (string file in Directory.GetFiles(dir, "*.jus"))
                doFile(file);
        }

        private static void doFile(string name)
        {
            string file = File.ReadAllText(name);

            Function print = new Function();
            print.Name = "print";
            print.agument.push("string", "context");
            print.call += Print_call;

            Function useage = new Function();
            useage.Name = "useage";
            useage.call += Useage_call;
             

            for (int i = 1; i <= ticks; i++)
            {
                if (isError)
                    return;
                Energy e = new Energy();
                e.setConfig("file.enabled", "true", false);
                e.setConfig("error.log.file", "tests/log.txt", false);
                e.push(print);
                e.push(useage);
                e.parse(file);
                if(e.getRunningStatus() != RunningState.Normal)
                {
                    showErrro(name, e);
                }
            }
        }

        private static CVar Useage_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            Console.WriteLine("----container----");
            foreach(string co in db.ContainerKeys())
            {
                Console.WriteLine("-" + co);
            }

            Console.WriteLine("------Global-----");
            foreach(string go in db.GlobalsKey())
            {
                Console.WriteLine("-" + go);
            }

            return new NullVariabel();
        }
    }
}
