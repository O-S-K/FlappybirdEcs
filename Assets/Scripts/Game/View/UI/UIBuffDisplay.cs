using System.Collections.Generic;
using UnityEngine;

namespace FlappyECS
{
    public class UIBuffDisplay : MonoBehaviour
    {
        public GameObject buffItemPrefab;
        public Transform container;

        private List<UIBuffItem> activeItems = new List<UIBuffItem>();

        private void OnEnable()
        {
            BuffIecsSystem.OnBuffsUpdated += UpdateBuffs;
        }

        private void OnDisable()
        {
            BuffIecsSystem.OnBuffsUpdated -= UpdateBuffs;
        }

        private void UpdateBuffs(List<BuffDisplayData> buffs)
        {
            // Sync số lượng items
            while (activeItems.Count < buffs.Count)
            {
                var go = Instantiate(buffItemPrefab, container);
                activeItems.Add(go.GetComponent<UIBuffItem>());
            }

            for (int i = 0; i < activeItems.Count; i++)
            {
                if (i < buffs.Count)
                {
                    activeItems[i].gameObject.SetActive(true);
                    activeItems[i].UpdateBuff(buffs[i]);
                }
                else
                {
                    activeItems[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
