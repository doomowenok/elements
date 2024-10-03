using System.Collections.Generic;
using Core.Element;
using Core.Session;
using Services.Save;

namespace Core
{
    public sealed class SessionSaver : ISessionSaver
    {
        private readonly ISaveService _saveService;
        private readonly Session.Session _session;

        private readonly List<int> _elements = new List<int>();
        private readonly SessionSaveData _saveData = new SessionSaveData();

        public SessionSaver(ISaveService saveService, Session.Session session)
        {
            _saveService = saveService;
            _session = session;
        }

        public void UpdateSaveData()
        {
            _elements.Clear();

            for (int i = 0; i < _session.Elements.Length; i++)
            {
                for (int j = 0; j < _session.Elements[i].Length; j++)
                {
                    GridGameElement element = _session.Elements[i][j];
                    ElementType type = element == null ? ElementType.None : element.Type;
                    _elements.Add((int)type);
                }
            }
            
            _saveData.Level = _session.Level;
            _saveData.Rows = _session.Elements.Length;
            _saveData.Columns = _session.Elements[0].Length;
            _saveData.Elements = _elements;

            _saveService.Save(_saveData);
        }
    }
}