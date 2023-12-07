using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkRoom
{
    public class GlobalDataManager : NetworkBehaviour
    {
        public event Action<DateTime> OnGameTimeChanged;

        #region SyncVar

        [SyncVar(hook = nameof(PlayerGameTimeChanged))]
        public DateTime time = DateTime.Now;

        void PlayerGameTimeChanged(DateTime _, DateTime newData)
        {
            OnGameTimeChanged?.Invoke(newData);
        }

        #endregion

        /// <summary>
        /// Sync all SyncVars
        /// </summary>
        public void InvokeAll()
        {
            OnGameTimeChanged?.Invoke(time);
        }

        private void Update()
        {
            time = DateTime.Now;
        }
    }
}

