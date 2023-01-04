using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.States
{
    class VillagerInteractable : MonoBehaviour, Interactable
    {
        public bool Interact(Humanoid user, bool hold, bool alt)
        {



            return true;

        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }
    }
}
