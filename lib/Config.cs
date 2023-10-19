using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DSmail
{
    public class Config
    {
        public Pop3? Pop3 { get; set; }
    }

    public class Pop3
    {
        public int Period { get; set; }
        public Item[]? Items {get; set; }
        public void Pop3TimerCallback(System.Object o)
        {
            Console.Write(".");
            DSmail.Api.ReceiveMessages(Items);
        }
    }
    public class Item
    {
        public string? Server { get; set; }
        public int Port { get; set; }
        public string? UserName { get; set; }
        public string? Passwd { get; set; }
        public string? Path { get; set; }
        public int Count  { get; set; }
        public int Timeout { get; set; }
    }
}
    
