using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleApp.Features.MatchObjects
{
    public interface IMatchObjectsProgress
    {
        event Action<int> LevelCompleted;
        bool IsCompleted(int levelIndex);
        void MarkCompleted(int levelIndex);
    }

    public sealed class MatchObjectsProgress : IMatchObjectsProgress
    {
        const string PrefsKey = "MatchObjects.CompletedLevels";

        readonly HashSet<int> _completed = new();

        public event Action<int> LevelCompleted;

        public MatchObjectsProgress()
        {
            Load();
        }

        public bool IsCompleted(int levelIndex) => _completed.Contains(levelIndex);

        public void MarkCompleted(int levelIndex)
        {
            if (!_completed.Add(levelIndex))
                return;

            Save();
            LevelCompleted?.Invoke(levelIndex);
        }

        void Load()
        {
            _completed.Clear();
            var raw = PlayerPrefs.GetString(PrefsKey, string.Empty);
            if (string.IsNullOrEmpty(raw))
                return;

            foreach (var part in raw.Split(','))
            {
                if (int.TryParse(part, out var idx))
                    _completed.Add(idx);
            }
        }

        void Save()
        {
            PlayerPrefs.SetString(PrefsKey, string.Join(",", _completed));
            PlayerPrefs.Save();
        }
    }
}
