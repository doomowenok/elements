using System.Collections.Generic;
using Core.Element;
using Core.Session;
using Services.Save;

namespace Core
{
    public sealed class SessionSaver : ISessionSaver
    {
        private readonly ISaveService _saveService;
        private readonly SessionData _sessionData;

        private readonly List<int> _elements = new List<int>();
        private readonly SessionSaveData _saveData = new SessionSaveData();

        public SessionSaver(ISaveService saveService, SessionData sessionData)
        {
            _saveService = saveService;
            _sessionData = sessionData;
        }

        public void UpdateSaveData()
        {
            _elements.Clear();

            for (int i = 0; i < _sessionData.Elements.Length; i++)
            {
                for (int j = 0; j < _sessionData.Elements[i].Length; j++)
                {
                    GridGameElement element = _sessionData.Elements[i][j];
                    ElementType type = element == null ? ElementType.None : element.Type;
                    _elements.Add((int)type);
                }
            }
            
            _saveData.Level = _sessionData.Level;
            _saveData.Rows = _sessionData.Elements.Length;
            _saveData.Columns = _sessionData.Elements[0].Length;
            _saveData.Elements = _elements;

            _saveService.Save(_saveData);
        }
    }
}