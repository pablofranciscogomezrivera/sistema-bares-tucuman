import { FiAlertTriangle } from 'react-icons/fi';

export default function DeleteConfirmModal({ show, bar, onClose, onConfirm, deleting }) {
  if (!show || !bar) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-panel" onClick={(e) => e.stopPropagation()}>
        <div className="modal-panel-header">
          <h5 className="modal-panel-title text-danger d-flex align-items-center">
            <FiAlertTriangle className="me-2" />
            Confirmar Eliminación
          </h5>
          <button 
            type="button" 
            className="btn-close" 
            onClick={onClose} 
            aria-label="Cerrar" 
            disabled={deleting} 
          />
        </div>

        <div className="modal-panel-body py-4">
          <p className="mb-2">¿Estás seguro de que deseas eliminar el bar <strong>"{bar.nombre}"</strong>?</p>
          <p className="text-muted small mb-0">Esta acción cambiará el estado del bar a inactivo.</p>
        </div>

        <div className="modal-panel-footer">
          <div className="d-flex justify-content-between align-items-center w-100">
            <button 
              type="button" 
              className="btn-modal-cancel" 
              onClick={onClose} 
              disabled={deleting}
            >
              Cancelar
            </button>
            <button 
              type="button" 
              className="btn btn-danger rounded-3 fw-semibold px-3 py-2" 
              onClick={() => onConfirm(bar)} 
              disabled={deleting}
            >
              {deleting ? (
                <><span className="spinner-border spinner-border-sm me-2" role="status" />Eliminando...</>
              ) : (
                'Sí, eliminar'
              )}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}