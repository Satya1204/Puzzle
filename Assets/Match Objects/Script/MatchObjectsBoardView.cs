using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.Features.MatchObjects;

namespace PuzzleApp.MatchObjects
{
    public class MatchObjectsBoardView : MonoBehaviour
    {
        [SerializeField] Transform _leftColumn;
        [SerializeField] Transform _rightColumn;
        [SerializeField] Transform _bottomBar;
        [SerializeField] MatchObjectsClueItem _clueItemPrefab;
        [SerializeField] MatchObjectsDropZone _dropZonePrefab;
        [SerializeField] MatchObjectsDraggableItem _draggableItemPrefab;

        int _solvedCount;
        int _totalPairs;
        public event Action GameWon;

        public void StartGame(MatchObjectsItemPair[] pairs, Canvas rootCanvas)
        {
            if (pairs == null || pairs.Length == 0)
            {
                Debug.LogError("MatchObjectsBoardView: no pairs provided.");
                return;
            }

            _solvedCount = 0;
            _totalPairs = pairs.Length;

            var shuffledPairs = new List<MatchObjectsItemPair>(pairs);
            ShuffleList(shuffledPairs);

            SpawnClueItems(shuffledPairs);
            SpawnDropZones(shuffledPairs);

            ShuffleList(shuffledPairs);
            SpawnDraggableItems(shuffledPairs, rootCanvas);
        }

        void SpawnClueItems(List<MatchObjectsItemPair> pairs)
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                var clueInstance = Instantiate(_clueItemPrefab, _leftColumn);
                clueInstance.Bind(pairs[i].pairId, pairs[i].leftSprite);
            }
        }

        void SpawnDropZones(List<MatchObjectsItemPair> pairs)
        {
            var dropZones = new List<MatchObjectsDropZone>();
            for (int i = 0; i < pairs.Count; i++)
            {
                var zoneInstance = Instantiate(_dropZonePrefab, _rightColumn);
                var onCorrectDrop = new Action(() => OnPairMatched());
                zoneInstance.Setup(pairs[i].pairId, onCorrectDrop);
                dropZones.Add(zoneInstance);
            }
        }

        void SpawnDraggableItems(List<MatchObjectsItemPair> pairs, Canvas rootCanvas)
        {
            var dropZones = _rightColumn.GetComponentsInChildren<MatchObjectsDropZone>();

            for (int i = 0; i < pairs.Count; i++)
            {
                var draggableInstance = Instantiate(_draggableItemPrefab, _bottomBar);
                draggableInstance.Bind(pairs[i].pairId, pairs[i].rightSprite, dropZones, rootCanvas);
            }
        }

        void OnPairMatched()
        {
            _solvedCount++;
            if (_solvedCount >= _totalPairs)
            {
                GameWon?.Invoke();
            }
        }

        void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIdx = UnityEngine.Random.Range(0, i + 1);
                var temp = list[i];
                list[i] = list[randomIdx];
                list[randomIdx] = temp;
            }
        }
    }
}
