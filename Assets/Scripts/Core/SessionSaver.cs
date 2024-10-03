using System.Collections.Generic;
using Core.Element;
using Core.Session;
using Services.Save;

namespace Core
{
    public sealed class SessionSaver : ISessionSaver
    {
        private readonly ISaveService _saveService;
        private readonly Session.SessionController _sessionController;

        private readonly List<int> _elements = new List<int>();
        private readonly SessionSaveData _saveData = new SessionSaveData();

        public SessionSaver(ISaveService saveService, Session.SessionController sessionController)
        {
            _saveService = saveService;
            _sessionController = sessionController;
        }

        public void UpdateSaveData()
        {
            _elements.Clear();

            for (int i = 0; i < _sessionController.Elements.Length; i++)
            {
                for (int j = 0; j < _sessionController.Elements[i].Length; j++)
                {
                    GridGameElement element = _sessionController.Elements[i][j];
                    ElementType type = element == null ? ElementType.None : element.Type;
                    _elements.Add((int)type);
                }
            }
            
            _saveData.Level = _sessionController.Level;
            _saveData.Rows = _sessionController.Elements.Length;
            _saveData.Columns = _sessionController.Elements[0].Length;
            _saveData.Elements = _elements;

            _saveService.Save(_saveData);
        }
    }
}