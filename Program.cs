using System;
using System.Collections.Generic;

namespace EncoderWrite {
    class MainClass {
        public static void Exit() {
            Console.ResetColor();
            if (System.Environment.OSVersion.ToString().Substring(0, 4) != "Unix") {
                Console.Write("Press any key to continue...");
                Console.ReadKey(true);
                Console.WriteLine();
            }
        }
        public static bool Arg(string[] args, int which, string equalTo) {
            if (args.Length >= which) {
                if (args[which - 1] == equalTo) {
                    return true;
                }
            }
            return false;
        }
        public static string ReadInput(string[] args) {
            Console.WriteLine("Enter message:");
            var input = "";
            if (Arg(args, 3, "read")) {
                input = new System.IO.StreamReader("input.txt", System.Text.Encoding.UTF8).ReadLine();
                Console.WriteLine("From file: {0}", input);
                return input;
            }
            return Console.ReadLine();
        }
        public static void WriteOutput(string[] args, string output) {
            if (Arg(args, 3, "write")) {
                System.IO.File.WriteAllLines("output.txt", new string[] { output }, System.Text.Encoding.UTF8);
                Console.WriteLine("Written to file: output.txt");
                return;
            }
            Console.WriteLine(output);
        }
        public static bool MinFilled(List<List<int>> array, int fillRateMinimum) {
            for (var i = 0; i < array.Count; i++) {
                if (array[i].Count < fillRateMinimum) {
                    return false;
                }
            }
            return true;
        }
        public static int ToNumber(string chars) {
            if (chars.Length == 3) {
                return (((int)chars[1]) << 8) + (int)chars[2];
            }
            return (int)chars[0];
        }
        public static string ToString(int number) {
            if (number > 0xFFFF) {
                return "#" + ((char)((number >> 16) << 8)).ToString() + ((char)(number & 0xFFFF)).ToString();
            }
            return ((char)number).ToString();
        }
        public static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            var alphabet = "";
            var encoders = new List<List<int>> { };
            if (args.Length > 0) {
                if (Arg(args, 2, "auto")) {
                    alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö.,-!?' 1234567890";
                    for (var i = 0; i < alphabet.Length; i++) {
                        encoders.Add(new List<int> { });
                    }
                    Console.WriteLine("Minimum amount of variants per character?\nThe program WILL crash if you don't enter a vaild number\n");
                    var fillRate = int.Parse(Console.ReadLine());
                    var seed = 0;
                    while (!MinFilled(encoders, fillRate)) {
                        if (seed < 0) {
                            Console.WriteLine("Overflow error");
                            break;
                        }
                        var newChar = (char)new Random(seed).Next();
                        var index = alphabet.IndexOf(newChar);
                        if (index != -1) {
                            encoders[index].Add(seed);
                        }
                        seed++;
                    }
                    for (var i = 0; i < encoders.Count; i++) {
                        Console.Write(alphabet[i] + ": ");
                        for (var j = 0; j < encoders[i].Count; j++) {
                            Console.Write(encoders[i][j] + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("Terminate");
                } else {
                    Console.WriteLine("Enter key:");
                    var index = 0;
                    while (true) {
                        var currentLine = Console.ReadLine();
                        if (currentLine == "Terminate") {
                            break;
                        }
                        alphabet += currentLine[0];
                        currentLine = currentLine.Substring(2);
                        encoders.Add(new List<int> { });
                        var numbers = currentLine.Split(' ');
                        for (var i = 0; i < numbers.Length; i++) {
                            if (Int32.TryParse(numbers[i], out var tmpI)) {
                                encoders[index].Add(tmpI);
                            }
                        }
                    }
                }
                var input = ReadInput(args);
                if (args[0] == "w") {
                    var rnd = new Random();
                    var output = "";
                    for (var i = 0; i < input.Length; i++) {
                        var index = alphabet.IndexOf(input[i]);
                        output += ToString(encoders[index][rnd.Next() % encoders[index].Count]);
                    }
                    WriteOutput(args, output);
                } else if (args[0] == "r") {
                    var errors = 0;
                    var output = "";
                    for (var i = 0; i < input.Length; i++) {
                        var searchInt = (int)input[i];
                        var index = -1;
                        if (input[i] == '#') {
                            searchInt = ToNumber(input.Substring(i, 3));
                            i += 2;
                        }
                        for (var j = 0; j < encoders.Count; j++) {
                            if (encoders[j].Contains(searchInt)) {
                                index = j;
                                break;
                            }
                        }
                        if (index == -1) {
                            errors++;
                            output += (char)9949;
                        } else {
                            output += alphabet[index];
                        }
                    }
                    Console.WriteLine("Errors: {0}", errors);
                    WriteOutput(args, output);
                } else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid arguments\nFirst argument [r/w] encrypt/decrypt\nSecond argument [auto/*] generate key or enter manual key\nThird argument [read/write] read or write input/output to/from input.txt/output.txt");
                }
                Exit();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Missing arguments\nFirst argument [r/w] encrypt/decrypt\nSecond argument [auto/*] generate key or enter manual key\nThird argument [read/write] read or write input/output to/from input.txt/output.txt");
            Console.WriteLine("If you ran this by double clicking on it, open a terminal and run with arguments");
            Exit();
        }
    }
}
