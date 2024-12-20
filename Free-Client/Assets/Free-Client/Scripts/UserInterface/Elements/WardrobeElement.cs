using Assambra.FreeClient.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class WardrobeElement : MonoBehaviour
    {
        [SerializeField] TMP_Text textWardrobeName;
        [SerializeField] TMP_Dropdown dropdownWardrobe;
        [SerializeField] GameObject _prefabColorElement;
        [SerializeField] Transform _colorHome;
        private DynamicCharacterAvatar _avatar;

        private Dictionary<string, int> options = new Dictionary<string, int>();
        private Dictionary<UMATextRecipe, int> recipes = new Dictionary<UMATextRecipe, int>();
        private List<UMATextRecipe> _recipesToShow = new List<UMATextRecipe>();
        private UMATextRecipe lastRecipe;
        private Transform _allwaysOnTop;
        private Transform _layout;
        private List<GameObject> _activeColorElements = new List<GameObject>();
        private bool _hasNoneOption;

        private void Awake()
        {
            dropdownWardrobe.ClearOptions();
        }

        public void InitializeWardrobe(DynamicCharacterAvatar avatar, string slotname, Transform layout, Transform allwayOnTop, SO_RecipesToShow recipesToShow, bool hasNoneOption)
        {
            textWardrobeName.text = slotname;
            this._avatar = avatar;
            this._layout = layout;
            this._allwaysOnTop = allwayOnTop;
            this._hasNoneOption = hasNoneOption;

            foreach (SO_Recipes rts in recipesToShow.RecipesToShow)
            {
                foreach (UMATextRecipe utr in rts.Recipes)
                {
                    _recipesToShow.Add(utr);
                }
            }

            CreateOptions(slotname, hasNoneOption);
        }

        private void CreateOptions(string slotname, bool hasNoneOption)
        {
            List<UMATextRecipe> slotRecipes = _avatar.AvailableRecipes[slotname];

            int i = 0;

            if (hasNoneOption)
            {
                options.Add("None", 0);
                i = 1;
            }

            foreach (UMATextRecipe utr in slotRecipes)
            {
                if (_recipesToShow.Contains(utr))
                {
                    string name = GetRecipeNameOrDisplayValue(utr);

                    recipes.Add(utr, i);
                    options.Add(name, i);

                    i++;
                }
            }

            dropdownWardrobe.AddOptions(options.Keys.ToList());
            PreselectionOption(options);

            dropdownWardrobe.onValueChanged.AddListener(delegate
            {
                OnDropdownValueChanged(dropdownWardrobe);
            });
        }

        private void PreselectionOption(Dictionary<string, int> options)
        {
            Dictionary<string, UMATextRecipe> avatarRecipies = _avatar.WardrobeRecipes;

            foreach (KeyValuePair<string, UMATextRecipe> recipe in avatarRecipies)
            {
                string name = GetRecipeNameOrDisplayValue(recipe.Value);

                if (options.ContainsKey(name))
                {
                    dropdownWardrobe.SetValueWithoutNotify(options[name]);
                    lastRecipe = recipe.Value;
                    OverlayColorData[] colors = recipe.Value.SharedColors;
                    CreateColorSelectors(colors, _allwaysOnTop);
                }
            }
        }

        private string GetRecipeNameOrDisplayValue(UMATextRecipe recipe)
        {
            string name;

            if (string.IsNullOrEmpty(recipe.DisplayValue))
                name = recipe.name;
            else
                name = recipe.DisplayValue;

            return name;
        }

        private void OnDropdownValueChanged(TMP_Dropdown change)
        {
            foreach (KeyValuePair<UMATextRecipe, int> recipe in recipes)
            {
                if (change.value != 0 || !_hasNoneOption)
                {
                    if (change.value == recipe.Value)
                    {
                        _avatar.SetSlot(recipe.Key);
                        _avatar.BuildCharacter(true);
                        lastRecipe = recipe.Key;

                        ClearColors();
                        OverlayColorData[] sharedColors = recipe.Key.SharedColors;
                        CreateColorSelectors(sharedColors, _allwaysOnTop);
                    }
                }
                else
                {
                    _avatar.ClearSlot(lastRecipe.wardrobeSlot);
                    _avatar.BuildCharacter(true);
                    ClearColors();
                    RebuildLayout();
                }
            }
        }

        private void CreateColorSelectors(OverlayColorData[] sharedColors, Transform allwaysOnTop)
        {
            for (int i = 0; i < sharedColors.Length; i++)
            {
                GameObject go = Instantiate(_prefabColorElement, _colorHome);
                _activeColorElements.Add(go);
                ColorSelectorElement color = go.GetComponent<ColorSelectorElement>();
                foreach (OverlayColorData ocd in _avatar.ActiveColors)
                {
                    if (ocd.name == sharedColors[i].name)
                        color.Initialize(_avatar, ocd, sharedColors[i].name, allwaysOnTop);
                }
            }
            RebuildLayout();
        }

        private void ClearColors()
        {
            foreach (GameObject go in _activeColorElements)
            {
                ColorSelectorElement color = go.GetComponent<ColorSelectorElement>();
                color.RemoveColorPicker();

                Destroy(go);
            }
            _activeColorElements.Clear();
        }

        private void RebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_colorHome.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layout.GetComponent<RectTransform>());
        }
    }
}
