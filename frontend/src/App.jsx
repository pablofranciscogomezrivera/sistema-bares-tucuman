import { useState, useEffect, useCallback } from 'react';
import Navbar from './components/Navbar';
import Dashboard from './components/Dashboard';
import Sidebar from './components/Sidebar';
import BarList from './components/BarList';
import BarModal from './components/BarModal';
import LogsModal from './components/LogsModal';
import { getBares, getStats, createBar, updateBar, deleteBar, triggerSync, getSyncLogs } from './api/baresApi';
import './index.css';

function App() {
  const [bares, setBares] = useState([]);
  const [stats, setStats] = useState([]);
  const [totalBares, setTotalBares] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [activeCategory, setActiveCategory] = useState(null);
  const [loading, setLoading] = useState(true);
  const [loadingStats, setLoadingStats] = useState(true);
  const [saving, setSaving] = useState(false);
  const [syncing, setSyncing] = useState(false);
  const [showBarModal, setShowBarModal] = useState(false);
  const [showLogsModal, setShowLogsModal] = useState(false);
  const [editingBar, setEditingBar] = useState(null);
  const [lastSync, setLastSync] = useState(null);
  const [toast, setToast] = useState(null);

  const showToast = (message, type = 'success') => {
    setToast({ message, type });
    setTimeout(() => setToast(null), 4000);
  };

  const fetchBares = useCallback(async (p = page) => {
    setLoading(true);
    try {
      const res = await getBares(p);
      setBares(res.data.data);
      setPage(res.data.page);
      setTotalPages(res.data.totalPages);
      setTotalBares(res.data.total);
    } catch (err) {
      console.error('Error fetching bares:', err);
      showToast('Error al cargar los bares', 'danger');
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

  const fetchLastSync = useCallback(async () => {
    try {
      const res = await getSyncLogs();
      if (res.data && res.data.length > 0) {
        const latest = res.data[0];
        const d = new Date(latest.timestamp);
        setLastSync(d.toLocaleString('es-AR', {
          day: '2-digit', month: '2-digit', year: 'numeric',
          hour: '2-digit', minute: '2-digit',
        }));
      }
    } catch (err) {
      console.error('Error fetching last sync:', err);
    }
  }, []);

  useEffect(() => {
    fetchBares(1);
    fetchStats();
    fetchLastSync();
  }, []);

  const handleSaveBar = async (formData, id) => {
    setSaving(true);
    try {
      if (id) {
        await updateBar(id, formData);
        showToast('Bar actualizado correctamente');
      } else {
        await createBar(formData);
        showToast('Bar creado correctamente');
      }
      setShowBarModal(false);
      setEditingBar(null);
      fetchBares(page);
      fetchStats();
    } catch (err) {
      console.error('Save error:', err);
      if (err.response?.data) {
        const errors = err.response.data;
        if (Array.isArray(errors)) {
          const msg = errors.map(e => e.error || e.Error || JSON.stringify(e)).join(' | ');
          showToast(msg, 'danger');
        } else if (typeof errors === 'object' && errors.errors) {
          const msgs = Object.values(errors.errors).flat().join(' | ');
          showToast(msgs, 'danger');
        } else if (typeof errors === 'string') {
          showToast(errors, 'danger');
        } else {
          showToast('Error al guardar el bar', 'danger');
        }
      } else {
        showToast('Error al guardar el bar', 'danger');
      }
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteBar = async (bar) => {
    if (!window.confirm(`Eliminar "${bar.nombre}"?`)) return;
    try {
      await deleteBar(bar.id);
      showToast('Bar eliminado correctamente');
      fetchBares(page);
      fetchStats();
    } catch (err) {
      console.error('Delete error:', err);
      showToast('Error al eliminar el bar', 'danger');
    }
  };

  const handleSync = async () => {
    setSyncing(true);
    try {
      const res = await triggerSync();
      const added = res.data?.baresAgregados ?? 0;
      showToast(`Sincronizacion completada. ${added} bares agregados.`);
      fetchBares(1);
      fetchStats();
      fetchLastSync();
    } catch (err) {
      console.error('Sync error:', err);
      showToast('Error al sincronizar', 'danger');
    } finally {
      setSyncing(false);
    }
  };

  const handleEdit = (bar) => { setEditingBar(bar); setShowBarModal(true); };
  const handleAddNew = () => { setEditingBar(null); setShowBarModal(true); };

  const filteredBares = activeCategory
    ? bares.filter((b) => b.categoriaAMostrar === activeCategory)
    : bares;

  return (
    <div className="app-wrapper" id="app">
      <Navbar
        onSync={handleSync}
        onShowLogs={() => setShowLogsModal(true)}
        syncing={syncing}
      />

      <main className="main-content">
        <Dashboard
          stats={stats}
          totalBares={totalBares}
          lastSync={lastSync}
          loading={loadingStats}
        />

        <section className="catalog-section" id="catalog">
          <div className="container-fluid px-4">
            <div className="row g-4">
              <div className="col-lg-3">
                <Sidebar
                  stats={stats}
                  activeCategory={activeCategory}
                  onCategoryChange={setActiveCategory}
                  onAddNew={handleAddNew}
                  loading={loadingStats}
                />
              </div>
              <div className="col-lg-9">
                <BarList
                  bares={filteredBares}
                  page={page}
                  totalPages={totalPages}
                  onPageChange={(p) => fetchBares(p)}
                  onEdit={handleEdit}
                  onDelete={handleDeleteBar}
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

      <BarModal
        show={showBarModal}
        bar={editingBar}
        onClose={() => { setShowBarModal(false); setEditingBar(null); }}
        onSave={handleSaveBar}
        saving={saving}
      />

      <LogsModal
        show={showLogsModal}
        onClose={() => setShowLogsModal(false)}
      />

      {toast && (
        <div className="toast-container position-fixed bottom-0 end-0 p-3" id="toast-container">
          <div className={`toast show align-items-center text-bg-${toast.type} border-0`} role="alert">
            <div className="d-flex">
              <div className="toast-body fw-semibold">{toast.message}</div>
              <button type="button" className="btn-close btn-close-white me-2 m-auto" onClick={() => setToast(null)} />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
