using System;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;

namespace MenuSystem {
    public class Menu {
        public string Title { get; set; }
        public MenuItem[] MenuItems { get; set; }
        public bool ClearConsole { get; set; } = true;
        public Action PrintActionToExecute { get; set; }
        public string[] Description { get; set; }

        private void PrintMenu() {
            if (ClearConsole) {
                Console.Clear();
            }

            PrintHr(Title);

            if (Description != null) {
                Description.ForEach(s => Console.WriteLine($"* {s}"));
                PrintHr();
            }

            if (PrintActionToExecute != null) {
                PrintActionToExecute();
                PrintHr();
            }

            if (MenuItems != null) {
                MenuItems.ForEach(t => {
                    if (t.IsDefaultChoice) {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(t);
                        Console.ResetColor();
                    } else {
                        Console.WriteLine(t);
                    }
                });

                PrintHr();
            }

            // No other menu items so GoBackItem is default choice
            if (MenuItems == null) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }

            Console.WriteLine(Menus.MainMenu.Equals(this) ? Menus.ExitProgramItem : Menus.GoBackItem);
            Console.ResetColor();

            Console.Write("> ");
        }

        private static void PrintHr(string title = null) {
            const int width = 64;

            if (title == null) {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[>" + new string('=', width - 4) + "<]");
                Console.ResetColor();
            } else {
                if (title.Length % 2 != 0) title += " ";
                var hr = "[>" + new string('=', (width - title.Length - 2) / 2 - 4) + "<]";

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"{hr} ");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(title);
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($" {hr}");
                Console.ResetColor();
            }
        }

        public string RunMenu() {
            while (true) {
                PrintMenu();
                var input = Console.ReadLine()?.Trim();

                // Go back
                if (Menus.GoBackItem.Shortcut.Equals(input?.ToUpper())) {
                    return null;
                }

                // No other input methods
                if (MenuItems == null) {
                    return null;
                }

                // Load user-specified or default menu item
                var item = string.IsNullOrEmpty(input)
                    ? MenuItems.FirstOrDefault(m => m.IsDefaultChoice)
                    : MenuItems.FirstOrDefault(m => m.Shortcut == input);

                // The menu item was null
                if (item == null) {
                    Console.WriteLine("Unknown input!");
                    Console.ReadKey(true);
                    continue;
                }

                // execute the command specified in the menu item
                if (item.MenuToRun == null && item.ActionToExecute == null) {
                    Console.WriteLine($"'{item.Description}' has no command assigned to it...");
                    Console.ReadKey(true);
                    continue;
                }

                // If the selected menu item had an action, execute it
                item.ActionToExecute?.Invoke();
                // If the selected menu item had a menu, run it
                input = item.MenuToRun?.RunMenu();

                if (input == null) {
                    continue;
                }

                return input;
            }
        }
    }
}