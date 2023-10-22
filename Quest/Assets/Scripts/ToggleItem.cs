using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleItem : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI _toggleText;
   [SerializeField] private Button _button;
   [SerializeField] private Image _backgroundImage;
   [SerializeField] private Color _selectedColour;
   [SerializeField] private Color _deselectedColour;

   private bool _isSelected;
   private int _index;

   public int Index => _index;

   public bool IsSelected
   {
      get => _isSelected;
      set
      {
         _isSelected = value;
         _backgroundImage.color = _isSelected ? _selectedColour : _deselectedColour;
      }
   }

   public event Action<ToggleItem> ButtonClicked; 

   public bool IsCorrect { get; private set; }

   public ToggleItem Init(string text, bool isCorrect, int index)
   {
      _toggleText.text = text;
      _index = index;
      IsCorrect = isCorrect;
      _button.onClick.AddListener(OnButtonClicked);
      
      return this;
   }

   private void OnDisable()
   {
      _button.onClick.RemoveListener(OnButtonClicked);
   }

   private void OnButtonClicked()
   {
      if (IsSelected)
      {
         return;
      }
      ButtonClicked?.Invoke(this);
   }
}