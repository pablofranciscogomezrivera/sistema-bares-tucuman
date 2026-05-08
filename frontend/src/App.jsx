import { useState, useEffect, useCallback } from 'react';
import Navbar from './components/Navbar';
import BarList from './components/BarList';
import { getBares } from './api/baresApi';
import './index.css';

function App() {
  const [bares, setBares] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);

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

  useEffect(() => {
    fetchBares(1);
  }, []);

  return (
    <div className="app-wrapper" id="app">
      <Navbar />

      <main className="main-content">
        <section className="catalog-section" id="catalog">
          <div className="container-fluid px-4">
            <h1 className="section-heading mb-4">Bares</h1>
            <BarList
              bares={bares}
              page={page}
              totalPages={totalPages}
              onPageChange={(p) => fetchBares(p)}
              loading={loading}
            />
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
