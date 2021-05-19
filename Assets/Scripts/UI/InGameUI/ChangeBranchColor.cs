using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt{
    public class ChangeBranchColor : MonoBehaviour
    {
        [SerializeField] private List<Image> _firstAbilityImg;
        [SerializeField] private List<Image> _secondAbilityImg;
        [SerializeField] private List<Image> _thirdAbilityImg;
        [SerializeField] private List<Image> _fourAbilityImg;
        [SerializeField] private List<Image> _fiveAbilityImg;
        [SerializeField] private List<Image> _sixAbilityImg;
        [SerializeField] private List<Image> _sevenAbilityImg;
        [SerializeField] private List<Image> _eightAbilityImg;
        [SerializeField] private List<Image> _nineAbilityImg;
        [SerializeField] private List<Image> _tenAbilityImg;
        [SerializeField] private List<Image> _elevenAbilityImg;
        [SerializeField] private List<Image> _twelveAbilityImg;


        
        [SerializeField] private List<Image> _firstAbilitySwapImg;
        [SerializeField] private List<Image> _secondAbilitySwapImg;
        [SerializeField] private List<Image> _thirdAbilitySwapImg;
        [SerializeField] private List<Image> _fourAbilitySwapImg;
        [SerializeField] private List<Image> _fiveAbilitySwapImg;
        [SerializeField] private List<Image> _sixAbilitySwapImg;
        [SerializeField] private List<Image> _sevenAbilitySwapImg;
        [SerializeField] private List<Image> _eightAbilitySwapImg;
        [SerializeField] private List<Image> _nineAbilitySwapImg;
        [SerializeField] private List<Image> _tenAbilitySwapImg;
        [SerializeField] private List<Image> _elevenAbilitySwapImg;
        [SerializeField] private List<Image> _twelveAbilitySwapImg;



        private bool first;
        private bool second;
        private bool third;
        private bool four;
        private bool five;
        private bool six;
        private bool seven;
        private bool eight;
        private bool nine;
        private bool ten;
        private bool eleven;
        private bool twelve;










        public void ChangeBranchesColor(int index){
            Debug.LogError("changing color index : "+index);
            switch(index){
                case 0: ChangeColor(_firstAbilityImg,_firstAbilitySwapImg);
                        first = false;
                        break;
                case 1: ChangeColor(_secondAbilityImg,_secondAbilitySwapImg);
                        second = false;
                        break;
                case 2: ChangeColor(_thirdAbilityImg, _thirdAbilitySwapImg);
                        third = false;
                        break;
                case 3: ChangeColor(_fourAbilityImg, _fourAbilitySwapImg);
                        four = false;
                        break;
                case 4: ChangeColor(_fiveAbilityImg, _fiveAbilitySwapImg);
                        five = false;
                        break;
                case 5: ChangeColor(_sixAbilityImg, _sixAbilitySwapImg);
                        six = false;
                        break;
                case 6: ChangeColor(_sevenAbilityImg, _sevenAbilitySwapImg);
                        seven = false;
                        break;
                case 7: ChangeColor(_eightAbilityImg, _eightAbilitySwapImg);
                        eight = false;
                        break;
                case 8: ChangeColor(_nineAbilityImg, _nineAbilitySwapImg);
                        nine = false;
                        break;
                case 9: ChangeColor(_tenAbilityImg, _tenAbilitySwapImg);
                        ten = false;
                        break;
                case 10: ChangeColor(_elevenAbilityImg, _elevenAbilitySwapImg);
                        eleven = false;
                        break;
                case 11: ChangeColor(_twelveAbilityImg, _twelveAbilitySwapImg);
                        twelve = false;
                        break;    
            }
        }

        private void ChangeColor(List<Image> images, List<Image> imagesToSwap){
             foreach(Image img in imagesToSwap)
                img.color = new Color(8,156,0,255);
             foreach(Image img in images)
                img.gameObject.SetActive(false);
        }

        private void Update(){
            if(first)  ChangeBranchesColor(0);
            else if(second)  ChangeBranchesColor(1);
            else if(third)  ChangeBranchesColor(2);
            else if(four)  ChangeBranchesColor(3);
            else if(five)  ChangeBranchesColor(4);
            else if(six)  ChangeBranchesColor(5);
            else if(seven)  ChangeBranchesColor(6);
            else if(eight)  ChangeBranchesColor(7);
            else if(nine)  ChangeBranchesColor(8);
            else if(ten)  ChangeBranchesColor(9);
            else if(eleven)  ChangeBranchesColor(10);
            else if(twelve)  ChangeBranchesColor(11);
        }

        public void animationEndTrigger(int index){
            switch(index){
                case 0: first = true;
                        break;
                case 1: second = true;
                        break;
                case 2: third = true;
                        break;
                case 3: four = true;
                        break;
                case 4: five = true;
                        break;
                case 5: six = true;
                        break;
                case 6: seven = true;
                        break;
                case 7: eight = true;
                        break;
                case 8: nine = true;
                        break;
                case 9: ten = true;
                        break;
                case 10: eleven = true;
                        break;
                case 11: twelve = true;
                        break;    
            }
        }
    }
}
