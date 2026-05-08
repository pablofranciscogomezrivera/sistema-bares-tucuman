import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from 'recharts';
import { FiBarChart2, FiDatabase, FiClock } from 'react-icons/fi';

const CHART_COLORS = ['#B1C20E', '#10b981', '#3b82f6', '#f59e0b', '#ef4444', '#8b5cf6'];

export default function Dashboard({ stats, totalBares, lastSync, loading }) {
  const chartData = stats.map((s) => ({
    name: s.categoria,
    value: s.cantidad,
  }));

  return (
    <section className="dashboard-section" id="dashboard">
      <div className="container-fluid px-4">
        <div className="row g-4">
          <div className="col-lg-5">
            <div className="row g-3">
              <div className="col-6">
                <div className="kpi-card h-100 d-flex flex-column justify-content-center align-items-center" id="kpi-total">
                  <div className="kpi-icon-wrap">
                    <FiDatabase size={20} />
                  </div>
                  <div className="kpi-value">
                    {loading ? <span className="placeholder col-4"></span> : totalBares}
                  </div>
                  <div className="kpi-label">Total de Bares</div>
                </div>
              </div>
              <div className="col-6">
                <div className="kpi-card h-100 d-flex flex-column justify-content-center align-items-center" id="kpi-categories">
                  <div className="kpi-icon-wrap kpi-icon-blue">
                    <FiBarChart2 size={20} />
                  </div>
                  <div className="kpi-value">
                    {loading ? <span className="placeholder col-4"></span> : stats.length}
                  </div>
                  <div className="kpi-label">Categorias</div>
                </div>
              </div>
              <div className="col-12">
                <div className="kpi-card kpi-card-wide" id="kpi-last-sync">
                  <div className="kpi-icon-wrap kpi-icon-amber">
                    <FiClock size={20} />
                  </div>
                  <div>
                    <div className="kpi-label mb-1">Ultima Sincronizacion</div>
                    <div className="kpi-value kpi-value-sm">
                      {loading
                        ? <span className="placeholder col-8"></span>
                        : (lastSync || 'Sin registros')
                      }
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="col-lg-7">
            <div className="chart-card" id="chart-categorias">
              <h6 className="chart-title">
                <FiBarChart2 className="me-2" />
                Distribucion por Categoria
              </h6>
              {loading ? (
                <div className="d-flex justify-content-center align-items-center" style={{ height: 220 }}>
                  <div className="spinner-border text-secondary spinner-border-sm" role="status" />
                </div>
              ) : chartData.length === 0 ? (
                <div className="text-center text-secondary py-5">Sin datos</div>
              ) : (
                <ResponsiveContainer width="100%" height={220}>
                  <PieChart>
                    <Pie
                      data={chartData}
                      cx="50%"
                      cy="50%"
                      innerRadius={55}
                      outerRadius={90}
                      paddingAngle={3}
                      dataKey="value"
                      stroke="none"
                    >
                      {chartData.map((_, i) => (
                        <Cell key={i} fill={CHART_COLORS[i % CHART_COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip
                      contentStyle={{
                        background: '#1a1a1a',
                        border: '1px solid #333',
                        borderRadius: '8px',
                        color: '#fff',
                        fontSize: '0.82rem',
                      }}
                      formatter={(value, name) => [`${value} bares`, name]}
                    />
                  </PieChart>
                </ResponsiveContainer>
              )}
              {!loading && chartData.length > 0 && (
                <div className="chart-legend">
                  {chartData.map((item, i) => (
                    <span key={item.name} className="chart-legend-item">
                      <span
                        className="chart-legend-dot"
                        style={{ background: CHART_COLORS[i % CHART_COLORS.length] }}
                      />
                      {item.name} ({item.value})
                    </span>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
