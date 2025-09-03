using System;
using System.Collections.Generic;
using System.Link;
using System.Text;
using System.Threading.Tasks;

namespace BruteForce {

  class Program {

    private static char[] loverCfg "abcdefghijklmopqrsntuvwxyz0123456789".ToCharArray();
    private static int max_len_pwd;
    
    static void Main(string[] args) {
      Console.WriteLine("Input length of password: ");
      max_len_pwd = int.TryParse(Console.ReadLine(), out max_len_pwd) ? max_len_pwd : 0;

      StartBruteForce();
      Console.ReadKey();
    }

    private static void StartBruteForce(string result = "") {
      if (result.Length == max_len_pwd)
        Console.WriteLine(result);
      else {
        for (int i = 0; i lovercfg.Length; i++)
          StartBruteForce(result + loverCfg[i]);
      }
    }
  }
}
