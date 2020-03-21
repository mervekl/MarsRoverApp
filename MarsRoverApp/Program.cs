using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MarsRoverApp
{
    class Program
    {
        private static string _currentFacing;
        private static (int, int) _currentCoordinates;
        private static (int, int) _plateauCoordinates;
        private const string PLATEAU_PATTERN = @"^[0-9]+\s[0-9]+$";
        private const string COORDINATE_PATTERN = @"^[0-9]+\s[0-9]+\s(N|W|E|S)$";
        private const string DIRECTION_PATTERN = @"^(L|M|R)+$";

        static void Main(string[] args)
        {
            Console.WriteLine("*** Welcome to Mars Router App ***");

            var options = new Options();

            if (args.Length == 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Please enter plateau coordinates:");
                var input = Console.ReadLine();

                while (!IsValidInput(input, PLATEAU_PATTERN))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Please enter valid coordinates:");
                    input = Console.ReadLine();
                }
                options.PlateauCoordinates = input;

                Console.WriteLine("");
                Console.WriteLine("Please enter first rover coordinates and facing value:");
                input = Console.ReadLine();

                while (!IsValidInput(input, COORDINATE_PATTERN))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Please enter valid coordinates and facing value:");
                    input = Console.ReadLine();
                }
                options.FirstRoverCoordinatesAndFacing = input;

                Console.WriteLine("");
                Console.WriteLine("Please enter first rover directions:");

                input = Console.ReadLine();
                while (!IsValidInput(input, DIRECTION_PATTERN))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Please enter valid directions:");
                    input = Console.ReadLine();
                }
                options.FirstRoverDirections = input.Trim();

                Console.WriteLine("");
                Console.WriteLine("Please enter second rover coordinates and facing value:");
                input = Console.ReadLine();

                while (!IsValidInput(input, COORDINATE_PATTERN))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Please enter valid coordinates and facing value:");
                    input = Console.ReadLine();
                }
                options.SecondRoverCoordinatesAndFacing = input;

                Console.WriteLine("");
                Console.WriteLine("Please enter second rover directions:");

                input = Console.ReadLine();
                while (!IsValidInput(input, DIRECTION_PATTERN))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Please enter valid directions:");
                    input = Console.ReadLine();
                }
                options.SecondRoverDirections = input.Trim();
            }
            else
            {
                if (args.Length != 5)
                {
                    Console.WriteLine("Invalid count of args detected!");
                    Console.WriteLine("App is terminating..");
                    return;
                }

                options = Parse(args);

                if (!IsValidInput(options.PlateauCoordinates, PLATEAU_PATTERN))
                {
                    Console.WriteLine("Invalid plateau coordinate input detected! --> " + options.PlateauCoordinates);
                    Console.WriteLine("App is terminating..");
                    return;
                }

                if (!IsValidInput(options.FirstRoverCoordinatesAndFacing, COORDINATE_PATTERN))
                {
                    Console.WriteLine("Invalid rover coordinate input detected! --> " + options.FirstRoverCoordinatesAndFacing);
                    Console.WriteLine("App is terminating..");
                    return;
                }

                if (!IsValidInput(options.FirstRoverDirections, DIRECTION_PATTERN))
                {
                    Console.WriteLine("Invalid rover direction input detected! --> " + options.FirstRoverDirections);
                    Console.WriteLine("App is terminating..");
                    return;
                }

                if (!IsValidInput(options.SecondRoverCoordinatesAndFacing, COORDINATE_PATTERN))
                {
                    Console.WriteLine("Invalid rover coordinate input detected! --> " + options.SecondRoverCoordinatesAndFacing);
                    Console.WriteLine("App is terminating..");
                    return;
                }

                if (!IsValidInput(options.SecondRoverDirections, DIRECTION_PATTERN))
                {
                    Console.WriteLine("Invalid rover direction input detected! --> " + options.SecondRoverDirections);
                    Console.WriteLine("App is terminating..");
                    return;
                }
            }

            // Inputs are validated and ready to calculate output
            GenerateOutput(options);
            Console.WriteLine("Finished!");
            Console.ReadKey();
        }

        private static void GenerateOutput(Options options)
        {
            var result = new List<string>();

            // Specify plateau coordinates
            var input = options.PlateauCoordinates.Split();
            RenderPlateau(Convert.ToInt32(input.GetValue(0)), Convert.ToInt32(input.GetValue(1)));

            // Specify current coordinates and facing of the first rover
            input = options.FirstRoverCoordinatesAndFacing.Split();
            _currentCoordinates = (Convert.ToInt32(input.GetValue(0)), Convert.ToInt32(input.GetValue(1)));
            _currentFacing = input.GetValue(2).ToString();

            // Execute first rover directions
            ExecuteRover(options.FirstRoverDirections);
            var locationStr = _currentCoordinates.Item1 + " " + _currentCoordinates.Item2 + " " + _currentFacing;
            if (_plateauCoordinates.Item1 < _currentCoordinates.Item1 || _plateauCoordinates.Item2 < _currentCoordinates.Item2)
                result.Add("First rover is out of plateau! --> " + locationStr);
            else
                result.Add(locationStr);

            // Specify current coordinates and facing of the second rover
            input = options.SecondRoverCoordinatesAndFacing.Split();
            _currentCoordinates = (Convert.ToInt32(input.GetValue(0)), Convert.ToInt32(input.GetValue(1)));
            _currentFacing = input.GetValue(2).ToString();

            // Execute second rover directions
            ExecuteRover(options.SecondRoverDirections);
            locationStr = _currentCoordinates.Item1 + " " + _currentCoordinates.Item2 + " " + _currentFacing;
            if (_plateauCoordinates.Item1 < _currentCoordinates.Item1 || _plateauCoordinates.Item2 < _currentCoordinates.Item2)
                result.Add("Second rover is out of plateau! --> " + locationStr);
            else
                result.Add(locationStr);

            Console.WriteLine();
            Console.WriteLine("-- Results --");
            // Output
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        /// <summary>
        /// Returns last facing of the rover
        /// </summary>
        public static void Spin(string direction)
        {
            switch (_currentFacing)
            {
                case "N":
                    if (direction == "L")
                        _currentFacing = "W";
                    else
                        _currentFacing = "E";
                    break;
                case "S":
                    if (direction == "L")
                        _currentFacing = "E";
                    else
                        _currentFacing = "W";
                    break;
                case "W":
                    if (direction == "L")
                        _currentFacing = "S";
                    else
                        _currentFacing = "N";
                    break;
                case "E":
                    if (direction == "L")
                        _currentFacing = "N";
                    else
                        _currentFacing = "S";
                    break;
            }
        }

        /// <summary>
        /// Returns last coordinate of the rover
        /// </summary>
        public static void Move()
        {
            switch (_currentFacing)
            {
                case "N":
                    _currentCoordinates = (_currentCoordinates.Item1, _currentCoordinates.Item2 + 1);
                    break;
                case "S":
                    _currentCoordinates = (_currentCoordinates.Item1, _currentCoordinates.Item2 + -1);
                    break;
                case "W":
                    _currentCoordinates = (_currentCoordinates.Item1 - 1, _currentCoordinates.Item2);
                    break;
                case "E":
                    _currentCoordinates = (_currentCoordinates.Item1 + 1, _currentCoordinates.Item2);
                    break;
            }
        }

        private static void ExecuteRover(string roverDirections)
        {
            for (int i = 0; i < roverDirections.Length; i++)
            {
                var item = roverDirections.Substring(i, 1);
                if (item.Equals("M"))
                    Move();
                else
                    Spin(item);
            }
        }

        public static void RenderPlateau(int xCordinate, int yCordinate)
        {
            _plateauCoordinates = (xCordinate, yCordinate);
        }


        public static bool IsValidInput(string input, string pattern)
        {
            var m = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
                return true;

            return false;
        }


        public static Options Parse(string[] args)
        {
            var options = new Options();
            for (var i = 0; i < args.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        options.PlateauCoordinates = args[i];
                        break;

                    case 1:
                        options.FirstRoverCoordinatesAndFacing = args[i];
                        break;

                    case 2:
                        options.FirstRoverDirections = args[i];
                        break;

                    case 3:
                        options.SecondRoverCoordinatesAndFacing = args[i];
                        break;

                    case 4:
                        options.SecondRoverDirections = args[i];
                        break;
                }
            }

            return options;
        }

    }
}
