using System;
using System.Collections.Generic;

namespace EncoderWrite {
    class MainClass {
        private static bool showErrorsAndInstructions = false;
        public static void Exit() {
            Console.ResetColor();
            if (System.Environment.OSVersion.ToString().Substring(0x0, 0x4) != "Unix") {
                Console.Write("Press any key to continue...");
                Console.ReadKey(true);
                Console.WriteLine();
            }
        }
        public static bool Arg(string[] args, int which, string equalTo) {
            if (args.Length >= which) {
                if (args[which - 0x1] == equalTo) {
                    return true;
                }
            }
            return false;
        }
        public static string ReadInput(string[] args) {
            Console.WriteLine("Enter message:");
            var input = "";
            if (Arg(args, 0x3, "read")) {
                input = new System.IO.StreamReader("input.txt", System.Text.Encoding.UTF8).ReadLine();
                Console.WriteLine("From file: {0}", input);
                return input;
            }
            return Console.ReadLine();
        }
        public static void WriteOutput(string[] args, string output) {
            if (Arg(args, 0x4, "write")) {
                System.IO.File.WriteAllLines("output.txt", new string[] { output }, System.Text.Encoding.UTF8);
                Console.WriteLine("Written to file: output.txt");
                return;
            }
            Console.WriteLine(output);
        }
        public static bool MinFilled(List<List<int>> array, int fillRateMinimum) {
            for (var i = 0x0; i < array.Count; i++) {
                if (array[i].Count < fillRateMinimum) {
                    return false;
                }
            }
            return true;
        }
        public static int ToNumber(string chars) {
            if (chars.Length == 0x3) {
                return (((int)chars[0x1]) << 0x8) + (int)chars[0x2];
            }
            return (int)chars[0x0];
        }
        public static string ToString(int number) {
            if (number > 0xFFFF) {
                return "#" + ((char)((number >> 0x10) << 0x8)).ToString() + ((char)(number & 0xFFFF)).ToString();
            }
            return ((char)number).ToString();
        }
        public static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            var alphabet = "";
            var encoders = new List<List<int>> { };
            if (args.Length > 0x0) {
                if (Arg(args, 0x2, "auto")) {
                    alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö.,-!?' 1234567890";
                    for (var i = 0x0; i < alphabet.Length; i++) {
                        encoders.Add(new List<int> { });
                    }
                    Console.WriteLine("Minimum amount of variants per character?\nThe program WILL crash if you don't enter a vaild number\n");
                    var fillRate = int.Parse(Console.ReadLine());
                    var seed = 0x0;
                    while (!MinFilled(encoders, fillRate)) {
                        if (seed < 0x0) {
                            Console.WriteLine("Overflow error");
                            break;
                        }
                        var newChar = (char)new Random(seed).Next();
                        var index = alphabet.IndexOf(newChar);
                        if (index != 0xF - 0x10) {
                            encoders[index].Add(seed);
                        }
                        seed++;
                    }
                    for (var i = 0x0; i < encoders.Count; i++) {
                        Console.Write(alphabet[i] + ": ");
                        for (var j = 0x0; j < encoders[i].Count; j++) {
                            Console.Write(encoders[i][j] + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("Terminate");
                } else {
                    Console.WriteLine("Enter key:");
                    var index = 0x0;
                    while (true) {
                        var currentLine = Console.ReadLine();
                        if (currentLine == "Terminate") {
                            break;
                        }
                        alphabet += currentLine[0];
                        currentLine = currentLine.Substring(0x2);
                        encoders.Add(new List<int> { });
                        var numbers = currentLine.Split(' ');
                        for (var i = 0; i < numbers.Length; i++) {
                            var tmpI = 0x0;
                            if (Int32.TryParse(numbers[i], out tmpI)) {
                                encoders[index].Add(tmpI);
                            }
                        }
                    }
                }
                var input = ReadInput(args);
                if (args[0x0] == "w") {
                    var rnd = new Random();
                    var output = "";
                    for (var i = 0x0; i < input.Length; i++) {
                        var index = alphabet.IndexOf(input[i]);
                        output += ToString(encoders[index][rnd.Next() % encoders[index].Count]);
                    }
                    WriteOutput(args, output);
                } else if (args[0x0] == "r") {
                    var errors = 0x0;
                    var output = "";
                    for (var i = 0x0; i < input.Length; i++) {
                        var searchInt = (int)input[i];
                        var index = 0xF - 0x10;
                        if (input[i] == '#') {
                            searchInt = ToNumber(input.Substring(i, 0x3));
                            i += 0x2;
                        }
                        for (var j = 0x0; j < encoders.Count; j++) {
                            if (encoders[j].Contains(searchInt)) {
                                index = j;
                                break;
                            }
                        }
                        if (index == 0xF - 0x10) {
                            errors++;
                            output += (char)0x26DD;
                        } else {
                            output += alphabet[index];
                        }
                    }
                    Console.WriteLine("Errors: {0}", errors);
                    WriteOutput(args, output);
                } else {
                    if (showErrorsAndInstructions) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid arguments\nFirst argument [r/w] encrypt/decrypt\nSecond argument [auto/*] generate key or enter manual key\nThird argument [read/*] read input from input.txt\nFourth argument [write/*] write output to output.txt");
                    }
                }
                Exit();
                return;
            }
            if (showErrorsAndInstructions) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Missing arguments\nFirst argument [r/w] encrypt/decrypt\nSecond argument [auto/*] generate key or enter manual key\nThird argument [read/*] read input from input.txt\nFourth argument [write/*] write output to output.txt");
                Console.WriteLine("If you ran this by double clicking on it, open a terminal and run with arguments");
            }
            Exit();
        }
    }
}
