## Overview OF Godot Tilemap scene generator

Refer to home page and samples for documentation on the tools. The GUI and Console are really just starter projects for running the library, but may all be that is required.

- TileMapGUI: WPF GUI front-end to the tilemap generator
- TileMapConsole: Command line front-end to the tilemap generator
- GodotTileMapCreator: Class library that allows you to create tilemap scene files from tilesheets and CSV map files

## Getting Started

Visual Studio: Download, open the solution file and build
Others: Run <code>dotnet build</code>

Pre-requisites: 
- GodotTileMapCreator: Build with .net core 2.2, but any version of .net core or any full .Net should work if you update project
- TileMapConsole: As GodotTileMapCreator
- TileMapGUI: Built with .net core 3.0 preview, but any version 3.0 onwards of .net core or any full .Net should work.

Alternatively, Windows binaries can be found here:

## Using class library
Assuming you have read the home page/sample introduction and know how to use the software and the different parameters, using the class library GodotTileMapCreator is simple.

<code>TileMapBuilder tb = new TileMapBuilder();</code>

If you are supplying a pre-made map file then:
<code>
            int[,] mapData = { {1,2,3 }, {4,5,6 } };
            TileMapBuilder tb = new TileMapBuilder(mapData);
</code>

The class is a Builder pattern with method chaining. All methods are either setters of the data or the actual build method, with the exception of one helper method described later. So simply apply all methods required and and with calling the Build() method to return a string containing your entire scene file. The console/gui applications take this string and save it to a file.

The following methods can be called to apply data. Note the defaults as many may not be required:

| Method        | Parameters           | Default  | Details |
| ------------- |-------------| ----- | ----- |
| SetMapData      | int[,] mapData | null | Add map CSV data if not supplied in constructor. Or leave out if creating tileset only with no map |
|  TransformInputData | string[] rows |   | If you have a single array of map data, with each row containing a single row of CSV data | | SetMapDataEmptyCellIndex      | int emptyIndex  | -1 | Which cell is to be ignored (blank cell). Default means do not ignore any |
| SetNodeName      | string name | MapTile | Give the node a unique name. Can be changed inside the IDE |
| SetTileSheetStartIndex      | int start | 0 | For map creation, does the first index value in the CSV presume 0 or 1 |
| SetCellSizePixels  | uint w, uint h  | 32,32  | Size in pixels of each cell in the tilesheet  |
| SetTileSheet | string filename |   | Name of tilesheet file. Do not use path, but instead relative path from the scene file  |
| SetTileSheetSizeUnits  |  uint w, uint h  | 0,0  | How many columns and rows of cells are in the tilesheet  |
| Build  |   |   | Construct the scene file and return a string  |
| SetFormat      | int format | 2 | Internal for changing header format |
| SetLoadSteps      | int load_steps | 3 | Internal for changing header load step |
| SetNodeType      | string name | TileMap | Internal for changing class type |
| SetTileSetType      | string name | TileSet | Internal for changing class type |
| SetGodotTileWidth  | uint w  | 65536  | Internal for setting number of columns in a tilemap row  |
in the form of a string, e.g. "1,2,3,4,5". This will convert it to the int[,] format required by the constructor or SetMapData method  |

All methods except Build() and TransformInputData returns 'this' to allow method chaining.

### Example 1: Create tilemap with no map data using all defaults and minimal usage:

<code>
TileMapBuilder tb = new TileMapBuilder();
string scene=tb
    .SetTileSheet("mario.bmp")
    .SetTileSheetSizeUnits(16, 10)
    .Build();
</code>

### Example 2: Create tilemap with map data using all defaults and minimal usage:

<code>
int[,] mapData = { {1,2,3 }, {4,5,6 } };
TileMapBuilder tb = new TileMapBuilder(mapData);
string scene=tb
    .SetTileSheet("mario.bmp")
    .SetTileSheetSizeUnits(16, 10)
    .Build();
</code>
