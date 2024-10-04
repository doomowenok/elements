using Core.Element.Factory;
using Core.Grid;
using Core.Session;
using Cysharp.Threading.Tasks;
using Services.Save;

namespace Core.Utils
{
    public sealed class GameplayDisposer : IGameplayDisposer
    {
        private readonly ISaveService _saveService;
        private readonly IGridGameElementFactory _elementFactory;
        private readonly SessionController _sessionController;
        private readonly IGridElementController _gridElementController;

        public GameplayDisposer(
            ISaveService saveService, 
            IGridGameElementFactory elementFactory, 
            SessionController sessionController,
            IGridElementController gridElementController)
        {
            _saveService = saveService;
            _elementFactory = elementFactory;
            _sessionController = sessionController;
            _gridElementController = gridElementController;
        }
        
        public async UniTask DisposeGameAsync()
        {
            _gridElementController.Dispose();
            _saveService.ClearSave<SessionSaveData>();
            await _elementFactory.DestroyAllElements();
            _sessionController.ClearElements();
        }
    }
}