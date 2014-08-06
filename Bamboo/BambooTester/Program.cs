using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Bamboo;

namespace BambooTester {
class Program {

//public static void thisFunction(string x, string xx, string xxx) { }
//public static void thatFunction(out string x, out string xx) { x = ""; xx = ""; }
//public delegate void OutAction<T1, T2>(out T1 a, out T2 b);
//public delegate void MyAction<T1, T2, T3, T4, T5>(T1 sCommand, T2 sCommandParameters, T3 sWorkingDirectory, out T4 Err, out T5 Result);

static void Main(string[] args) {

    string Err = "";
    string Result = "";
    ArrayList tArray = null;

    Console.WriteLine( Bamboo.About.PrintMe() );
    Console.WriteLine( "Press any key for demo" );
    Console.ReadLine();

    // shell on single thread and wait for end, app halts until shell ends on thread
    Bamboo.Shells bShells = new Bamboo.Shells();
    bShells.ShellTo("calc.exe", "", @"C:\", out Err, out Result);

    // example of how to get ip addresses
    Bamboo.ActiveDirectoryMore bADmore = new Bamboo.ActiveDirectoryMore();
    tArray = bADmore.DNS_GetIpAddresses("localhost", true);

    // shell on multi thread and wait for end, app continues while another thread runs until shell ends on that thread
    //MyAction<string, string, string, string, string> MyFunction = bShells.ShellTo;
    //object[] parameters = new object [] { "mspaint.exe", "", @"C:\", Err, Result };
    //bThreads.Initialize(MyFunction, parameters);


}


}
}
