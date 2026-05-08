import { useState, useEffect } from 'react';

const CATEGORIAS = [
  'BaresClasicos',
  'Restobares',
  'Cervecerias',
  'Cafeterias',
  'Pubs',
  'Otros',
];

const emptyForm = {
  nombre: '',
  ubicacion: '',
  categoriaAMostrar: 'BaresClasicos',
  categoria: 'BaresClasicos',
  aiDescription: '',
};

export default function BarModal({ show, bar, onClose, onSave, saving }) {
  const [form, setForm] = useState(emptyForm);
  const isEditing = !!bar;

  useEffect(() => {
    if (bar) {
      setForm({
        nombre: bar.nombre || '',
        ubicacion: bar.ubicacion || '',
        categoriaAMostrar: bar.categoriaAMostrar || 'BaresClasicos',
        categoria: bar.categoria || bar.categoriaAMostrar || '',
        aiDescription: bar.aiDescription || '',
      });
    } else {
      setForm(emptyForm);
    }
  }, [bar, show]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    if (name === 'categoriaAMostrar') {
      setForm((f) => ({
        ...f,
        categoriaAMostrar: value,
        categoria: value,
      }));
    } else {
      setForm((f) => ({ ...f, [name]: value }));
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const dto = {
      nombre: form.nombre,
      ubicacion: form.ubicacion,
      categoria: form.categoria,
      categoriaAMostrar: form.categoriaAMostrar,
    };
    if (isEditing) {
      dto.aiDescription = form.aiDescription;
    }
    onSave(dto, bar?.id);
  };

  if (!show) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-panel" onClick={(e) => e.stopPropagation()}>

        <div className="modal-panel-header">
          <h5 className="modal-panel-title">
            {isEditing ? 'Editar Bar' : 'Nuevo Bar'}
          </h5>
          <button type="button" className="btn-close" onClick={onClose} aria-label="Cerrar" />
        </div>

        <form onSubmit={handleSubmit}>
          <div className="modal-panel-body">
            <div className="mb-3">
              <label className="modal-label" htmlFor="input-nombre">Nombre</label>
              <input
                type="text"
                className="modal-input"
                id="input-nombre"
                name="nombre"
                value={form.nombre}
                onChange={handleChange}
                required
                placeholder="Ej: La Birreria del Centro"
              />
            </div>
            <div className="mb-3">
              <label className="modal-label" htmlFor="input-ubicacion">Ubicacion</label>
              <input
                type="text"
                className="modal-input"
                id="input-ubicacion"
                name="ubicacion"
                value={form.ubicacion}
                onChange={handleChange}
                required
                placeholder="Ej: Av. Avellaneda 500"
              />
            </div>
            <div className="mb-3">
              <label className="modal-label" htmlFor="input-categoria">Categoria</label>
              <select
                className="modal-input"
                id="input-categoria"
                name="categoriaAMostrar"
                value={form.categoriaAMostrar}
                onChange={handleChange}
              >
                {CATEGORIAS.map((cat) => (
                  <option key={cat} value={cat}>{cat}</option>
                ))}
              </select>
            </div>
            {isEditing && (
              <div className="mb-3">
                <label className="modal-label" htmlFor="input-ai-description">Descripcion IA</label>
                <textarea
                  className="modal-input"
                  id="input-ai-description"
                  name="aiDescription"
                  value={form.aiDescription}
                  onChange={handleChange}
                  rows={3}
                  placeholder="Descripcion generada por IA..."
                />
              </div>
            )}
          </div>

          <div className="modal-panel-footer">
            <div className="d-flex justify-content-between align-items-center w-100">
              <button type="button" className="btn-modal-cancel" onClick={onClose} disabled={saving}>
                Cancelar
              </button>
              <button type="submit" className="btn-modal-submit" disabled={saving} id="btn-save-bar">
                {saving
                  ? (<><span className="spinner-border spinner-border-sm me-2" role="status" />Guardando...</>)
                  : (isEditing ? 'Actualizar' : 'Crear Bar')
                }
              </button>
            </div>
          </div>
        </form>

      </div>
    </div>
  );
}
