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

        private const int FLOOR_TILE = 0;
        private const int WALL_TILE = 1;
        private const int KEY_TILE = 2;
        private const int OPEN_DOOR_TILE = 3;
        private const int CLOSED_DOOR_TILE = 4;

        private const string FLOOR_CHAR = "  ";
        private const string WALL_CHAR = "██";
        private const string KEY_CHAR = "O╣";
        private const string OPEN_DOOR_CHAR = "▒▒";
        private const string CLOSED_DOOR_CHAR = "▓▓";

        public void Run()
        {
            WelcomeMessage();
            InstructionMessage();
            MapSize();
            DrawMap();
        }

        /// <summary>
        /// Bisschen was fürs Auge.
        /// </summary>
        private void WelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" _____ ____   ____    _    ____  _____   ____   ___   ___  __  __ \r\n| ____/ ___| / ___|  / \\  |  _ \\| ____| |  _ \\ / _ \\ / _ \\|  \\/  |\r\n|  _| \\___ \\| |     / _ \\ | |_) |  _|   | |_) | | | | | | | |\\/| |\r\n| |___ ___) | |___ / ___ \\|  __/| |___  |  _ <| |_| | |_| | |  | |\r\n|_____|____/ \\____/_/   \\_\\_|   |_____| |_| \\_\\\\___/ \\___/|_|  |_|");
            Console.WriteLine("__        _____ ____  _   _          \r\n\\ \\      / /_ _/ ___|| | | |         \r\n \\ \\ /\\ / / | |\\___ \\| |_| |         \r\n  \\ V  V /  | | ___) |  _  |         \r\n __\\_/\\_/__|___|____/|_|_|_|_  _   _ \r\n| ____|  _ \\_ _|_   _|_ _/ _ \\| \\ | |\r\n|  _| | | | | |  | |  | | | | |  \\| |\r\n| |___| |_| | |  | |  | | |_| | |\\  |\r\n|_____|____/___| |_| |___\\___/|_| \\_|");
            Console.ReadKey();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Viel Bla Bla... Erklärung der Tastenbelegung und so.
        /// </summary>
        private void InstructionMessage()
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

        /// <summary>
        /// Hier wird aufgefordert die Raum Größe zwischen 5-15 auszuwählen.
        /// </summary>
        private void MapSize()
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

        /// <summary>
        /// Phantasie ist schön aber es zu sehen mach mehr Spaß! Hier wird die Map in die Konsole geprintet (warum heißt die Methode dann eigentlich "Draw" und nicht "Print" o.O), ach ja die Spielerwegung beginnt auch hier.
        /// </summary>
        private void DrawMap()
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

        /// <summary>
        /// Hier bekommt die Map langsam Hand und Fuß ähhh Wand und Boden....
        /// </summary>
        private void InitializeMap()
        {
            map = new int[MAP_SIZE, MAP_SIZE]; // Initialisiert die Map anhand der Größe die der Spieler eingegeben hat
            for (int y = 0; y < MAP_SIZE; y++) // geht jede Zeile der Map durch
            {
                for (int x = 0; x < MAP_SIZE; x++) // geht jede Zeile durch
                {
                    if (y == 0 || x == 0 || y == MAP_SIZE - 1 || x == MAP_SIZE - 1)
                    // y == 0 (Abfrage: Unten), x == 0 (Abfrage: Links), y == MAP_SIZE - 1 (Abfrage: Oben), x == MAP_SIZE - 1 (Abfrage: Rechts)
                    {
                        map[x, y] = WALL_TILE; // Vergibt den Wert einer Wand
                    }
                    else
                    {
                        map[x, y] = FLOOR_TILE; // vergibt den Wert einem Boden
                    }
                }
            }

            playerX = MAP_SIZE / 2; // setzt die X Position des Characters auf die Mitte
            playerY = MAP_SIZE / 2; // setzt die Y Position des Characters auf die Mitte
        }

        /// <summary>
        /// Push the Button... Hier werden Nägel mit Köpf... äh knöpfen gemacht, oh und die Spieler Position wird Aktualisiert.
        /// </summary>
        /// <param name="keyInput">stellt den aufgenommenen Schlüssel dar.</param>
        private void HandleUserInput(ConsoleKeyInfo keyInput)
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
                    if (map[playerY - 1, playerX] != WALL_TILE) // Prüfe, ob die nächste Position eine Wand ist
                        playerY--;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (map[playerY, playerX - 1] != WALL_TILE)
                        playerX--;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (map[playerY + 1, playerX] != WALL_TILE)
                        playerY++;
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (map[playerY, playerX + 1] != WALL_TILE)
                        playerX++;
                    break;
            }

            CheckCharacterPosition();

            HandleKeyCollection();

            if (isKeyCollected && map[playerY, playerX] == OPEN_DOOR_TILE)
            {
                Console.Clear();
                Console.WriteLine("Congratulations! You have escaped the room!");
                isRunning = false;
            }
        }

        /// <summary>
        /// Player Position... Check! Hier wird geschaut das der Charackter nicht ausbüchsen kann.
        /// </summary>
        private static void CheckCharacterPosition()
        {
            // Die Zeile prüft ob der Character außerhalb der Mapbegrenzung liegt oder sich auf der Wand befindet
            if (playerX < 0 || playerX >= MAP_SIZE || playerY < 0 || playerY >= MAP_SIZE || map[playerY, playerX] == WALL_TILE)
            {
                // Setzt die Spielerposition in die Map Begrenzung
                playerX = Math.Clamp(playerX, 0, MAP_SIZE - 1); // MUSS GEÄNDERT WERDEN, SPIELER KANN AUF DER WAND LAUFEN!
                playerY = Math.Clamp(playerY, 0, MAP_SIZE - 1); // MUSS GEÄNDERT WERDEN, SPIELER KANN AUF DER WAND LAUFEN!
            }

            HandleKeyCollection();

            if (hasExitedDoor) // Sobald der Character die Tür verlassen hat...
            {
                Console.Clear(); // Wird die Map gelöscht...
                Console.WriteLine("Congratulations! You have escaped the room! ╰(°▽°)╯"); // und die Winning Message erscheint.
                isRunning = false;
            }
        }

        /// <summary>
        /// Ahhhh hier haben wir das Printen der Map und des Characters
        /// </summary>
        private void PrintMapAndCharacter()
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
                        if (isKeyCollected && mapValue == CLOSED_DOOR_TILE) // Wenn der Schlüssel eingesammelt wurde und die Positin eine Geschlossene Tür ist...
                        {
                            Console.Write(FLOOR_CHAR); // Wird eine Geschlossene Tür geprintet...
                        }
                        else
                        {
                            Console.Write(GetMapTileChar(mapValue)); // ??
                        }
                    }
                }

                Console.WriteLine(); //Geht zur nächsten Zeile
            }
        }

        /// <summary>
        /// Hier wird der Schlüssel zufällig auf der Map gespwant - Hier kein Witz, is ne ernste Sache! ;)
        /// </summary>
        private void DrawKey()
        {
            System.Random rnd = new Random(); // Generiert eine Zufällige Zahl

            int keyX;
            int keyY;

            do
            {
                keyX = rnd.Next(1, MAP_SIZE - 1); // Hier wird für den Schlüssel eine zufällige X Position generiert
                keyY = rnd.Next(1, MAP_SIZE - 1); // Das selbe nur für Y
            } while (map[keyX, keyY] != FLOOR_TILE); // Wiederholt dies solange bis eine Position mit Boden gefunden wurde

            map[keyX, keyY] = KEY_TILE; // Hier wird er geprintet
        }

        /// <summary>
        /// Hier wird dafür gesorgt das der Schlüssel auch eine Funktion bekommt - und zwar das Löschen sobald der Character ihn aufsammelt.
        /// </summary>
        private static void HandleKeyCollection()
        {
            if (map[playerX, playerY] == KEY_TILE) // Wenn der Character die selbe Position hat wie der Schlüssel
            {
                isKeyCollected = true; // Setzt den Status des eingesammelten SChlüssels auf true.
                map[playerX, playerY] = FLOOR_TILE; // Entfernt den Schlüssel und macht aus der Position einen Boden.

                OpenDoor(); // Öffnet die Tür (Hoffentlich)
            }
        }

        /// <summary>
        /// Ein Escape Room ohne Tür? Ist wie Brot ohne Nutella...
        /// </summary>
        private static void DrawDoor()
        {
            System.Random rndDoor = new Random(); // auch hier wird wieder ein zufallswert erstellt um die Position der Tür zu ermitteln.

            int doorX;
            int doorY;

            do
            {
                doorX = rndDoor.Next(0, MAP_SIZE - 2); // Hier wird für die Tür eine zufällige X Position generiert 
                doorY = rndDoor.Next(0, MAP_SIZE - 2); // The same procedure as ... above, Miss Sophie?
            } while (map[doorX, doorY] != WALL_TILE); // Wiederholt das solange bis eine freie stelle gefunden wurde.

            map[doorX, doorY] = CLOSED_DOOR_TILE; // Platziert die geschlossene Tür auf der Map
        }

        /// <summary>
        /// Offen sollte sie am ende auch sein... ansonsten GAME OVER!
        /// </summary>
        private static void OpenDoor()
        {
            for (int x = 0; x < MAP_SIZE; x++) // Durchläuft die Zeilen der Map ein drittes mal o.O
            {
                for (int y = 0; y < MAP_SIZE; y++) // Durchläuft auch ein drittes mal die Spalten der Map
                {
                    if (map[x, y] == CLOSED_DOOR_TILE) // Sobald eine Tür gefunden wurde...
                    {
                        map[x, y] = OPEN_DOOR_TILE; // Wir diese geöffnet und mit einem anderen Symbol gegenzeichnet.
                        return; // Beendet die Methode
                    }
                }
            }
        }

        private string GetMapTileChar(int mapValue)
        {
            switch (mapValue)
            {
                case FLOOR_TILE:
                    return FLOOR_CHAR;
                case WALL_TILE:
                    return WALL_CHAR;
                case KEY_TILE:
                    return KEY_CHAR;
                case OPEN_DOOR_TILE:
                    return OPEN_DOOR_CHAR;
                case CLOSED_DOOR_TILE:
                    return CLOSED_DOOR_CHAR;
                default:
                    return "??";
            }
        }
    }
}

