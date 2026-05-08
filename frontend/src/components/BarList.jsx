import BarCard from './BarCard';
import { FiChevronLeft, FiChevronRight, FiInbox } from 'react-icons/fi';

export default function BarList({ bares, page, totalPages, onPageChange, loading }) {
  if (loading) {
    return (
      <div id="bar-list-loading">
        {Array.from({ length: 3 }).map((_, i) => (
          <div key={i} className="card bar-card mb-3 placeholder-glow">
            <div className="row g-0">
              <div className="col-md-4">
                <div className="placeholder w-100 rounded-start" style={{ height: 250 }}></div>
              </div>
              <div className="col-md-8">
                <div className="card-body py-3">
                  <h5><span className="placeholder col-7"></span></h5>
                  <p><span className="placeholder col-5"></span></p>
                  <p><span className="placeholder col-9"></span></p>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (bares.length === 0) {
    return (
      <div className="text-center py-5" id="no-results">
        <FiInbox size={48} className="empty-icon mb-3" />
        <h5 className="text-white">No se encontraron bares</h5>
        <p className="text-secondary">No hay datos disponibles.</p>
      </div>
    );
  }

  return (
    <div id="bar-list">
      {bares.map((bar) => (
        <BarCard key={bar.id} bar={bar} />
      ))}

      {totalPages > 1 && (
        <nav aria-label="Paginacion" id="pagination-controls">
          <ul className="pagination pagination-custom justify-content-center mt-4 mb-0">
            <li className={`page-item ${page <= 1 ? 'disabled' : ''}`}>
              <button
                className="page-link"
                onClick={() => onPageChange(page - 1)}
                disabled={page <= 1}
                id="btn-prev-page"
              >
                <FiChevronLeft size={15} />
                <span className="d-none d-sm-inline ms-1">Anterior</span>
              </button>
            </li>
            {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
              <li key={p} className={`page-item ${page === p ? 'active' : ''}`}>
                <button className="page-link" onClick={() => onPageChange(p)} id={`btn-page-${p}`}>
                  {p}
                </button>
              </li>
            ))}
            <li className={`page-item ${page >= totalPages ? 'disabled' : ''}`}>
              <button
                className="page-link"
                onClick={() => onPageChange(page + 1)}
                disabled={page >= totalPages}
                id="btn-next-page"
              >
                <span className="d-none d-sm-inline me-1">Siguiente</span>
                <FiChevronRight size={15} />
              </button>
            </li>
          </ul>
        </nav>
      )}
    </div>
  );
}
