using System;
using System.Collections.Generic;
using System.Linq;

namespace Huffman
{
    class Program
    {
        static void Main(string[] args)
        {
            //Получение отсортированного словаря количества вхождений уникальных символов
            Console.WriteLine("Введите текст");
            string str = Console.ReadLine(), resStr = "";
            Console.WriteLine("\nВведите пользовательскую пару символов");
            string doubleChar = Console.ReadLine(), strCopy = str;
            char[] chars = str.ToCharArray();
            int doubleCounter = 0;

            Dictionary<string, int> signs = new Dictionary<string, int>(), doubleSigns = new Dictionary<string, int>();

            foreach (char i in chars)
            {
                if (!signs.ContainsKey(Convert.ToString(i)))
                {
                    signs.Add(Convert.ToString(i), 0);
                }
            }
            foreach (KeyValuePair<string, int> kvp in signs)
            {
                resStr += $"'{kvp.Key}' : {kvp.Value}, ";
            }
            resStr = resStr.TrimEnd(new char[] { ',', ' ' });
            Console.WriteLine("\nИнициализированный словарь уникальных символов:\n" + resStr);

            resStr = "";
            foreach (char i in str)
            {
                signs[Convert.ToString(i)] += 1;
            }
            foreach (KeyValuePair<string, int> kvp in signs)
            {
                resStr += $"'{kvp.Key}' : {kvp.Value}, ";
            }
            resStr = resStr.TrimEnd(new char[] { ',', ' ' });
            Console.WriteLine("\nКоличество вхождений уникальных символов в строку:\n" + resStr);

            while (strCopy.IndexOf(doubleChar) > -1)
            {
                strCopy = strCopy.Remove(strCopy.IndexOf(doubleChar), 2);
                doubleCounter += 1;
            }
            Console.WriteLine("\nКоличество вхождений пользовательской пары в строку:\n" + doubleCounter);

            resStr = "";
            for (int i = 0; i <= str.Length - 2; i++)
            {
                string buf = string.Concat(str[i], str[i + 1]);
                if (!doubleSigns.ContainsKey(buf))
                {
                    doubleSigns.Add(buf, 0);
                }
            }
            foreach (KeyValuePair<string, int> kvp in doubleSigns)
            {
                resStr += $"'{kvp.Key}' : {kvp.Value}, ";
            }
            resStr = resStr.TrimEnd(new char[] { ',', ' ' });
            Console.WriteLine("\nИнициализированный словарь уникальных двойных сочетаний:\n" + resStr);

            resStr = "";
            for (int i = 0; i <= str.Length - 2; i++)
            {
                string buf = string.Concat(str[i], str[i + 1]);
                doubleSigns[buf] += 1;
            }
            foreach (KeyValuePair<string, int> kvp in doubleSigns)
            {
                resStr += $"'{kvp.Key}' : {kvp.Value}, ";
            }
            resStr = resStr.TrimEnd(new char[] { ',', ' ' });
            Console.WriteLine("\nКоличество вхождений уникальных пар символов в строку:\n" + resStr);

            resStr = "";
            int frequency = 0;
            foreach (KeyValuePair<string, int> kvp in doubleSigns)
            {
                if (kvp.Value > frequency)
                {
                    frequency = kvp.Value;
                }
            }
            foreach (KeyValuePair<string, int> kvp in doubleSigns)
            {
                if (kvp.Value == frequency)
                {
                    signs.Add(kvp.Key, kvp.Value);
                }
            }

            //Соритровка словаря по возрастанию значений
            SortDic(ref signs);

            foreach (KeyValuePair<string, int> kvp in signs)
            {
                resStr += $"'{kvp.Key}' : {kvp.Value}, ";
            }
            resStr = resStr.TrimEnd(new char[] { ',', ' ' });
            Console.WriteLine("\nСловарь с добавленными самыми частовстречаемыми двойными комбинациями:\n" + resStr);

            static void SortDic(ref Dictionary<string, int> signs)
            {
                signs = signs.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            //Составление дерева
            static void SortList(ref List<Node> list)
            {
                Node buff;
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        if (list[i].Value > list[j].Value)
                        {
                            buff = list[i];
                            list[i] = list[j];
                            list[j] = buff;
                        }
                    }
                }
            }

            List<Node> signNodes = new List<Node>();
            List<Node> treeNodes = new List<Node>();

            foreach (KeyValuePair<string, int> kvp in signs)
            {
                Node oN = new Node(kvp.Key, kvp.Value);
                signNodes.Add(oN);
            }

            do
            {
                for (int i = 0; i < 2; i++)
                {

                    treeNodes.Add(signNodes[i]);
                    if (i == 1)
                    {
                        Node nN = new Node(treeNodes[treeNodes.Count - 2], treeNodes[treeNodes.Count - 1], treeNodes[treeNodes.Count - 2].Value + treeNodes[treeNodes.Count - 1].Value);
                        signNodes.RemoveAt(0);
                        signNodes.RemoveAt(0);
                        signNodes.Add(nN);
                        SortList(ref signNodes);
                    }
                }
            } while (signNodes.Count > 1);
            treeNodes.Add(signNodes[0]);
            signNodes.RemoveAt(0);

            //Получение кодов
            void CreateBitCodes(Node n)
            {
                if (n.Sign == null)
                {
                    n.LargeParent.CodeBit = n.CodeBit + "1";
                    CreateBitCodes(n.LargeParent);
                    n.SmallParent.CodeBit = n.CodeBit + "0";
                    CreateBitCodes(n.SmallParent);
                }
            }

            CreateBitCodes(treeNodes[treeNodes.Count - 1]);

            Dictionary<string, string> signCodes = new Dictionary<string, string>();
            foreach (Node n in treeNodes)
            {
                if (n.Sign != null)
                {
                    signCodes.Add(n.Sign, n.CodeBit);
                }
            }

            Console.WriteLine("\nКод для каждого символа:");
            foreach (KeyValuePair<string, string> kvp in signCodes)
            {
                Console.WriteLine(kvp.Key + ": " + kvp.Value);
            }

            //Кодирование строки
            Console.WriteLine("\nВведите строку для кодирования");
            string testStr = Console.ReadLine(), crypTestStr = "";
            for (int i = 0; i < testStr.Length - 1; i++)
            {
                if (signCodes.ContainsKey(Convert.ToString(testStr[i] + Convert.ToString(testStr[i + 1]))))
                {
                    crypTestStr += signCodes[Convert.ToString(testStr[i] + Convert.ToString(testStr[i + 1]))];
                    i++;
                }
                else if (signCodes.ContainsKey(Convert.ToString(testStr[i])))
                {
                    crypTestStr += signCodes[Convert.ToString(testStr[i])];
                }
            }
            if (signCodes.ContainsKey(Convert.ToString(testStr[testStr.Length - 1])))
            {
                crypTestStr += signCodes[Convert.ToString(testStr[testStr.Length - 1])];
            }
            Console.WriteLine("\nЗакодированная пользовательская строка:\n" + crypTestStr);

            //Декодирование строки
            string decChar = "", decStr = "";
        Here:
            for (int i = 0; i < crypTestStr.Length; i++)
            {
                decChar += crypTestStr[i];
                if (signCodes.ContainsValue(decChar))
                {
                    decStr += signCodes.Where(x => x.Value == decChar).First().Key;
                    decChar = "";
                }
                if (crypTestStr.Length > 0)
                {
                    crypTestStr = crypTestStr.Remove(0, 1);
                    goto Here;
                }
            }
            Console.WriteLine("\nДекодированная пользовательская строка:\n" + decStr);
        }
    }
}