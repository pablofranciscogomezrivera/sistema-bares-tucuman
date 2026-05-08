import { FiEdit2, FiTrash2, FiMapPin } from 'react-icons/fi';
import { BsRobot } from 'react-icons/bs';

export default function BarCard({ bar, onEdit, onDelete }) {
  return (
    <div className="card bar-card mb-3" id={`bar-card-${bar.id}`}>
      <div className="row g-0">
        <div className="col-12">
          <div className="card-body d-flex flex-column h-100 py-3">
            <div className="d-flex justify-content-between align-items-start mb-2">
              
              <span 
                className="text-secondary fw-bold text-uppercase" 
                style={{ fontSize: '0.75rem', letterSpacing: '0.5px' }}
              >
                {bar.categoriaAMostrar}
              </span>
              
              <div className="bar-actions">
                <button className="btn btn-sm btn-outline-secondary" title="Editar" onClick={() => onEdit(bar)} id={`btn-edit-${bar.id}`}>
                  <FiEdit2 size={13} />
                </button>
                <button className="btn btn-sm btn-outline-danger" title="Eliminar" onClick={() => onDelete(bar)} id={`btn-delete-${bar.id}`}>
                  <FiTrash2 size={13} />
                </button>
              </div>
            </div>

            <h5 className="card-title bar-name mb-1">{bar.nombre}</h5>
            <p className="bar-location mb-2">
              <FiMapPin size={13} className="me-1" />
              {bar.ubicacion || 'Ubicacion no disponible'}
            </p>

            {bar.aiDescription && (
              <div className="ai-description mt-auto">
                <BsRobot className="ai-icon" />
                <span className="ai-label">Descripcion IA:</span>
                <span>{bar.aiDescription}</span>
              </div>
            )}

            {bar.fuente && (
              <small className="bar-source mt-2 d-block">
                Fuente: {bar.fuente} — {bar.scrapedAt ? new Date(bar.scrapedAt).toLocaleDateString('es-AR') : '---'}
              </small>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}