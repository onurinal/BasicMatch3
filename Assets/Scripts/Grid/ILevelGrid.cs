using System.Collections;

namespace BasicMatch3.Grid
{
    public interface ILevelGrid
    {
        IEnumerator StartScanGrid();
        bool IsGridInitializing { get; }
    }
}