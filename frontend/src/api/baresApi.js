import axios from 'axios';

const API = axios.create({
  baseURL: 'https://localhost:44312/api/bares',
});

export const getBares = (page = 1, pageSize = 6) =>
  API.get('/', { params: { page, pageSize } });

export const getStats = () => API.get('/stats');

export default API;
