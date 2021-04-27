using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    [CreateAssetMenu(fileName = "newCharacterData", menuName = "Data/Character Data/Clan Data")]
    public class ClanShowCaseData : ScriptableObject
    {
        public  List<Sprite> _abilitisSprite;
        [TextArea] public List<string> abilitiesDescription;
        public List<int> abilitiesCost;
        public List<string> abilitiesName;
        public Sprite clanLogo;
        public Sprite characterSprite;


    }
}
