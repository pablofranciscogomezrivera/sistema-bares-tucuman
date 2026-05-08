import axios from 'axios';

const API = axios.create({
  baseURL: 'https://localhost:44312/api/bares',
});

export const getBares = (page = 1, pageSize = 6) =>
  API.get('/', { params: { page, pageSize } });

export const getStats = () => API.get('/stats');

export const createBar = (data) => API.post('/', data);

export const updateBar = (id, data) => API.put(`/${id}`, data);

export const deleteBar = (id) => API.delete(`/${id}`);

export const triggerSync = () => API.post('/sync');

export const getSyncLogs = () => API.get('/sync/logs');

export default API;
