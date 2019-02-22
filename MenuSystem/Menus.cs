using Service;

namespace MenuSystem {
    public static class Menus {
        private static readonly Menu ConfigReadmeMenu = new Menu {
            Title = "Config Readme",
            Description = new[] {
                "POESESSID is required for the official site live search to",
                "work. Without it you can only use the other live search",
                "methods."
            }
        };

        
        private static readonly Menu ConfigMenu = new Menu {
            Title = "Config Menu",
            MenuItems = new[] {
                new MenuItem {
                    IsDefaultChoice = true,
                    Description = "Readme",
                    Shortcut = "1",
                    MenuToRun = ConfigReadmeMenu
                },
                new MenuItem {
                    Description = "Set POESESSID",
                    Shortcut = "2",
                    MenuToRun = new Menu {
                        Title = "Config Menu",
                        PrintActionToExecute = Controller.SetPoeSessId
                    }
                }
            }
        };

        private static readonly Menu UrlReadmeMenu = new Menu {
            Title = "URL Readme",
            Description = new[] {
                "Create a file 'poelive.txt' and place all search URLs there.",
                "The line above an URL will act as a title for that connection.",
                "The file must be in the same folder as the executable. The",
                "first line must not be a URL.",
                "Example file contents:",
                "    exalted orbs",
                "    http://poe.trade/search/osikarikinoami",
                "",
                "    exalted orbs but from the official site",
                "    http://www.pathofexile.com/trade/search/Betrayal/NV6ofp",
                "    http://www.pathofexile.com/trade/search/Betrayal/NV6ofp"
            }
        };

        private static readonly Menu UrlMenu = new Menu {
            Title = "URL Menu",
            PrintActionToExecute = Controller.PrintConnections,
            MenuItems = new[] {
                new MenuItem {
                    IsDefaultChoice = true,
                    Description = "Readme",
                    Shortcut = "1",
                    MenuToRun = UrlReadmeMenu
                },
                new MenuItem {
                    Description = "Load file",
                    Shortcut = "2",
                    MenuToRun = new Menu {
                        Title = "URL Menu",
                        PrintActionToExecute = Controller.Load
                    }
                },
                new MenuItem {
                    Description = "Open folder",
                    Shortcut = "3",
                    ActionToExecute = Controller.OpenInExplorer
                },
                new MenuItem {
                    Description = "Remove all",
                    Shortcut = "4",
                    ActionToExecute = Controller.RemoveConnections
                }
            }
        };

        public static readonly Menu MainMenu = new Menu {
            Title = "Main Menu",
            MenuItems = new[] {
                new MenuItem {
                    Description = "Connections",
                    Shortcut = "1",
                    MenuToRun = UrlMenu,
                    IsDefaultChoice = true
                },
                new MenuItem {
                    Description = "Start search",
                    Shortcut = "2",
                    MenuToRun = new Menu {
                        Title = "Search",
                        PrintActionToExecute = Controller.Run
                    }
                },
                new MenuItem {
                    Description = "Config",
                    Shortcut = "3",
                    MenuToRun = ConfigMenu
                }
            }
        };

        public static readonly MenuItem GoBackItem = new MenuItem {
            Shortcut = "X",
            Description = "Go back"
        };

        public static readonly MenuItem ExitProgramItem = new MenuItem {
            Shortcut = "X",
            Description = "Exit program"
        };
    }
}