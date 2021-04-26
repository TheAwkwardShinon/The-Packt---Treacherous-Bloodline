using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt{

    [CreateAssetMenu(fileName = "newCharacterData", menuName = "Data/Character Data/Showcase Data")]
    public class CharacterShowCaseData : ScriptableObject
    {
        #region sprites
        [Header("Sprites")]
        public Sprite personalAbility;
        public ClanShowCaseData classData;

        #endregion

        #region descriptions
        [Header("Descriptions")]
        [TextArea(10,30)] public string lore;
        public string ClassName;

        public string personalAbilityName;
        [TextArea] public string personalAbilityDescription;
        #endregion


    }
}
