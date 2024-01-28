using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeRoom_Fertig
{
    internal class Game
    {
        // Variablen für die Map
        private static int MAP_SIZE;
        private static int[,] map;

        // Variablen für die Eingabe und Handlung des Spielers
        private int userInput;
        private bool isValidInput = false;
        public static bool isRunning = true;
        private static bool isKeyCollected = false;

        // Variablen für den Character
        private static int playerX;
        private static int playerY;
        private static string playerChar = ":)";

        // Variablen für die Tür
        public static bool hasExitedDoor = false;

        private enum EMapTiles
        // Mehr Variablen bzw ein ENUM für die Map
        {
            floor = -1,
            wall,
            key,
            openDoor,
            closedDoor
        }

        private static string[] mapTileChar = new string[]
        // Noch mehr Variablen für die Map. Mensch das hört nicht auf....
        {
            "  ", // Boden
            "██", // Wand
            "O╣", // Schlüssel
            "▒▒", // Offene Tür
            "▓▓"  // Geschlossene Tür
        };

        public void Run()
        // Ersetzt die Main Methode in Program.cs
        {
            WelcomeMessage();
            InstructionMessage();
            MapSize();
            DrawMap();
        }

        private void WelcomeMessage()
        // Bisschen was fürs Auge.
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" _____ ____   ____    _    ____  _____   ____   ___   ___  __  __ \r\n| ____/ ___| / ___|  / \\  |  _ \\| ____| |  _ \\ / _ \\ / _ \\|  \\/  |\r\n|  _| \\___ \\| |     / _ \\ | |_) |  _|   | |_) | | | | | | | |\\/| |\r\n| |___ ___) | |___ / ___ \\|  __/| |___  |  _ <| |_| | |_| | |  | |\r\n|_____|____/ \\____/_/   \\_\\_|   |_____| |_| \\_\\\\___/ \\___/|_|  |_|");
            Console.WriteLine("__        _____ ____  _   _          \r\n\\ \\      / /_ _/ ___|| | | |         \r\n \\ \\ /\\ / / | |\\___ \\| |_| |         \r\n  \\ V  V /  | | ___) |  _  |         \r\n __\\_/\\_/__|___|____/|_|_|_|_  _   _ \r\n| ____|  _ \\_ _|_   _|_ _/ _ \\| \\ | |\r\n|  _| | | | | |  | |  | | | | |  \\| |\r\n| |___| |_| | |  | |  | | |_| | |\\  |\r\n|_____|____/___| |_| |___\\___/|_| \\_|");
            Console.ReadKey();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void InstructionMessage()
        // Viel Bla Bla... Erklärung der Tastenbelegung und so.
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Find the key (");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("O╣");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(") and open the door (");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("▒▒");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(")");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("Key assignment");
            Console.WriteLine("   Up =  W | \u2191");
            Console.WriteLine(" Down =  S | \u2193");
            Console.WriteLine(" Left =  A | Computer says no... (Think of an left arrow here)");
            Console.WriteLine("Right =  D | \u2192");
            Console.ReadKey();
            Console.Clear();
        }

        private void MapSize()
        // Es kommt nicht auf die Größe an... Hab ich gehört! - Hier wir der Spieler aufgefordert die Map Größe zu bestimmen, ja auch TryParse ist mit drin ^^ weil Doofheit der Menschen und so.
        {
            do
            {
                // Hier wird aufgefordert die Raum Größe zwischen 5-15 auszuwählen.
                Console.WriteLine("Please enter a number for the room size. (number between 5-15):");

                if (int.TryParse(Console.ReadLine(), out userInput)) // Überprüft ob die eingegebene Zahl eine Ganzzahl ist.
                {
                    if (userInput >= 5 && userInput <= 15) // Hier wird gecheckt, ob die Eingabe zwischen 5 und 15 liegt.
                    {
                        MAP_SIZE = userInput; // Gib der Karte den Korrekten Wert, Aber nur WENN der Wert Korrekt ist.
                        isValidInput = true; // sobald isValidInput auf true gesetzt wird, wird die Schleife verlassen.
                    }
                    else
                    {
                        Console.WriteLine("Oops, that wasn't right... Try again :("); // Gibt eine Fehlermeldung aus und fordert auf einen neuen Wert einzugeben.
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number."); // Gibt eine Fehlermeldung aus und fordert auf einen neuen Wert einzugeben.
                }
            } while (!isValidInput); // Wird solange wiederholt BIS der User versteht was zutun ist.

            Console.Clear();
        }

        private void DrawMap()
        // Phantasie ist schön aber es zu sehen mach mehr Spaß! Hier wird die Map in die Konsole geprintet (warum heißt die Methode dann eigentlich "Draw" und nicht "Print" o.O), ach ja die Spielerwegung beginnt auch hier. 
        {
            InitializeMap();
            DrawKey();
            DrawDoor();

            while (isRunning) // Wiederholt die Schleife solange das Spiel läuft
            {
                Console.Clear();
                PrintMapAndCharacter();

                ConsoleKeyInfo keyInput = Console.ReadKey();

                HandleUserInput(keyInput); // 
                CheckCharacterPosition(); // Checkt die Position des Spielers
            }
        }

        private void InitializeMap()
        // Hier bekommt die Map langsam Hand und Fuß ähhh Wand und Boden....
        {
            map = new int[MAP_SIZE, MAP_SIZE]; // Initialisiert die Map anhand der Größe die der Spieler eingegeben hat
            for (int y = 0; y < MAP_SIZE; y++) // geht jede Zeile der Map durch
            {
                for (int x = 0; x < MAP_SIZE; x++) // geht jede Zeile durch
                {
                    if (y == 0 || x == 0 || y == MAP_SIZE - 1 || x == MAP_SIZE - 1)
                    // y == 0 (Abfrage: Unten), x == 0 (Abfrage: Links), y == MAP_SIZE - 1 (Abfrage: Oben), x == MAP_SIZE - 1 (Abfrage: Rechts)
                    {
                        map[x, y] = (int)EMapTiles.wall; // Vergibt den Wert einer Wand
                    }
                    else
                    {
                        map[x, y] = (int)EMapTiles.floor; // vergibt den Wert einem Boden
                    }
                }
            }

            playerX = MAP_SIZE / 2; // setzt die X Position des Characters auf die Mitte
            playerY = MAP_SIZE / 2; // setzt die Y Position des Characters auf die Mitte
        }

        private void HandleUserInput(ConsoleKeyInfo keyInput)
        // Push the Button... Hier werden Nägel mit Köpf... äh knöpfen gemacht, oh und die Spieler Position wird Aktualisiert.
        {
            int prevPlayerX = playerX; // Speichert die vergangene X Position des Characters
            int prevPlayerY = playerY; // Speichert die vergangene Y Position des Characters

            switch (keyInput.Key) // Hier wird die Tastenbelegung 
            {
                case ConsoleKey.Escape:
                    isRunning = false;
                    Console.Clear();
                    break;
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    playerY--;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    playerX--;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    playerY++;
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    playerX++;
                    break;
            }

            CheckCharacterPosition();

            HandleKeyCollection();

            if (isKeyCollected && map[playerX, playerY] == (int)EMapTiles.openDoor)
            {
                Console.Clear();
                Console.WriteLine("Congratulations! You have escaped the room!");
                isRunning = false;
            }
        }

        private static void CheckCharacterPosition()
        // Player Position... Check! Hier wird geschaut das der Charackter nicht ausbüchsen kann.
        {
            // Die Zeile prüft ob der Character außerhalb der Mapbegrenzung liegt oder sich auzf der Wand befindet
            if (playerX < 0 || playerX >= MAP_SIZE || playerY < 0 || playerY >= MAP_SIZE || map[playerY, playerX] == (int)EMapTiles.wall)
            {
                // Setzt die Spielerposition in die Map Begrenzung
                playerX = Math.Clamp(playerX, 0, MAP_SIZE - 1);
                playerY = Math.Clamp(playerY, 0, MAP_SIZE - 1);
            }

            HandleKeyCollection();

            if (hasExitedDoor) // Sobald der Character die Tür verlassen hat...
            {
                Console.Clear(); // Wird die Map gelöscht...
                Console.WriteLine("Congratulations! You have escaped the room! ╰(°▽°)╯"); // und die Winning Message erscheint.
                isRunning = false;
            }
        }

        private void PrintMapAndCharacter()
        // Ahhhh hier haben wir das Printen der Map und des Characters
        {
            for (int y = 0; y < MAP_SIZE; y++) // Durchläuft die Zeilen der Map erneut...
            {
                for (int x = 0; x < MAP_SIZE; x++) // auch die Spalten
                {
                    if (y == playerY && x == playerX) // Wenn der Character die aktuelle Position erreicht hat...
                    {
                        Console.Write(playerChar); // Werden die Zeichen geprintet.
                    }
                    else
                    {
                        int mapValue = map[x, y]; // Checkt den Wert der aktuellen Position.
                        if (isKeyCollected && mapValue == (int)EMapTiles.closedDoor) // Wenn der Schlüssel eingesammelt wurde und die Positin eine Geschlossene Tür ist...
                        {
                            Console.Write(mapTileChar[(int)EMapTiles.floor + 1]); // Wird eine Geschlossene Tür geprintet...
                        }
                        else
                        {
                            Console.Write(mapTileChar[mapValue + 1]); // ??
                        }
                    }
                }

                Console.WriteLine(); //Geht zur nächsten Zeile
            }
        }

        private void DrawKey()
        // Hier wird der Schlüssel zufällig auf der Map gespwant - Hier kein Witz, is ne ernste Sache! ;)
        {
            System.Random rnd = new Random(); // Generiert eine Zufällige Zahl

            int keyX;
            int keyY;

            do
            {
                keyX = rnd.Next(1, MAP_SIZE - 1); // Hier wird für den Schlüssel eine zufällige X Position generiert
                keyY = rnd.Next(1, MAP_SIZE - 1); // Das selbe nur für Y
            } while (map[keyX, keyY] != (int)EMapTiles.floor); // Wiederholt dies solange bis eine Position mit Boden gefunden wurde

            map[keyX, keyY] = (int)EMapTiles.key; // Hier wird er geprintet
        }

        private static void HandleKeyCollection()
        // Hier wird dafür gesorgt das der Schlüssel auch eine Funktion bekommt - und zwar das Löschen sobald der Character ihn aufsammelt.
        {
            if (map[playerX, playerY] == (int)EMapTiles.key) // Wenn der Character die selbe Position hat wie der Schlüssel
            {
                isKeyCollected = true; // Setzt den Status des eingesammelten SChlüssels auf true.
                map[playerX, playerY] = (int)EMapTiles.floor; // Entfernt den Schlüssel und macht aus der Position einen Boden.

                OpenDoor(); // Öffnet die Tür (Hoffentlich)
            }
        }

        private static void DrawDoor()
        // Ein Escape Room ohne Tür? Ist wie Brot ohne Nutella...
        {
            System.Random rndDoor = new Random(); // auch hier wird wieder ein zufallswert erstellt um die Position der Tür zu ermitteln.

            int doorX;
            int doorY;

            do
            {
                doorX = rndDoor.Next(0, MAP_SIZE - 2); // Hier wird für die Tür eine zufällige X Position generiert 
                doorY = rndDoor.Next(0, MAP_SIZE - 2); // The same procedure as ... above, Miss Sophie?
            } while (map[doorX, doorY] != (int)EMapTiles.wall); // Wiederholt das solange bis eine freie stelle gefunden wurde.

            map[doorX, doorY] = (int)EMapTiles.closedDoor; // Platziert die geschlossene Tür auf der Map
        }

        private static void OpenDoor()
        // Offen sollte sie am ende auch sein... ansonsten GAME OVER!
        {
            for (int x = 0; x < MAP_SIZE; x++) // Durchläuft die Zeilen der Map ein drittes mal o.O
            {
                for (int y = 0; y < MAP_SIZE; y++) // Durchläuft auch ein drittes mal die Spalten der Map
                {
                    if (map[x, y] == (int)EMapTiles.closedDoor) // Sobald eine Tür gefunden wurde...
                    {
                        map[x, y] = (int)EMapTiles.openDoor; // Wir diese geöffnet und mit einem anderen Symbol gegenzeichnet.
                        return; // Beendet die Methode
                    }
                }
            }
        }
    }
}

