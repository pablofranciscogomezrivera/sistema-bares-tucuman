import { useState, useEffect, useCallback } from 'react';
import Navbar from './components/Navbar';
import Sidebar from './components/Sidebar';
import BarList from './components/BarList';
import { getBares, getStats } from './api/baresApi';
import './index.css';

function App() {
  const [bares, setBares] = useState([]);
  const [stats, setStats] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [activeCategory, setActiveCategory] = useState(null);
  const [loading, setLoading] = useState(true);
  const [loadingStats, setLoadingStats] = useState(true);

  const fetchBares = useCallback(async (p = page) => {
    setLoading(true);
    try {
      const res = await getBares(p);
      setBares(res.data.data);
      setPage(res.data.page);
      setTotalPages(res.data.totalPages);
    } catch (err) {
      console.error('Error fetching bares:', err);
    } finally {
      setLoading(false);
    }
  }, [page]);

  const fetchStats = useCallback(async () => {
    setLoadingStats(true);
    try {
      const res = await getStats();
      setStats(res.data);
    } catch (err) {
      console.error('Error fetching stats:', err);
    } finally {
      setLoadingStats(false);
    }
  }, []);

  useEffect(() => {
    fetchBares(1);
    fetchStats();
  }, []);

  const filteredBares = activeCategory
    ? bares.filter((b) => b.categoriaAMostrar === activeCategory)
    : bares;

  return (
    <div className="app-wrapper" id="app">
      <Navbar />

      <main className="main-content">
        <section className="catalog-section" id="catalog">
          <div className="container-fluid px-4">
            <div className="row g-4">
              <div className="col-lg-3">
                <Sidebar
                  stats={stats}
                  activeCategory={activeCategory}
                  onCategoryChange={setActiveCategory}
                  onAddNew={() => {}}
                  loading={loadingStats}
                />
              </div>
              <div className="col-lg-9">
                <BarList
                  bares={filteredBares}
                  page={page}
                  totalPages={totalPages}
                  onPageChange={(p) => fetchBares(p)}
                  loading={loading}
                />
              </div>
            </div>
          </div>
        </section>
      </main>

      <footer className="app-footer" id="app-footer">
        <div className="container-fluid px-4 text-center">
          <p className="mb-0 small">
            Tucuman Bares &mdash; Prueba Tecnica &copy; {new Date().getFullYear()}
          </p>
        </div>
      </footer>
    </div>
  );
}

export default App;
