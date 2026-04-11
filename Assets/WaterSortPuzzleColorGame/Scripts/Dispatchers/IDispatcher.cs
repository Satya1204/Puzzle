
using System;

namespace WaterSortPuzzleGame.Dispatchers
{
    public interface IDispatcher
    {
        void Invoke(Action fn);
    }
}