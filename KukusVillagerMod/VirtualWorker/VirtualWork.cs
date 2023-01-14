using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.VirtualWorker
{
    class VirtualWork
    {
        public void vw()
        {
            if (ZInput.instance != null && Player.m_localPlayer != null)
            {
                if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Backspace)
                {
                    KLog.warning("BACKLOG PRESSED");
                }
            }
        }
    }
}
