using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_FinalTask
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class FinalTask : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            using (Transaction ts = new Transaction(doc, "Создание помещений"))
            {
                int roomNum = 1;
                int levelNum = 1;

                ts.Start();

                foreach (Level level in listLevel)
                {
                    PlanTopology topology = doc.get_PlanTopology(level);
                    PlanCircuitSet circuitSet = topology.Circuits;
                    foreach (PlanCircuit circuit in circuitSet)
                    {
                        if (!circuit.IsRoomLocated)
                        {
                            Room room = doc.Create.NewRoom(null, circuit);
                            room.Number = $"{roomNum}_{levelNum}";
                        }

                        roomNum++;
                    }

                    levelNum++;
                }

                ts.Commit();
            }

            return Result.Succeeded;
        }
    }
}
