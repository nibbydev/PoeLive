using System;
using System.Reflection;
using CsQuery.ExtensionMethods.Internal;
using Domain;
using Service;
using Service.Connection;

namespace Program {
    public static class Program {
        static Program() {
            BaseConnection.DispatchNewItem = DispatchNewItem;
            BaseConnection.DispatchDelItem = DispatchDelItem;
            PaConnection.HashApiUrl = "http://127.0.0.1:3001/?obj_to_hash=";
        }

        private static void Main(string[] args) {
            MenuSystem.Menus.MainMenu.RunMenu();
        }

        private static void DispatchNewItem(Item item) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private static void DispatchDelItem(string identifier) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}