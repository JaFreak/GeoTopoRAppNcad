using Teigha.Runtime;
using HostMgd.EditorInput;
using Teigha.DatabaseServices;


namespace WorkWithDimensions
{
    public class ChangeDimensionText
    {
        [CommandMethod("ChangeDimText")]
        [CommandMethod("ДополнитьРазмерныйТекст")]
        public void ChangeDimText()
        {
            HostMgd.ApplicationServices.Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database dB = doc.Database;
            Editor ed = doc.Editor;

            TypedValue[] filterListForSelectDimensions = new TypedValue[1];
            filterListForSelectDimensions[0] = new TypedValue(0, "DIMENSION");
            SelectionFilter filterForSelectDimensions = new SelectionFilter(filterListForSelectDimensions);

            PromptStringOptions prefOptions = new PromptStringOptions("\nВведите префикс");
            prefOptions.AllowSpaces = true;
            PromptStringOptions sufOptions = new PromptStringOptions("\nВведите суффикс");
            sufOptions.AllowSpaces = true;

            PromptResult pref = ed.GetString(prefOptions);
            PromptResult suf = ed.GetString(sufOptions);

            // Starts a new transaction with the Transaction Manager
            using (Transaction trans = dB.TransactionManager.StartTransaction())
            {
                ed.WriteMessage("\nВыберите размеры для редактирования");
                PromptSelectionResult selDim = ed.GetSelection(filterForSelectDimensions);
                if (selDim.Status != PromptStatus.OK)
                {
                    ed.WriteMessage("\nНет выбранных объектов");
                    return;
                }
                ObjectIdCollection selectedDimsId = new ObjectIdCollection(selDim.Value.GetObjectIds());
                for (int i = 0; i < selectedDimsId.Count; i++)
                {

                    DBObject exampleOfDim = selectedDimsId[i].GetObject(OpenMode.ForWrite);
                    Dimension dimensionForEdit = exampleOfDim as Dimension;
                    if (dimensionForEdit.DimensionText != "")
                    {
                        string text = dimensionForEdit.DimensionText;
                        if (dimensionForEdit.DimensionText.StartsWith("\\X"))
                        {
                            dimensionForEdit.DimensionText = dimensionForEdit.DimensionText.Insert(2, pref.StringResult) + suf.StringResult;
                        }
                        else
                            dimensionForEdit.DimensionText = pref.StringResult + dimensionForEdit.DimensionText + suf.StringResult;
                    }
                    else
                    {
                        dimensionForEdit.Prefix += pref.StringResult;
                        dimensionForEdit.Suffix += suf.StringResult;
                    }
                }
                trans.Commit();
            }
        }
    }
}