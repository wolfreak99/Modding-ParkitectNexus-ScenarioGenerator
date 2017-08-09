using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ScenarioGenerator
{
	public class ConfigWindow : MonoBehaviour
	{
		private bool _isWindowOpen = false;
		private Rect _windowRectangle = new Rect(50, 50, 1, 1);
		private ValueStore _valuestore = new ValueStore();
		private Generator _generator;
		private string _generatedKey = "";
		private string _displayKey = "";

		private void Start()
		{
			_generator = new Generator(GameController.Instance.park);
		}

		private void Awake()
		{
			DontDestroyOnLoad(this);
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.F8)) {
				_isWindowOpen = !_isWindowOpen;

				// If first opening set window to the center
				if (_windowRectangle.width == 1) {
					_windowRectangle = new Rect(Screen.width / 2 - 310 / 2, Screen.height / 2 - 500 / 2, 310, 744);
				}
			}
		}

		private Rect UIRectangle(int index)
		{
			const int elementHeight = 20;
			const int elementHeightWithMargin = elementHeight + 2;

			return new Rect(5, 15 + elementHeightWithMargin * index, 300, elementHeight);
		}
		private Rect UIRectangleSub(int indexX, int indexY, int countX)
		{
			int elementWidth = 300 / countX;
			int elementWidthWithMargin = elementWidth + 2;
			const int elementHeight = 20;
			const int elementHeightWithMargin = elementHeight + 2;

			return new Rect(3 + elementWidthWithMargin * indexX, 15 + elementHeightWithMargin * indexY, elementWidth, elementHeight);
		}

		private void DoWindow(int windowId)
		{
			var index = 0;

			GUI.Label(UIRectangle(index++), "Seed: ");
			_valuestore.Seed = GUI.TextField(UIRectangle(index++), _valuestore.Seed);

			GUI.Label(UIRectangle(index++), "Plain scale: " + _valuestore.PlainScale);
			_valuestore.PlainScale = GUI.HorizontalSlider(UIRectangle(index++), _valuestore.PlainScale, 0.0005f, 1.0f);

			GUI.Label(UIRectangle(index++), "Max height: " + _valuestore.MaxHeight);
			_valuestore.MaxHeight = GUI.HorizontalSlider(UIRectangle(index++), _valuestore.MaxHeight, 0, 20);

			GUI.Label(UIRectangle(index++), "Max depth: " + _valuestore.MaxDepth);
			_valuestore.MaxDepth = GUI.HorizontalSlider(UIRectangle(index++), _valuestore.MaxDepth, 0, 20);

			GUI.Label(UIRectangle(index++), "Ditch ratio: " + _valuestore.DitchRatio);
			_valuestore.DitchRatio = GUI.HorizontalSlider(UIRectangle(index++), _valuestore.DitchRatio, 0, 1);

			GUI.Label(UIRectangle(index++), "Flood rounds: " + _valuestore.FloodRounds);
			_valuestore.FloodRounds = GUI.HorizontalSlider(UIRectangle(index++), _valuestore.FloodRounds, 0, 100);

			GUI.Label(UIRectangle(index++), "Entrance clearance: " + _valuestore.EntranceClearance);
			_valuestore.EntranceClearance = GUI.HorizontalSlider(UIRectangle(index++), _valuestore.EntranceClearance, 0, 50);

			_valuestore.GenerateTerrainType = GUI.Toggle(UIRectangle(index++), _valuestore.GenerateTerrainType, "Generate Terrain Type");

			GUI.Label(UIRectangle(index++), "Terrain scale: " + _valuestore.TerrainScale);
			_valuestore.TerrainScale = GUI.HorizontalSlider(UIRectangle(index++), _valuestore.TerrainScale, 0.2f, 5.0f);

			GUI.Label(UIRectangle(index++), "Trees: " + _valuestore.TreeCount);
			_valuestore.TreeCount = GUI.HorizontalSlider(UIRectangle(index++), _valuestore.TreeCount, 0, 1000);


			if (GUI.Button(UIRectangle(index++), "Generate")) {
				_generator.Generate(_valuestore, GenerateFlags.All);
				_isWindowOpen = false;
			}
			{
				int indexX = 0;
				if (GUI.Button(UIRectangleSub(indexX++, index, 4), "Height")) {
					_generator.Generate(_valuestore, GenerateFlags.Height);
				}
				if (GUI.Button(UIRectangleSub(indexX++, index, 4), "Type")) {
					_generator.Generate(_valuestore, GenerateFlags.TerrainType);
				}
				if (GUI.Button(UIRectangleSub(indexX++, index, 4), "Water")) {
					_generator.Generate(_valuestore, GenerateFlags.Water);
				}
				if (GUI.Button(UIRectangleSub(indexX++, index, 4), "Trees")) {
					_generator.Generate(_valuestore, GenerateFlags.Trees);
				}
				index++;
			}
			if (GUI.Button(UIRectangle(index++), "Reset")) {
				_generator.Reset(GenerateFlags.All);
			}
			{
				int indexX = 0;
				if (GUI.Button(UIRectangleSub(indexX++, index, 4), "Height")) {
					_generator.Reset(GenerateFlags.Height);
				}
				if (GUI.Button(UIRectangleSub(indexX++, index, 4), "Type")) {
					_generator.Reset(GenerateFlags.TerrainType);
				}
				if (GUI.Button(UIRectangleSub(indexX++, index, 4), "Water")) {
					_generator.Reset(GenerateFlags.Water);
				}
				if (GUI.Button(UIRectangleSub(indexX++, index, 4), "Trees")) {
					_generator.Reset(GenerateFlags.Trees);
				}
				index++;
			}

			if (GUI.Button(UIRectangle(index++), "Cancel")) {
				_isWindowOpen = false;
			}

			_displayKey = GUI.TextField(UIRectangle(index++), _displayKey);
			if (_valuestore.HasValueStoreChange) {
				_displayKey = _valuestore.Serialize();
			}
			else {
				try {
					_valuestore.DeSerialize(_displayKey);
				}
				catch (Exception e) {
					Debug.Log(e.Message);
				}
			}
			
			if (GUI.Button(UIRectangle(index++), "clipboard")) {
				TextEditor te = new TextEditor();
				te.text = _displayKey;
				te.SelectAll();
				te.Copy();
			}

			if (GUI.Button(UIRectangle(index++), "Clear")) {
				_displayKey = "";

			}

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}

		public void OnGUI()
		{
			if (!_isWindowOpen) {
				return;
			}

			_windowRectangle = GUI.Window(0, _windowRectangle, DoWindow, "Scenario Generators");
		}
	}
}
