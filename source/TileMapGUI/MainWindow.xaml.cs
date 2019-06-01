using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Chucklepie.GodotTileMap;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Chucklepie.TileMapGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateScene_Click(object sender, RoutedEventArgs e)
        {
            if(ValidateForm())
            {
                GenerateSceneFile();
            }
        }

        private void GenerateSceneFile()
        {
            string[] rowData;
            TileMapBuilder tb = new TileMapBuilder();

            if (CSVString.Text.Trim()!="")
            {
                rowData = CSVString.Text.Trim().Split('|');
            }
            else if(CSVImport.Text.Trim()!="")
            {
                rowData = File.ReadAllLines(CSVImport.Text.Trim());
            }
            else
            {
                rowData = null;
            }

            try
            {
                tb.SetMapData(tb.TransformInputData(rowData));
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed: {e.Message}");
            }

            string sceneFile = tb
                .SetCellSizePixels(uint.Parse(CellWidth.Text), uint.Parse(CellHeight.Text))
                .SetTileSheetSizeUnits(uint.Parse(UnitWidth.Text), uint.Parse(UnitHeight.Text))
                .SetTileSheetStartIndex(uint.Parse(StartCellIndex.Text))
                .SetTileSheet(ImportSheet.Text)
                .SetFormat(int.Parse(InternalFormat.Text))
                .SetLoadSteps(int.Parse(InternalSteps.Text))
                .SetNodeType(InternalMapClass.Text)
                .SetTileSetType(InternalTileClass.Text)
                .SetGodotTileWidth(uint.Parse(InternalMapWidth.Text))
                .SetMapDataEmptyCellIndex(int.Parse(MapIgnoreCell.Text))
                .SetNodeName(NodeName.Text)
                .Build();

            try
            {
                File.WriteAllText(OutFile.Text, sceneFile);
                MessageBox.Show($"File written to:\n {OutFile.Text}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to write output file: {OutFile.Text}\n\n{e.Message}");
            }
        }

        private bool ValidateForm()
        {
            if(String.IsNullOrEmpty(OutFile.Text.Trim()))
            {
                MessageBox.Show("You must specify an output file to create");
                return false;
            }

            if (!IsNumeric(CellHeight.Text) || !IsNumeric(CellWidth.Text) || !IsNumeric(UnitHeight.Text)
                || !IsNumeric(UnitWidth.Text))
            {
                MessageBox.Show("Unit and Cell Width/Height values must be numeric.");
                return false;
            }

            if (CSVImport.Text != "" && CSVString.Text != "")
            {
                MessageBox.Show("Cannot select CSV file and data");
                return false;
            } else if(CSVImport.Text.Trim()!="")
            {
                if(!File.Exists(CSVImport.Text))
                {
                    MessageBox.Show("CSV File entered does not exist.");
                    return false;
                }
            }

            if (String.IsNullOrEmpty(ImportSheet.Text.Trim()))
            {
                if(MessageBox.Show("You have not specified a import sheet. The scene will probably not load. Are you sure you wish to continue?","No sheet specified",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.No)
                {
                    return false;
                }
            } else
            {
                if (int.Parse(UnitWidth.Text) < 1 || int.Parse(UnitHeight.Text) < 1 || int.Parse(CellWidth.Text) < 1 || int.Parse(CellHeight.Text) < 1)
                {
                    if (MessageBox.Show("You have specified zero or negative values for unit/cell sizes. This is probably wrong and will fail. Are you sure you wish to continue?", "No sheet specified", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsNumeric(string value)
        {
            bool var = value.All(char.IsNumber);
            return var;
        }
        private void CSVStringSampleButton_Click(object sender, RoutedEventArgs e)
        {
            CSVString.Text = "1,2,3|4,5,6";
        }

        private void CSVIMportSelectButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog d = new System.Windows.Forms.OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true,
                DefaultExt = "*.csv",
                Filter = "csv files (*.csv)|*.csv|txt files (*.txt)|*.txt|all files (*.*)|*.*",
                Title = "Select file"
            };

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CSVImport.Text = d.FileName;
            }
        }

        private void ImportSheetSelectButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            System.Windows.Forms.OpenFileDialog d = new System.Windows.Forms.OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true,
                DefaultExt = "*.bmp",
                Filter = "bmp files (*.bmp)|*.bmp|bmp files (*.png)|*.png|bmp files (*.jpg)|*.jpg|all files (*.*)|*.*",
                Title = "Select file"
            };

            try
            {
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName= d.FileName;
                    TileSheetImage.Source = new BitmapImage(new Uri(fileName));
                }
                ImportSheet.Text = System.IO.Path.GetFileName(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load image '{fileName}' due to:\n\n{ex.Message}");
            }
        }

        private void SceneFileButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog d = new System.Windows.Forms.OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = false,
                DefaultExt = "*.tscn",
                Filter = "tscn files (*.tscn)|*.tscn|all files (*.*)|*.*",
                Title = "Select file"
            };

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OutFile.Text = d.FileName;
            }
        }
    }
}
