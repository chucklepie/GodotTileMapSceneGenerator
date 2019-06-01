using System;
using System.Linq;
using System.Collections.Generic;
using Chucklepie.GodotTileMap;
using System.Reflection;
using System.IO;
using System.Text;

namespace Chucklepie.TileMapConsole
{
    class Program
    {
        /// <summary>
        /// Sample command line program to use the tilemap builder
        /// Will take various input values and create a scene file for a tileset and tilemap.
        /// If no parameters are supplied, help with be shown.
        /// 
        /// Usage:
        /// <code>TileMapConsole -out myfile.tscn -ir spritesheet.png -cw 10 -ch 10</code>
        /// </summary>
        /// <param name="args">Refer to DisplayHelp</param>
        static void Main(string[] args)
        {
            Header();

            try
            {
                string sceneFormat = GetArgument(args, "-sf");
                string loadSteps = GetArgument(args, "-ls");
                string nodeType = GetArgument(args, "-nt");
                string setType = GetArgument(args, "-st");
                string cellIndex = GetArgument(args, "-ci");
                string nodeName = GetArgument(args, "-nn");
                string cellWidth = GetArgument(args, "-cw");
                string cellHeight = GetArgument(args, "-ch");
                string imageWidth = GetArgument(args, "-iw");
                string imageHeight = GetArgument(args, "-ih");
                string imageFile = GetArgument(args, "-ir");
                string godotWidth = GetArgument(args, "-gw");
                string mapIgnoreIndex = GetArgument(args, "-ms");
                string csvData = GetArgument(args, "-csd");
                string csvFile = GetArgument(args, "-csv");
                string outFile = GetArgument(args, "-out");

                if (outFile==null || ((csvData != null && csvFile != null)))
                {
                    throw new Exception("An output file and either CSV data or a CSV file must be supplied, as follows");
                }

                TileMapBuilder tb = new TileMapBuilder();
                
                if(sceneFormat!=null)
                {
                    tb.SetFormat(Convert.ToInt32(sceneFormat));
                }
                if (loadSteps != null)
                {
                    tb.SetLoadSteps(Convert.ToInt32(loadSteps));
                }
                if(nodeType!=null)
                {
                    tb.SetNodeType(nodeType);
                }
                if (setType != null)
                {
                    tb.SetTileSetType(setType);
                }
                if (nodeName!=null)
                {
                    tb.SetNodeName(nodeName);
                }
                if(cellIndex!=null)
                {
                    tb.SetTileSheetStartIndex(Convert.ToUInt32(cellIndex));
                }
                if(cellHeight!=null || cellWidth !=null)
                {
                    //will error if either not supplied
                    tb.SetCellSizePixels(Convert.ToUInt32(cellWidth), Convert.ToUInt32(cellHeight));
                }
                if(imageWidth!=null | imageHeight!=null)
                {
                    tb.SetTileSheetSizeUnits(Convert.ToUInt32(imageWidth), Convert.ToUInt32(imageHeight));
                }
                if(imageFile!=null)
                {
                    tb.SetTileSheet(imageFile);
                }
                if(godotWidth!=null)
                {
                    tb.SetGodotTileWidth(Convert.ToUInt32(godotWidth));
                }
                if(mapIgnoreIndex != null)
                {
                    tb.SetMapDataEmptyCellIndex(Convert.ToInt32(mapIgnoreIndex));
                }

                try
                {
                    int[,] data = GetInputData(tb, csvData, csvFile);
                    tb.SetMapData(data);

                    string sceneFile = tb.Build();
                    Console.WriteLine(sceneFile);
                    File.WriteAllText(outFile, sceneFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed: {e.Message}");
                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                DisplayHelp(e.Message);
            }
        }

        /// <summary>
        /// Take the supplied data and transform to the required array format required by the builder
        /// CSV file data should be comma separated for each map cell and a newline for each row, e.g.
        ///     1,1,2,3
        ///     4,2,1,3
        ///     
        ///  CSV string data supplied should be comma separated as CSV file but each row separated by pipe ('|')
        ///  The map may be 0 or 1 indexed for the first item so ensure the start index value is set
        /// </summary>
        /// <param name="csvData">Tilemap data in string format. More for debugging but may be useful.</param>
        /// <param name="csvFile">Tilemap data as a file</param>
        /// <returns></returns>
        private static int[,] GetInputData(TileMapBuilder tb, string csvData, string csvFile)
        {
            string[] rowData;
            if (csvData == null && csvFile == null)
            {
                return null;
            }
            if (csvData != null)
            {
                rowData = csvData.Split('|');
            }
            else if(csvFile!=null)
            {
                rowData = File.ReadAllLines(csvFile);
            } else
            {
                return null;
            }

            int[,] data;
            try
            {
                data = tb.TransformInputData(rowData);
            }
            catch (Exception e)
            {
                throw e;
            }
            return data;
        }

        /// <summary>
        /// Output basic text as first thing to the screen
        /// </summary>
        private static void Header()
        {
            string assemblyVersion = "x.x.x";
            string assemblyLib = "x.x.x";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                assemblyVersion = assembly.GetName().Version.ToString();
                assemblyLib = typeof(TileMapBuilder).Assembly.GetName().Version.ToString();
            }
            finally
            {
                Console.WriteLine($"Godot TileMap scene generator V{assemblyVersion}, Builder V{assemblyLib}{Environment.NewLine}{Environment.NewLine}");
            }
        }

        private static void DisplayHelp(string message)
        {
            StringBuilder sb = new StringBuilder(message).AppendLine();

            sb.AppendLine($"Required:");
            sb.AppendLine($"-out filename.tscn : the file to store the generated scene file");
            sb.AppendLine();
            sb.AppendLine($"Optional:");
            sb.AppendLine($"-nn name     : Name to give your node when you add it. Defaults to 'TileMap'");
            sb.AppendLine();
            sb.AppendLine($"Tileset:");
            sb.AppendLine($" Tileset represents the individual graphic items in the tileset. They are a subset of the sheet graphic supplied.");
            sb.AppendLine($" Typically supply a graphic (tilesheet/spritesheet/atlas) containing a grid of the same sized items, each one ");
            sb.AppendLine($" an indvidual cell displayed in the tilemap.");
            sb.AppendLine($" Note: be care when more than a 100 or so as Godot can crash. You need to update the project 'queue size'");
            sb.AppendLine($"  to something larger, e.g. 102400, or unless the 'bug' has been fixed consuming so much memory on tilesets.");
            sb.AppendLine();
            sb.AppendLine($" -ir filename : the graphic representing the tiles and Godot will import as an asset");
            sb.AppendLine($" -ci <int>    : when matching map data to tiles, what is the first value. Normally 0 or 1. Defaults to 0");
            sb.AppendLine($" -cw <int>    : number of cells (tile graphics) across the width of the tileset graphic");
            sb.AppendLine($" -ch <int>    : as -cw but for height in cells");
            sb.AppendLine($" -iw <int>    : width of each cell in pixels. Defaults to 32");
            sb.AppendLine($" -ih <int>    : as -w for height. Defaults to 32");
            sb.AppendLine($"");
            sb.AppendLine($"Tilemap:");
            sb.AppendLine($" This is for the generation of the tilemap, i.e. the actual map you see in Godot.");
            sb.AppendLine($" If you are just creating a blank tilemap with all the assets imported and are drawing manually");
            sb.AppendLine($" then simply do not supply any parameters.");
            sb.AppendLine();
            sb.AppendLine($" -csv file.csv: comma separated file, each row represented by a newline. Values are indexes to the sheet");
            sb.AppendLine($" -csd <data>  : more for debugging, use this to supply csv data as a string. Pipe separates rows, e.g. '1,2,3|4,5,6'");
            sb.AppendLine($" -ms <int>    : What number in the data represents empty cell (these are skipped). Defaults to -1 for none");
            sb.AppendLine();
            sb.AppendLine($"Godot internals (change only if Godot changes this or you think you need to):");
            sb.AppendLine($" -sf <int>    : part of the standard header 'format'. Defaults to 2");
            sb.AppendLine($" -ls <int>    : part of the standard header 'load_steps'. Defaults to 3.");
            sb.AppendLine($" -gw <int>    : maximum columns in a tilemap. Defaults to 65536");
            sb.AppendLine($" -nt name     : class name of TileMap node. Defaults to TileMap. Change if you have custom tilemap");
            sb.AppendLine($" -st name     : class name of TileSet node. Defaults to TileSet. Change if you have a custom tileset");
            sb.AppendLine();
            sb.AppendLine("Example usage:");
            sb.AppendLine(" TileMapConsole -out myscene.tscn -ir marioworld.bmp -iw 16 -ih 16 -cw 256 -ch 32 -csv marioworld.csv -ms 1");
            sb.AppendLine(" This will create a scene using a mario world csv 256x10 units, with each tile being 16x16 pixels and in the csv 1 represents empty cell");
            Console.WriteLine(sb.ToString());

        }

        /// <summary>
        /// Basic command line argument parser
        /// </summary>
        /// <param name="args"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private static string GetArgument (IEnumerable<string> args, string option, string defaultValue=null)
        {
            string v = args.SkipWhile(i => i != option).Skip(1).Take(1).FirstOrDefault();
            if(v==null)
            {
                v = defaultValue;
            }
            return v;
        }
    }
}
