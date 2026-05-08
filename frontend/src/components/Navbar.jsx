import { FiRefreshCw, FiClock } from 'react-icons/fi';

export default function Navbar({ onSync, onShowLogs, syncing }) {
  return (
    <nav className="navbar navbar-expand navbar-dark sticky-top" id="main-navbar">
      <div className="container-fluid px-4">
        <a className="navbar-brand fw-bold d-flex align-items-center gap-2" href="#">
          <span className="brand-dot"></span>
          Bares Tucuman
        </a>
        <div className="d-flex align-items-center gap-2">
          <button
            className="btn btn-nav btn-nav-outline"
            onClick={onShowLogs}
            id="btn-show-logs"
            title="Ver Historial de Sincronización"
          >
            <FiClock size={15} className="me-1" />
            <span className="d-none d-sm-inline">Historial</span>
          </button>
          <button
            className="btn btn-nav btn-nav-accent"
            onClick={onSync}
            disabled={syncing}
            id="btn-sync"
            title="Forzar Sincronización IA"
          >
            {syncing ? (
              <>
                <span className="spinner-border spinner-border-sm me-1" role="status" />
                <span className="d-none d-sm-inline">Sincronizando...</span>
              </>
            ) : (
              <>
                <FiRefreshCw size={15} className="me-1" />
                <span className="d-none d-sm-inline">Sincronizar</span>
              </>
            )}
          </button>
        </div>
      </div>
    </nav>
  );
}
