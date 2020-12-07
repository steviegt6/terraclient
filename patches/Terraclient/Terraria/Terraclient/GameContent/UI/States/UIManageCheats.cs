using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Terraclient.Cheats;
using Terraria.Terraclient.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.Terraclient.GameContent.UI.States
{
	public class UIManageCheats : UIState
	{
		private static List<ICheat> _cheatsFullLine = new List<ICheat> {
			CheatHandler.GodMode,
			CheatHandler.MapTeleport
		};

		private List<UIElement> _bindsKeyboard = new List<UIElement>();
		private UIElement _outerContainer;
		private UIList _uiList;

		public override void OnInitialize() {
			_outerContainer = new UIElement();
			_outerContainer.Width.Set(0f, 0.8f);
			_outerContainer.MaxWidth.Set(600f, 0f);
			_outerContainer.Top.Set(220f, 0f);
			_outerContainer.Height.Set(-200f, 1f);
			_outerContainer.HAlign = 0.5f;

			UIPanel panel = new UIPanel();
			panel.Width.Set(0f, 1f);
			panel.Height.Set(-110f, 1f);
			panel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			_outerContainer.Append(panel);

			_uiList = new UIList();
			_uiList.Width.Set(-25f, 1f);
			_uiList.Height.Set(-10f, 1f);
			_uiList.VAlign = 1f;
			_uiList.PaddingBottom = 5f;
			_uiList.ListPadding = 20f;
			panel.Append(_uiList);

			AssembleBindPanels();
			FillList();

			UIScrollbar scrollbar = new UIScrollbar();
			scrollbar.SetView(100f, 1000f);
			scrollbar.Height.Set(-67f, 1f);
			scrollbar.HAlign = 1f;
			scrollbar.VAlign = 1f;
			scrollbar.MarginBottom = 11f;
			panel.Append(scrollbar);
			_uiList.SetScrollbar(scrollbar);

			UITextPanel<LocalizedText> localizedCheatsTextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Cheats"), 0.7f, large: true);
			localizedCheatsTextPanel.HAlign = 0.5f;
			localizedCheatsTextPanel.Top.Set(-45f, 0f);
			localizedCheatsTextPanel.Left.Set(-10f, 0f);
			localizedCheatsTextPanel.SetPadding(15f);
			localizedCheatsTextPanel.BackgroundColor = new Color(73, 94, 171);
			_outerContainer.Append(localizedCheatsTextPanel);

			UITextPanel<LocalizedText> localizedBackTextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, large: true);
			localizedBackTextPanel.Width.Set(-10f, 0.5f);
			localizedBackTextPanel.Height.Set(50f, 0f);
			localizedBackTextPanel.VAlign = 1f;
			localizedBackTextPanel.HAlign = 0.5f;
			localizedBackTextPanel.Top.Set(-45f, 0f);
			localizedBackTextPanel.OnMouseOver += FadedMouseOver;
			localizedBackTextPanel.OnMouseOut += FadedMouseOut;
			localizedBackTextPanel.OnClick += GoBackClick;
			_outerContainer.Append(localizedBackTextPanel);

			Append(_outerContainer);
		}

		private void AssembleBindPanels() {
			List<ICheat> misc = new List<ICheat> {
				CheatHandler.MapTeleport,
				CheatHandler.JourneyMode,
				CheatHandler.GamemodeUnlockedWorld
			};

			List<ICheat> godMode = new List<ICheat> {
				CheatHandler.GodMode
			};

			int group = 0;
			_bindsKeyboard.Add(CreateBindingGroup(group++, misc));
			_bindsKeyboard.Add(CreateBindingGroup(group++, godMode));
		}

		private UISortableElement CreateBindingGroup(int elementIndex, List<ICheat> cheats) {
			UISortableElement sortableElementIndex = new UISortableElement(elementIndex);
			sortableElementIndex.HAlign = 0.5f;
			sortableElementIndex.Width.Set(0f, 1f);
			sortableElementIndex.Height.Set(2000f, 0f);

			UIPanel panel = new UIPanel();
			panel.Width.Set(0f, 1f);
			panel.Height.Set(-16f, 1f);
			panel.VAlign = 1f;
			panel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			sortableElementIndex.Append(panel);

			UIList list = new UIList();
			list.OverflowHidden = false;
			list.Width.Set(0f, 1f);
			list.Height.Set(-8f, 1f);
			list.VAlign = 1f;
			list.ListPadding = 5f;
			panel.Append(list);

            panel.BackgroundColor = Color.Lerp(panel.BackgroundColor, new Color(63, 82, 151), 0.18f);

			CreateElementGroup(list, panel.BackgroundColor, cheats);

			panel.BackgroundColor = panel.BackgroundColor.MultiplyRGBA(new Color(111, 111, 111));

			LocalizedText text;
			switch (elementIndex) {
				default:
				case 0:
					text = Language.GetText("UI.MiscOptions");
					break;

				case 1:
					text = Language.GetText("UI.GodModeOptions");
					break;
			}

			UITextPanel<LocalizedText> localizedPanel = new UITextPanel<LocalizedText>(text, 0.7f) {
				VAlign = 0f,
				HAlign = 0.5f
			};

			sortableElementIndex.Append(localizedPanel);
			sortableElementIndex.Recalculate();
			sortableElementIndex.Width.Set(0f, 1f);
			sortableElementIndex.Height.Set(list.GetTotalHeight() + 30f + 16f, 0f);

			return sortableElementIndex;
		}

		private void CreateElementGroup(UIList parent, Color color, List<ICheat> cheats) {
			for (int i = 0; i < cheats.Count; i++) {
				UISortableElement sortableIndex = new UISortableElement(i);
				sortableIndex.Width.Set(0f, 1f);
				sortableIndex.Height.Set(30f, 0f);
				sortableIndex.HAlign = 0.5f;
				parent.Add(sortableIndex);

				if (_cheatsFullLine.Contains(cheats[i])) {
					UIElement panel = CreatePanel(color, cheats[i]);
					panel.Width.Set(0f, 1f);
					panel.Height.Set(0f, 1f);
					sortableIndex.Append(panel);
				}
				else {
					UIElement panel = CreatePanel(color, cheats[i]);
					panel.Width.Set(-5f, 0.5f);
					panel.Height.Set(0f, 1f);
					sortableIndex.Append(panel);

					i++;

					if (i < cheats.Count) {
						panel = CreatePanel(color, cheats[i]);
						panel.Width.Set(-5f, 0.5f);
						panel.Height.Set(0f, 1f);
						panel.HAlign = 1f;
						sortableIndex.Append(panel);
					}
				}
			}
		}

		public UIElement CreatePanel(Color color, ICheat cheat) => new UICheatsListItem(color, cheat);

		public override void OnActivate() {
			if (Main.gameMenu) {
				_outerContainer.Top.Set(220f, 0f);
				_outerContainer.Height.Set(-220f, 1f);
			}
			else {
				_outerContainer.Top.Set(120f, 0f);
				_outerContainer.Height.Set(-120f, 1f);
			}

			if (PlayerInput.UsingGamepadUI)
				UILinkPointNavigator.ChangePoint(3002);
		}

		private void FillList() {
			_uiList.Clear();

			foreach (UIElement item in _bindsKeyboard)
				_uiList.Add(item);
		}

		private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(12);
			((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
			((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
		}

		private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement) {
			((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.7f;
			((UIPanel)evt.Target).BorderColor = Color.Black;
		}

		private void GoBackClick(UIMouseEvent evt, UIElement listeningElement) {
			Main.menuMode = 11;
			CheatHandler.SaveCheatSettings();
			IngameFancyUI.Close();
		}
	}
}
