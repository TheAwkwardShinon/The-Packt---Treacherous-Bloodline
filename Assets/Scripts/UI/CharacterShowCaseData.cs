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
        [SerializeField] private Sprite _characterSprite;
        [SerializeField] private Sprite _personal;
        [SerializeField] private Sprite _stat3_a;
        [SerializeField] private Sprite _weakActive;
        [SerializeField] private Sprite _passive1;
        [SerializeField] private Sprite _stat3_b;
        [SerializeField] private Sprite _stat2_a;
        [SerializeField] private Sprite _stat2_b;
        [SerializeField] private Sprite _midActive;
        [SerializeField] private Sprite _passive2;
        [SerializeField] private Sprite _stat3_c;
        [SerializeField] private Sprite _passive3;
        [SerializeField] private Sprite _stat1;
        [SerializeField] private Sprite _activeAttack;
        [SerializeField] private Sprite _ultimate;

        #endregion

        #region descriptions
        [Header("Descriptions")]
        [TextArea] [SerializeField] private string _lore;
        [SerializeField] private string _ClassName;
        [TextArea] [SerializeField] private string _descriptionAbility1;
        [TextArea] [SerializeField] private string _descriptionAbility2;
        [TextArea] [SerializeField] private string _descriptionAbility3;
        [TextArea] [SerializeField] private string _descriptionAbility4;
        [TextArea] [SerializeField] private string _descriptionAbility5;
        [TextArea] [SerializeField] private string _descriptionAbility6;
        [TextArea] [SerializeField] private string _descriptionAbility7;
        [TextArea] [SerializeField] private string _descriptionAbility8;
        [TextArea] [SerializeField] private string _descriptionAbility9;
        [TextArea] [SerializeField] private string _descriptionAbility10;
        [TextArea] [SerializeField] private string _descriptionAbility11;
        [TextArea] [SerializeField] private string _descriptionAbility12;
        [TextArea] [SerializeField] private string _descriptionAbility13;
        [TextArea] [SerializeField] private string _descriptionAbility14;
        #endregion


    }
}
