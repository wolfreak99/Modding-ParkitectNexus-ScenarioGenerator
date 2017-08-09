using UnityEngine;

namespace ScenarioGenerator
{
    public class Main : IMod
    {
        private GameObject _go;

		#region Implementation of IMod


        public void onEnabled()
        {
            _go = new GameObject(Name);
			_go.AddComponent<ConfigWindow>();
        }

        public void onDisabled()
        {
            Object.Destroy(_go);
        }

        public string Name
        {
            get { return "Scenario Generator"; }
        }

        public string Description
        {
            get { return "Genarates Scenarios. Press F8 to open menu."; }
        }

        public string Identifier { get; set; }

        #endregion
    }
}