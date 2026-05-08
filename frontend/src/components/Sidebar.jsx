import { FiPlus, FiFilter } from 'react-icons/fi';

export default function Sidebar({ stats, activeCategory, onCategoryChange, onAddNew, loading }) {
  return (
    <aside className="sidebar-filters" id="sidebar-filters">
      <h6 className="sidebar-title">
        <FiFilter className="me-2" />
        Filtrar por Categoria
      </h6>

      <div className="d-grid gap-2">
        <button
          className={`btn btn-filter ${activeCategory === null ? 'active' : ''}`}
          onClick={() => onCategoryChange(null)}
          id="filter-all"
        >
          <span>Todos</span>
          {!loading && (
            <span className="badge rounded-pill badge-count">
              {stats.reduce((s, x) => s + x.cantidad, 0)}
            </span>
          )}
        </button>

        {loading
          ? Array.from({ length: 4 }).map((_, i) => (
              <div key={i} className="placeholder-glow">
                <span className="placeholder col-12 rounded" style={{ height: 38 }}></span>
              </div>
            ))
          : stats.map((s) => (
              <button
                key={s.categoria}
                className={`btn btn-filter ${activeCategory === s.categoria ? 'active' : ''}`}
                onClick={() => onCategoryChange(s.categoria)}
                id={`filter-${s.categoria}`}
              >
                <span>{s.categoria}</span>
                <span className="badge rounded-pill badge-count">{s.cantidad}</span>
              </button>
            ))}
      </div>

      <hr className="my-3 sidebar-divider" />

      <button
        className="btn btn-add-bar w-100 d-flex align-items-center justify-content-center gap-2"
        id="btn-add-new-bar"
        onClick={onAddNew}
      >
        <FiPlus size={16} />
        Agregar Nuevo Bar
      </button>
    </aside>
  );
}
