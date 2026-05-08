import { useState, useEffect } from 'react';
import { getSyncLogs } from '../api/baresApi';
import { FiCheckCircle, FiXCircle, FiClock } from 'react-icons/fi';

export default function LogsModal({ show, onClose }) {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (show) {
      setLoading(true);
      getSyncLogs()
        .then((res) => setLogs(res.data))
        .catch((err) => console.error('Error fetching logs:', err))
        .finally(() => setLoading(false));
    }
  }, [show]);

  if (!show) return null;

  const formatDate = (dateStr) => {
    if (!dateStr) return '---';
    const d = new Date(dateStr);
    return d.toLocaleString('es-AR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-panel modal-panel-wide" onClick={(e) => e.stopPropagation()}>

        <div className="modal-panel-header">
          <h5 className="modal-panel-title">
            <FiClock className="me-2" style={{ verticalAlign: '-2px' }} />
            Historial de Sincronizacion
          </h5>
          <button type="button" className="btn-close" onClick={onClose} aria-label="Cerrar" />
        </div>

        <div className="modal-panel-body p-0">
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border text-secondary spinner-border-sm" role="status" />
              <p className="mt-2 mb-0 small text-secondary">Cargando historial...</p>
            </div>
          ) : logs.length === 0 ? (
            <div className="text-center py-5">
              <FiClock size={32} className="text-secondary mb-2" />
              <p className="text-secondary mb-0">No hay registros de sincronizacion.</p>
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table logs-table mb-0" id="logs-table">
                <thead>
                  <tr>
                    <th>Fecha</th>
                    <th>Bares Agregados</th>
                    <th>Estado</th>
                    <th>Detalle</th>
                  </tr>
                </thead>
                <tbody>
                  {logs.map((log) => (
                    <tr key={log.id}>
                      <td className="text-nowrap">{formatDate(log.timestamp)}</td>
                      <td>
                        <span className="fw-semibold">{log.barsAdded}</span>
                      </td>
                      <td>
                        {log.isSuccess ? (
                          <span className="badge badge-log-success">
                            <FiCheckCircle size={12} className="me-1" />
                            Exito
                          </span>
                        ) : (
                          <span className="badge badge-log-error">
                            <FiXCircle size={12} className="me-1" />
                            Error
                          </span>
                        )}
                      </td>
                      <td className="log-error-msg">
                        {log.errorMessage || '—'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        <div className="modal-panel-footer">
          <div className="d-flex justify-content-end w-100">
            <button type="button" className="btn-modal-cancel" onClick={onClose}>
              Cerrar
            </button>
          </div>
        </div>

      </div>
    </div>
  );
}
