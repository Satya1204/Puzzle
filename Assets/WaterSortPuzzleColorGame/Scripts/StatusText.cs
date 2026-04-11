using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace WaterSortPuzzleGame
{
    public class StatusText : Text
    {
        // This file for capture Error ad warning log .... To see this Setactive Gameobject inside Menu Scene->Canvas->TopList->Scroll View
        private SynchronizationContext _synchronizationContext;

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                supportRichText = true;
                text = string.Empty;
                _synchronizationContext = SynchronizationContext.Current;
                Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        }

        private void OnLogMessageReceivedThreaded(string logString, string stackTrace, LogType type)
        {
            _synchronizationContext.Post((sender) =>
            {
                // Safeguard against race conditions from Unity disposed objects.
                if (this == null || !Application.isPlaying)
                {
                    return;
                }

                string color;
                switch (type)
                {
                    case LogType.Warning:
                        color = "yellow";
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        color = "red";
                        break;
                    default:
                        color = "white";
                        break;
                }

                string message = $"<color={color}>{logString}</color>\n\r";
                text += message;
            }, this);
        }
    }
}