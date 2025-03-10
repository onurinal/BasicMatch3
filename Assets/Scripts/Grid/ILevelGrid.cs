using System.Collections;

namespace BasicMatch3.Grid
{
    public interface ILevelGrid
    {
        public IEnumerator StartScanGrid();
        public bool IsGridInitializing { get; }
    }
}