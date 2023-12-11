using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkRoom
{
    public class LocalDataManager : NetworkBehaviour
    {
        public event Action<string> OnNameChanged;
        public event Action<Color32> OnColorChanged;
        public event Action<ushort> OnHitCountChanged;
        public event Action<ushort> OnAttackCountChanged;

        #region SyncVar

        [SyncVar(hook = nameof(PlayerNameChanged))]
        public string name = string.Empty;

        [SyncVar(hook = nameof(PlayerColorChanged))]
        public Color32 color = Color.white;

        [SyncVar(hook = nameof(PlayerHitCountChanged))]
        public ushort hitCount = 0;

        [SyncVar(hook = nameof(PlayerAttackCountChanged))]
        public ushort attackCount = 0;

        void PlayerNameChanged(string _, string newData)
        {
            OnNameChanged?.Invoke(newData);
        }

        void PlayerColorChanged(Color32 _, Color32 newColor)
        {
            OnColorChanged?.Invoke(newColor);
        }

        void PlayerHitCountChanged(ushort _, ushort newData)
        {
            OnHitCountChanged?.Invoke(newData);
        }

        void PlayerAttackCountChanged(ushort _, ushort newData)
        {
            OnAttackCountChanged?.Invoke(newData);
        }

        #endregion

        /// <summary>
        /// Sync all SyncVars
        /// </summary>
        public void InvokeAll()
        {
            OnColorChanged?.Invoke(color);
            OnNameChanged?.Invoke(name);
            OnHitCountChanged?.Invoke(hitCount);
            OnAttackCountChanged?.Invoke(attackCount);
        }
    }
}
