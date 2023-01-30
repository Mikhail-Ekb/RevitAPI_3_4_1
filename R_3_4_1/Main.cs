using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitAPI_3_4_1
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
{
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string wallinfo = string.Empty;

            var walls = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .Cast<Wall>()
                .ToList();
            
            foreach (Wall wall in walls)
            {
                double volumeWall;
                
                Parameter volumeParametr = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (volumeParametr.StorageType == StorageType.Double)
                {
                    volumeWall = volumeParametr.AsDouble();
                    volumeWall = UnitUtils.ConvertFromInternalUnits(volumeWall, DisplayUnitType.DUT_CUBIC_METERS);
                    wallinfo += $"{wall.WallType}/t{volumeWall}{Environment.NewLine}";
                }
                
            }

            var saveDialog = new System.Windows.Forms.SaveFileDialog
            {
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "All files (*.*)|*.*",
                FileName = "roomInfo.csv",
                DefaultExt = ".csv"
            };

            string selectedFilePath = string.Empty;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = saveDialog.FileName;
            }

            if (string.IsNullOrEmpty(selectedFilePath))
                return Result.Cancelled;

            File.WriteAllText(selectedFilePath, wallinfo);

        return Result.Succeeded;
    }
}
}
