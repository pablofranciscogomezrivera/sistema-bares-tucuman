import { FiMapPin } from 'react-icons/fi';

const CATEGORY_COLORS = {
  Restobares: 'badge-cat-primary',
  Bares: 'badge-cat-success',
  'Cervecerías': 'badge-cat-warning',
  Pubs: 'badge-cat-info',
  Restaurantes: 'badge-cat-danger',
};

export default function BarCard({ bar }) {
  const badgeClass = CATEGORY_COLORS[bar.categoriaAMostrar] || 'badge-cat-default';

  return (
    <div className="card bar-card mb-3" id={`bar-card-${bar.id}`}>
      <div className="row g-0">
        <div className="col-md-8">
          <div className="card-body d-flex flex-column h-100 py-3">
            <div className="mb-2">
              <span className={`badge ${badgeClass} text-uppercase`}>
                {bar.categoriaAMostrar}
              </span>
            </div>

            <h5 className="card-title bar-name mb-1">{bar.nombre}</h5>
            <p className="bar-location mb-2">
              <FiMapPin size={13} className="me-1" />
              {bar.ubicacion || 'Ubicacion no disponible'}
            </p>
            {bar.fuente && (
              <small className="bar-source mt-auto">
                Fuente: {bar.fuente}
              </small>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
