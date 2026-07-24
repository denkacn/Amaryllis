using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amaryllis.Logs;
using Amaryllis.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.Persistence
{
    public class StatesJsonSerializer : MonoBehaviour
    {
        public string CaptureJson()
        {
            return JsonUtility.ToJson(CaptureSceneSnapshot());
        }

        public StatesSceneSnapshot CaptureSceneSnapshot()
        {
            return new StatesSceneSnapshot
            {
                CreatedUtc = DateTime.UtcNow.ToString("O"),
                States = FindStatesObjects()
                    .Select(statesObject => statesObject.CaptureSnapshot())
                    .Where(state => state != null && !string.IsNullOrWhiteSpace(state.SaveId) && state.StateId != -1)
                    .GroupBy(state => state.SaveId)
                    .Select(group => group.First())
                    .ToList()
            };
        }

        public async UniTask<bool> ApplyJsonAsync(string json, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            var snapshot = JsonUtility.FromJson<StatesSceneSnapshot>(json);
            await ApplySceneSnapshotAsync(snapshot, cancellationToken);
            return true;
        }

        public async UniTask ApplySceneSnapshotAsync(StatesSceneSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            if (snapshot == null || snapshot.States == null)
            {
                return;
            }

            var statesBySaveId = FindStatesObjects()
                .Where(statesObject => !string.IsNullOrWhiteSpace(statesObject.SaveId))
                .GroupBy(statesObject => statesObject.SaveId)
                .ToDictionary(group => group.Key, group => group.First());

            foreach (var stateSnapshot in snapshot.States)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (stateSnapshot == null || string.IsNullOrWhiteSpace(stateSnapshot.SaveId))
                {
                    continue;
                }

                if (!statesBySaveId.TryGetValue(stateSnapshot.SaveId, out var statesObject))
                {
                    AmaryllisLog.Log($"[StatesJsonSerializer] State object {stateSnapshot.SaveId} not found while applying json");
                    continue;
                }

                await statesObject.RestoreSnapshotAsync(stateSnapshot, cancellationToken);
            }
        }

        private static IEnumerable<IStatesObject> FindStatesObjects()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<IStatesObject>();
        }
    }
}
